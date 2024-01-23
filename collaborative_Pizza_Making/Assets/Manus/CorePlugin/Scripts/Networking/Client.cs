using System.Collections.Generic;
using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// The basic Client Implementation.
	/// This class should be inherited from in order to make additions such as specific message listening.
	/// This can be done through the RegisterMessage function.
	/// Custom message types should start with ID's LARGER than Message.Type.CustomMessage
	/// </summary>
	public class Client : Peer
	{
		LidNet.NetClient m_LNet = null;

		float m_UpdateTime = 0.03f;
		float m_UpdateTimer = 0.0f;

		Dictionary<long, NetObject> m_ControlledNetObjects = new Dictionary<long, NetObject>();

		/// <summary>
		/// The servers discovered by our discovery request.
		/// </summary>
		public Dictionary<System.Net.IPEndPoint, object> discoveredServers
		{
			get
			{
				return new Dictionary<System.Net.IPEndPoint, object>(m_DiscoveredServers);
			}
		}

		protected Dictionary<System.Net.IPEndPoint, object> m_DiscoveredServers = new Dictionary<System.Net.IPEndPoint, object>();

		/// <summary>
		/// Initializes the Client with the basic message types
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID, this must be unique and is used for matchmaking</param>
		public Client(NetworkManager p_Man, string p_AppID)
		{
			m_Manager = p_Man;
			LidNet.NetPeerConfiguration t_Config = new LidNet.NetPeerConfiguration(p_AppID)
			{
				AutoFlushSendQueue = false
			};
			t_Config.EnableMessageType(LidNet.NetIncomingMessageType.DiscoveryResponse);
			m_LNet = new LidNet.NetClient(t_Config);

			RegisterMessage(Message.Type.IDInitialize, OnMessageIDInitialize);
			RegisterMessage(Message.Type.ObjectsInitialize, OnMessageObjectsInitialize);
			RegisterMessage(Message.Type.ObjectsUpdate, OnMessageObjectsUpdate);
			RegisterMessage(Message.Type.ObjectCreate, OnMessageObjectCreate);
			RegisterMessage(Message.Type.ObjectDestroy, OnMessageObjectDestroy);
			RegisterMessage(Message.Type.ObjectChangeOwner, OnMessageObjectChangeOwner);
		}

		~Client()
		{
			Shutdown();
		}

		/// <summary>
		/// Starts the client
		/// </summary>
		public override void Start()
		{
			m_LNet.Start();
		}

		/// <summary>
		/// Shuts the client down
		/// </summary>
		public override void Shutdown()
		{
			m_LNet.Shutdown("Client Shutting Down...");
		}

		/// <summary>
		/// Connects to a given IP Address and Port.
		/// </summary>
		/// <param name="p_Host">IP Address</param>
		/// <param name="p_Port">Port</param>
		public void Connect(string p_Host, int p_Port)
		{
			LidNet.NetOutgoingMessage t_Hail = m_LNet.CreateMessage();
			t_Hail.Write(m_Manager.appIdentifier);
			m_LNet.Connect(p_Host, p_Port, t_Hail);
		}

		/// <summary>
		/// Connects to a given IP EndPoint.
		/// </summary>
		/// <param name="p_RemoteEndPoint">EndPoint to connect to</param>
		public void Connect(System.Net.IPEndPoint p_RemoteEndPoint)
		{
			LidNet.NetOutgoingMessage t_Hail = m_LNet.CreateMessage();
			t_Hail.Write(m_Manager.appIdentifier);
			m_LNet.Connect(p_RemoteEndPoint, t_Hail);
		}

		/// <summary>
		/// Disconnects from Server.
		/// </summary>
		public void Disconnect()
		{
			m_LNet.Disconnect("Requested by user");
		}

		/// <summary>
		/// Sends a discovery signal on a given port
		/// </summary>
		/// <param name="p_Port">Port to send the signal on</param>
		public void SendDiscoverySignal(int p_Port)
		{
			m_DiscoveredServers.Clear();
			Debug.Log("Discov");
			m_LNet.DiscoverLocalPeers(p_Port);
		}

		/// <summary>
		/// Handles the incoming messages, Data messages are passed on to the registered ReceiveMessageFunc functions.
		/// </summary>
		public override void HandleIncomingMessages()
		{
			LidNet.NetIncomingMessage t_Msg;
			while ((t_Msg = m_LNet.ReadMessage()) != null)
			{
				switch (t_Msg.MessageType)
				{
					case LidNet.NetIncomingMessageType.DebugMessage:
					case LidNet.NetIncomingMessageType.ErrorMessage:
					case LidNet.NetIncomingMessageType.WarningMessage:
					case LidNet.NetIncomingMessageType.VerboseDebugMessage:
						{
							string t_Text = t_Msg.ReadString();
							Debug.Log(t_Text);
						}
						break;
					case LidNet.NetIncomingMessageType.DiscoveryResponse:
						{
							Debug.Log("Found server at " + t_Msg.SenderEndPoint);

							System.Net.IPEndPoint t_IPEP = t_Msg.SenderEndPoint;
							m_DiscoveredServers[t_IPEP] = OnReceiveDiscoveryMessage(t_Msg);
						}
						break;
					case LidNet.NetIncomingMessageType.StatusChanged:
						{
							LidNet.NetConnectionStatus t_Status = (LidNet.NetConnectionStatus)t_Msg.ReadByte();

							string t_Reason = t_Msg.ReadString();
							if (t_Status == LidNet.NetConnectionStatus.Connected)
							{
								OnConnected(t_Reason);
								break;
							}

							if (t_Status == LidNet.NetConnectionStatus.Disconnected)
							{
								OnDisconnected(t_Reason);
								break;
							}

							Debug.Log(t_Status.ToString() + ": " + t_Reason);

						}
						break;
					case LidNet.NetIncomingMessageType.Data:
						{
							ushort t_Type = t_Msg.ReadUInt16();
							ReceiveMessageFunc t_NetMsgFunc;
							if (m_Messages.TryGetValue(t_Type, out t_NetMsgFunc))
							{
								t_NetMsgFunc(t_Msg);
							}
							else
							{
								Debug.LogWarning("Missing message type: " + t_Type);
							}
						}
						break;
				}
				m_LNet.Recycle(t_Msg);
			}
		}

		/// <summary>
		/// Gets the ID the Server sees as this client.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageIDInitialize(LidNet.NetIncomingMessage p_Msg)
		{
			m_ID = p_Msg.ReadInt64();
		}

		/// <summary>
		/// The message received used to initialize the objects in the scene.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectsInitialize(LidNet.NetIncomingMessage p_Msg)
		{
			int t_Size = p_Msg.ReadInt32();
			Dictionary<NetObject, LidNet.NetBuffer> t_DataDic = new Dictionary<NetObject, LidNet.NetBuffer>();
			for (int i = 0; i < t_Size; i++)
			{
				long t_ID = p_Msg.ReadInt64();
				int t_PrefabID = p_Msg.ReadInt32();
				long t_ControllerID = p_Msg.ReadInt64();
				bool t_Enabled = p_Msg.ReadBoolean();

				int t_DataSize = p_Msg.ReadInt32();

				byte[] t_Bytes = new byte[LidNet.NetUtility.BytesToHoldBits(t_DataSize)];
				p_Msg.ReadBits(t_Bytes, 0, t_DataSize);
				NetObject t_NObj = m_NetObjectManager.GetNetObject(t_ID);
				if (t_NObj == null)
				{
					t_NObj = OnNewNetObject(t_PrefabID, t_ID);
				}

				if (t_NObj == null) return;
				t_NObj.gameObject.SetActive(t_Enabled);

				if (t_NObj.SetControllerID(t_ControllerID, m_ID))
				{
					m_ControlledNetObjects.Add(t_NObj.m_NetID, t_NObj);
				}

				LidNet.NetBuffer t_ObjBuff = new LidNet.NetBuffer();
				t_ObjBuff.Write(t_Bytes);
				t_ObjBuff.Position = 0;
				t_DataDic.Add(t_NObj, t_ObjBuff);
			}
			foreach (var t_Data in t_DataDic)
			{
				t_Data.Key.ReceiveAllData(t_Data.Value);
			}
		}

		/// <summary>
		/// The message received when objects are updated.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectsUpdate(LidNet.NetIncomingMessage p_Msg)
		{
			int t_ObjCount = p_Msg.ReadInt32();
			for (int i = 0; i < t_ObjCount; i++)
			{
				long t_ID = p_Msg.ReadInt64();
				int t_DataSize = p_Msg.ReadInt32();
				byte[] t_Bytes = new byte[LidNet.NetUtility.BytesToHoldBits(t_DataSize)];
				p_Msg.ReadBits(t_Bytes, 0, t_DataSize);
				NetObject t_NObj = m_NetObjectManager.GetNetObject(t_ID);
				if (t_NObj == null)
				{
					Debug.LogWarning("Update Net Message possibly corrupt!");
					continue;
				}
				LidNet.NetBuffer t_ObjBuff = new LidNet.NetBuffer();
				t_ObjBuff.Write(t_Bytes);
				t_ObjBuff.Position = 0;
				t_NObj.ReceiveData(t_ObjBuff);
			}
		}

		/// <summary>
		/// Update function, updates the object changes made on objects owned by this client.
		/// </summary>
		/// <param name="p_DT">(Delta Time) Time passed since last Update Call</param>
		public override void Update(float p_DT)
		{
			m_UpdateTimer -= p_DT;
			if (m_UpdateTimer > 0) return;
			m_UpdateTimer = m_UpdateTime;

			//Debug.Log("Checking Changes...");

			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectsUpdate);

			LidNet.NetBuffer t_ObjBuff = new LidNet.NetBuffer();
			int t_ObjCount = 0;

			foreach (KeyValuePair<long, NetObject> t_Entry in m_ControlledNetObjects)
			{
				if (t_Entry.Value.IsDirty())
				{
					t_ObjCount++;
					t_ObjBuff.Write(t_Entry.Key);
					LidNet.NetBuffer t_Buff = new LidNet.NetBuffer();
					t_Entry.Value.WriteData(t_Buff);
					t_ObjBuff.Write(t_Buff.LengthBits);
					t_ObjBuff.Write(t_Buff);
				}
			}

			if (t_ObjCount != 0)
			{
				t_Msg.Write(t_ObjCount);
				t_Msg.Write(t_ObjBuff);
				//Debug.Log("Sending Changes...");
				m_LNet.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
				m_LNet.FlushSendQueue();
			}

		}

		/// <summary>
		/// Function called when a connection is made to a server.
		/// </summary>
		/// <param name="p_Reason">A string explaining what happened</param>
		protected virtual void OnConnected(string p_Reason)
		{
			Debug.Log("Connected: " + p_Reason);
		}

		/// <summary>
		/// Function called when the client disconnects.
		/// </summary>
		/// <param name="p_Reason">The reason for disconnection</param>
		protected virtual void OnDisconnected(string p_Reason)
		{
			Debug.Log("Disconnected: " + p_Reason);
		}

		/// <summary>
		/// Function called when the client receives a discovery message from a server, usually contains server information.
		/// The returned object is saved to the Discovered Servers list which contains the endpoints of the servers and whatever information is passed back.
		/// </summary>
		/// <param name="p_Msg"></param>
		/// <returns>Server information</returns>
		protected virtual object OnReceiveDiscoveryMessage(LidNet.NetIncomingMessage p_Msg)
		{
			Debug.Log(p_Msg.ReadString());
			return null;
		}

		/// <summary>
		/// Call this function when an object needs to be created.
		/// Internally this sends a request to the server to create an object.
		/// The object created will have this client as owner
		/// </summary>
		/// <param name="p_NetObj">The prefab of the NetObject to create. (Must be registered as a Networked Prefab!)</param>
		/// <param name="p_Position">The initial position of the spawned object.</param>
		/// <param name="p_Rotation">The initial rotation of the spawned object.</param>
		/// <returns>NULL, since the NetObject is not instantly created</returns>
		public override NetObject CreateObject(NetObject p_NetObj, Vector3 p_Position, Quaternion p_Rotation)
		{
			if (p_NetObj.m_PrefabID == int.MinValue)
			{
				Debug.LogError("Object (" + p_NetObj.name + ") is not a registered prefab!");
				return null;
			}
			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectCreate);
			t_Msg.Write(p_NetObj.m_PrefabID);
			t_Msg.Write(m_ID);
			t_Msg.Write(p_Position);
			t_Msg.Write(p_Rotation);
			m_LNet.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			m_LNet.FlushSendQueue();
			return null;
		}

		/// <summary>
		/// Function called when the client receives a create object message.
		/// This creates an object on the client and handles the initialization.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectCreate(LidNet.NetIncomingMessage p_Msg)
		{
			long t_ID = p_Msg.ReadInt64();
			int t_PrefabID = p_Msg.ReadInt32();
			NetObject t_NO = OnNewNetObject(t_PrefabID, t_ID);
			if (t_NO != null)
			{
				long t_ControllerID = p_Msg.ReadInt64();
				t_NO.ReceiveAllData(p_Msg);

				if (t_NO.SetControllerID(t_ControllerID, m_ID))
				{
					m_ControlledNetObjects.Add(t_NO.m_NetID, t_NO);
				}
				else
				{
					m_ControlledNetObjects.Remove(t_NO.m_NetID);
				}
			}
		}

		/// <summary>
		/// Function called when a new object needs to be created.
		/// </summary>
		/// <param name="p_PrefabID">Prefab ID registered in the list of network prefabs.</param>
		/// <param name="p_NetID">Net ID of the objects owner.</param>
		/// <returns>The created NetObject, is null if it could'nt be created.</returns>
		NetObject OnNewNetObject(int p_PrefabID, long p_NetID)
		{
			NetObject[] t_AllObjects = NetworkManager.GetPrefabs();
			for (int i = 0; i < t_AllObjects.Length; i++)
			{
				if (t_AllObjects[i].m_PrefabID == p_PrefabID)
				{
					GameObject t_GObj = Object.Instantiate(t_AllObjects[i].gameObject);
					NetObject t_NO = t_GObj.GetComponent<NetObject>();
					if (t_NO)
					{
						t_NO.Initialize(p_NetID, p_PrefabID, false);
						m_NetObjectManager.AddNetObject(t_NO);
						return t_NO;
					}
					Debug.LogError("Object " + t_AllObjects[i].name + "(" + t_AllObjects[i].GetInstanceID() + ") does not have a NetObject!");
					return null;
				}
			}
			Debug.LogError("Object with ID " + p_PrefabID + " does not exist!");
			return null;
		}

		/// <summary>
		/// Call this function when an object needs to be destroyed.
		/// Internally this sends a request to the server to destroy an object.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to destroy.</param>
		public override void DestroyObject(NetObject p_NetObj)
		{
			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectDestroy);

			t_Msg.Write(p_NetObj.m_NetID);

			m_LNet.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			m_LNet.FlushSendQueue();
		}

		/// <summary>
		/// Function called when the client receives a destroy object message.
		/// This destroys an object on the client and handles the cleanup.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectDestroy(LidNet.NetIncomingMessage p_Msg)
		{
			long t_NetID = p_Msg.ReadInt64();
			OnDestroyObject(t_NetID);
		}

		/// <summary>
		/// Function called when an object needs to be destroyed.
		/// </summary>
		/// <param name="p_NetID">Net ID of the objects owner</param>
		/// <returns>The created NetObject, is null if it could'nt be created</returns>
		void OnDestroyObject(long p_NetID)
		{
			NetObject t_NObj = m_NetObjectManager.GetNetObject(p_NetID);
			if (t_NObj == null)
			{
				Debug.LogWarning("DestroyObject warning: Net Object (" + p_NetID + ") does not exist.");
				return;
			}
			m_ControlledNetObjects.Remove(t_NObj.m_NetID);
			m_NetObjectManager.RemoveNetObject(t_NObj.m_NetID);

			GameObject.Destroy(t_NObj.gameObject);
		}

		/// <summary>
		/// Call this function when the client wants to take control of an object.
		/// Internally this sends a request to the server to take control of an object.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to take control of</param>
		public override void TakeControlOfObject(NetObject p_NetObj)
		{
			if (p_NetObj.m_ControllerID == m_ID)
			{
				Debug.LogWarning("Object (" + p_NetObj.name + ") is already mine to control!");
				return;
			}

			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectChangeOwner);
			t_Msg.Write(true);
			t_Msg.Write(p_NetObj.m_NetID);
			m_LNet.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			m_LNet.FlushSendQueue();
		}

		/// <summary>
		/// Call this function when the client wants to release control of an object.
		/// Internally this sends a request to the server to release control of an object.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to take control of</param>
		public override void ReleaseControlOfObject(NetObject p_NetObj)
		{
			if (p_NetObj.m_ControllerID != m_ID)
			{
				Debug.LogWarning("Object (" + p_NetObj.name + ") is not mine to control!");
				return;
			}

			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectChangeOwner);
			t_Msg.Write(false);
			t_Msg.Write(p_NetObj.m_NetID);
			m_LNet.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			m_LNet.FlushSendQueue();
		}

		/// <summary>
		/// Function called when the client receives a change object owner message.
		/// This changes the controller of an object on the client.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectChangeOwner(LidNet.NetIncomingMessage p_Msg)
		{
			long t_NetID = p_Msg.ReadInt64();
			long t_PlayerID = p_Msg.ReadInt64();
			OnChangeObjectController(t_NetID, t_PlayerID);
		}

		/// <summary>
		/// Function called when an object needs to change owner.
		/// </summary>
		/// <param name="p_NetID">Net ID of the objects owner</param>
		/// <param name="p_PlayerID">The ID of the new owner</param>
		void OnChangeObjectController(long p_NetID, long p_PlayerID)
		{
			NetObject t_NObj = m_NetObjectManager.GetNetObject(p_NetID);
			if (t_NObj == null)
			{
				Debug.LogWarning("ChangeObjectController warning: Net Object (" + p_NetID + ") does not exist.");
				return;
			}

			if (t_NObj.SetControllerID(p_PlayerID, m_ID))
			{
				m_ControlledNetObjects.Add(t_NObj.m_NetID, t_NObj);
			}
			else
			{
				m_ControlledNetObjects.Remove(t_NObj.m_NetID);
			}
		}

		/// <summary>
		/// Call this function to send custom messages to the server.
		/// </summary>
		/// <param name="p_Type">The Message Type</param>
		/// <param name="p_Msg">The Message</param>
		public override void SendMessage(ushort p_Type, LidNet.NetBuffer p_Msg)
		{
			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write(p_Type);
			t_Msg.Write(p_Msg);
			m_LNet.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			m_LNet.FlushSendQueue();
		}
	}
}
