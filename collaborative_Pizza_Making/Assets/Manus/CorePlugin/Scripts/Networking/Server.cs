using System.Collections.Generic;
using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// The basic Server Implementation.
	/// This class should be inherited from in order to make additions such as specific message listening.
	/// This can be done through the RegisterMessage function.
	/// Custom message types should start with ID's LARGER than Message.Type.CustomMessage
	/// </summary>
	public class Server : Peer
	{
		LidNet.NetServer m_LNet = null;

		float m_UpdateTime = 0.03f;
		float m_UpdateTimer = 0.0f;

		public delegate void ClientConnectedFunc(long _PlayerNetID);
		public delegate void ClientDisconnectedFunc(long _PlayerNetID);

		public ClientConnectedFunc onClientConnected;
		public ClientDisconnectedFunc onClientDisconnected;

		/// <summary>
		/// Initializes the Server with the basic message types and port connection
		/// </summary>
		/// <param name="p_Man">The Network Manager</param>
		/// <param name="p_AppID">The AppID, this must be unique and is used for matchmaking</param>
		/// <param name="p_Port">Port the server should listen on</param>
		public Server(NetworkManager p_Man, string p_AppID, int p_Port)
		{
			m_Manager = p_Man;
			LidNet.NetPeerConfiguration t_Config = new LidNet.NetPeerConfiguration(p_AppID)
			{
				MaximumConnections = 100,
				Port = p_Port
			};
			t_Config.EnableMessageType(LidNet.NetIncomingMessageType.ConnectionApproval);
			t_Config.EnableMessageType(LidNet.NetIncomingMessageType.DiscoveryRequest);
			m_LNet = new LidNet.NetServer(t_Config);

			RegisterMessage(Message.Type.ObjectsUpdate, OnMessageObjectsUpdate);
			RegisterMessage(Message.Type.ObjectCreate, OnMessageObjectCreate);
			RegisterMessage(Message.Type.ObjectDestroy, OnMessageObjectDestroy);
			RegisterMessage(Message.Type.ObjectChangeOwner, OnMessageObjectChangeOwner);
			RegisterMessage(Message.Type.GrabbableObjectSync, OnMessageGrabbableObjectSync);
		}

		~Server()
		{
			Shutdown();
		}

		/// <summary>
		/// Starts the server and intializes and takes control of all objects
		/// </summary>
		public override void Start()
		{
			m_LNet.Start();
			m_ID = m_LNet.m_uniqueIdentifier;
			m_NetObjectManager.InitializeControlOfAllObjects(m_ID);
		}

		/// <summary>
		/// Shuts the server down
		/// </summary>
		public override void Shutdown()
		{
			m_LNet.Shutdown("Server Shutting Down...");
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
					case LidNet.NetIncomingMessageType.ConnectionApproval:
						{
							string t_String = t_Msg.ReadString();
							if (t_String == m_Manager.appIdentifier)
								t_Msg.SenderConnection.Approve();
							else
								t_Msg.SenderConnection.Deny();
						}
						break;
					case LidNet.NetIncomingMessageType.DiscoveryRequest:
						{
							Debug.Log("Discovery Signal Received");
							// Create a response and write some example data to it
							LidNet.NetOutgoingMessage t_Response = m_LNet.CreateMessage();

							OnRequestServerInfo(t_Response);

							// Send the response to the sender of the request
							m_LNet.SendDiscoveryResponse(t_Response, t_Msg.SenderEndPoint);
						}
						break;
					case LidNet.NetIncomingMessageType.StatusChanged:
						{
							LidNet.NetConnectionStatus t_Status = (LidNet.NetConnectionStatus)t_Msg.ReadByte();

							string t_Reason = t_Msg.ReadString();
							Debug.Log(LidNet.NetUtility.ToHexString(t_Msg.SenderConnection.RemoteUniqueIdentifier) + " " + t_Status + ": " + t_Reason);

							if (t_Status == LidNet.NetConnectionStatus.Connected)
							{
								string t_Greet = t_Msg.SenderConnection.RemoteHailMessage.ReadString();
								Debug.Log("New Connection: " + t_Greet);
								LidNet.NetOutgoingMessage t_ResMsg = m_LNet.CreateMessage();
								t_ResMsg.Write((ushort)Message.Type.IDInitialize);
								t_ResMsg.Write(t_Msg.SenderConnection.RemoteUniqueIdentifier);
								t_Msg.SenderConnection.SendMessage(t_ResMsg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);

								OnConnected(t_Msg.SenderConnection);
							}

							if (t_Status == LidNet.NetConnectionStatus.Disconnected)
							{
								OnDisconnected(t_Msg.SenderConnection);
							}
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
		/// Function called when the server receives a discovery message from a client, they want information about the server.
		/// The client side should have the opposite in OnReceiveDiscoveryMessage to receive the data correctly.
		/// See the SimpleServer.cs for a sample implementation.
		/// </summary>
		/// <param name="p_Msg"></param>
		protected virtual void OnRequestServerInfo(LidNet.NetOutgoingMessage p_Msg)
		{
			p_Msg.Write("Server");
		}

		/// <summary>
		/// Function called when a client makes a connection to the server.
		/// </summary>
		void OnConnected(LidNet.NetConnection p_Connection)
		{
			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectsInitialize);

			LidNet.NetBuffer t_ObjBuff = new LidNet.NetBuffer();
			int t_ObjCount = m_NetObjectManager.WriteAllNetObjectData(ref t_ObjBuff);
			t_Msg.Write(t_ObjCount);
			t_Msg.Write(t_ObjBuff);

			Debug.Log("Sending Init Net Objs...");
			p_Connection.SendMessage(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);

			OnCreateObject(m_Manager.playerPrefab, p_Connection.RemoteUniqueIdentifier, m_Manager.playerSpawnPostion, Quaternion.Euler(m_Manager.playerSpawnRotation));

			onClientConnected?.Invoke(p_Connection.RemoteUniqueIdentifier);
		}

		/// <summary>
		/// Function called when a client disconnects from the server.
		/// </summary>
		void OnDisconnected(LidNet.NetConnection p_Connection)
		{
			//Either destroy or give me, the host, all of your objects...
			List<NetObject> t_NObjs = m_NetObjectManager.GetNetObjectsControlledByPlayer(p_Connection.RemoteUniqueIdentifier);
			for (int i = 0; i < t_NObjs.Count; i++)
			{
				if (t_NObjs[i].destroyOnControllerDisconnected)
				{
					DestroyObject(t_NObjs[i]);
					continue;
				}
				TakeControlOfObject(t_NObjs[i]);
			}

			onClientDisconnected?.Invoke(p_Connection.RemoteUniqueIdentifier);
		}

		/// <summary>
		/// The message received when objects are updated.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectsUpdate(LidNet.NetIncomingMessage p_Msg)
		{
			int t_PassObjCount = 0;
			LidNet.NetBuffer t_PassObjBuff = new LidNet.NetBuffer();

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
				if (t_NObj.GetControllerID() != p_Msg.SenderConnection.RemoteUniqueIdentifier)
				{
					Debug.LogWarning("Update Net Message from connection not the controller!");
					continue;
				}
				t_PassObjCount++;
				LidNet.NetBuffer t_ObjBuff = new LidNet.NetBuffer();
				t_ObjBuff.Write(t_Bytes);
				t_ObjBuff.Position = 0;
				t_NObj.ReceiveData(t_ObjBuff);

				t_PassObjBuff.Write(t_ID);
				t_PassObjBuff.Write(t_ObjBuff.LengthBits);
				t_PassObjBuff.Write(t_ObjBuff);
			}

			if (t_ObjCount != 0)
			{
				List<LidNet.NetConnection> t_AllCons = m_LNet.Connections;
				t_AllCons.Remove(p_Msg.SenderConnection);
				if (t_AllCons.Count > 0)
				{
					LidNet.NetOutgoingMessage t_PassOnMsg = m_LNet.CreateMessage();
					t_PassOnMsg.Write((ushort)Message.Type.ObjectsUpdate);
					t_PassOnMsg.Write(t_PassObjCount);
					t_PassOnMsg.Write(t_PassObjBuff);
					m_LNet.SendMessage(t_PassOnMsg, t_AllCons, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
				}
			}
		}

		/// <summary>
		/// Update function, updates the object changes made on objects owned by the server.
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

			int t_ObjCount = m_NetObjectManager.WriteDirtyNetObjectData(ref t_ObjBuff);

			if (t_ObjCount != 0)
			{
				t_Msg.Write(t_ObjCount);
				t_Msg.Write(t_ObjBuff);
				//Debug.Log("Sending Changes...");
				m_LNet.SendToAll(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			}
		}

		/// <summary>
		/// Call this function when an object needs to be created.
		/// </summary>
		/// <param name="p_NetObj">The prefab of the NetObject to create. (Must be registered as a Networked Prefab!)</param>
		/// <param name="p_Position">The initial position of the spawned object.</param>
		/// <param name="p_Rotation">The initial rotation of the spawned object.</param>
		/// <returns>The created NetObject.</returns>
		public override NetObject CreateObject(NetObject p_NetObj, Vector3 p_Position, Quaternion p_Rotation)
		{
			return OnCreateObject(p_NetObj, m_ID, p_Position, p_Rotation);
		}

		/// <summary>
		/// Function called when the server receives a create object message.
		/// This creates an object on the server and handles the initialization and passes it on to all the clients.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectCreate(LidNet.NetIncomingMessage p_Msg)
		{
			int t_PrefabID = p_Msg.ReadInt32();
			long t_ControllerID = p_Msg.ReadInt64();
			Vector3 t_Pos = p_Msg.ReadVector3();
			Quaternion t_Rot = p_Msg.ReadQuaternion();
			OnCreateObject(t_PrefabID, t_ControllerID, t_Pos, t_Rot);
		}

		/// <summary>
		/// This function tries to create a NetObject belonging to a certain player.
		/// Returns NULL if the NetObject is not found.
		/// </summary>
		/// <param name="p_PrefabID">NetObject's Prefab ID</param>
		/// <param name="p_PlayerID">PlayerNetID</param>
		/// <param name="p_Position">The initial position of the spawned object.</param>
		/// <param name="p_Rotation">The initial rotation of the spawned object.</param>
		/// <returns>NULL if NetObject not found.</returns>
		NetObject OnCreateObject(int p_PrefabID, long p_PlayerID, Vector3 p_Position, Quaternion p_Rotation)
		{
			NetObject[] t_AllObjects = NetworkManager.GetPrefabs();
			for (int i = 0; i < t_AllObjects.Length; i++)
			{
				if (t_AllObjects[i].m_PrefabID == p_PrefabID)
				{
					return OnCreateObject(t_AllObjects[i], p_PlayerID, p_Position, p_Rotation);
				}
			}
			Debug.LogError("Object with ID " + p_PrefabID + " does not exist!");
			return null;
		}

		/// <summary>
		/// This function creates a NetObject belonging to a certain player.
		/// </summary>
		/// <param name="p_NetObject">The NetObject to create</param>
		/// <param name="p_PlayerID">PlayerNetID</param>
		/// <param name="p_Position">The initial position of the spawned object.</param>
		/// <param name="p_Rotation">The initial rotation of the spawned object.</param>
		/// <returns>The created NetObject</returns>
		NetObject OnCreateObject(NetObject p_NetObject, long p_PlayerID, Vector3 p_Position, Quaternion p_Rotation)
		{
			GameObject t_GObj = Object.Instantiate(p_NetObject.gameObject, p_Position, p_Rotation);
			NetObject t_NO = t_GObj.GetComponent<NetObject>();
			if (t_NO)
			{
				t_NO.Initialize(m_NetObjectManager.GetNextNetID(), t_NO.m_PrefabID, false);
				m_NetObjectManager.AddNetObject(t_NO);

				t_NO.SetControllerID(p_PlayerID, m_ID);

				LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
				t_Msg.Write((ushort)Message.Type.ObjectCreate);

				t_Msg.Write(t_NO.m_NetID);
				t_Msg.Write(t_NO.m_PrefabID);
				t_Msg.Write(t_NO.GetControllerID());
				t_NO.WriteAllData(t_Msg);

				m_LNet.SendToAll(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
			}
			return t_NO;
		}

		/// <summary>
		/// Call this function when an object needs to be destroyed.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to destroy.</param>
		public override void DestroyObject(NetObject p_NetObj)
		{
			OnDestroyObject(p_NetObj);
		}

		/// <summary>
		/// Function called when the server receives a destroy object message.
		/// This destroys an object on the server if the player is authorized to do so.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectDestroy(LidNet.NetIncomingMessage p_Msg)
		{
			long t_NetID = p_Msg.ReadInt64();
			OnDestroyObject(t_NetID, p_Msg.SenderConnection.RemoteUniqueIdentifier);
		}

		/// <summary>
		/// This function tries to destroy a NetObject belonging to a certain player.
		/// This destroys an object on the server if the player is authorized to do so.
		/// </summary>
		/// <param name="p_NetID">The NetID of the NetObject to destroy</param>
		/// <param name="p_PlayerID">PlayerNetID</param>
		void OnDestroyObject(long p_NetID, long p_PlayerID)
		{
			NetObject t_NObj = m_NetObjectManager.GetNetObject(p_NetID);
			if (t_NObj == null)
			{
				Debug.LogWarning("DestroyObject warning: Net Object (" + p_NetID + ") does not exist.");
				return;
			}
			if (t_NObj.m_ControllerID != p_PlayerID)
			{
				Debug.LogWarning("Player (" + p_PlayerID + ") isn't authorized to destroy Object (" + p_NetID + ")!");
				return;
			}
			DestroyObject(t_NObj);
		}

		/// <summary>
		/// This function destroys a NetObject and notifies all clients to do the same.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to destroy</param>
		void OnDestroyObject(NetObject p_NetObj)
		{
			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectDestroy);

			t_Msg.Write(p_NetObj.m_NetID);

			m_LNet.SendToAll(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);

			m_NetObjectManager.RemoveNetObject(p_NetObj.m_NetID);

			GameObject.Destroy(p_NetObj.gameObject);
		}

		/// <summary>
		/// Call this function when the server wants to take control of an object.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to take control of</param>
		public override void TakeControlOfObject(NetObject p_NetObj)
		{
			OnChangeObjectController(p_NetObj, m_ID, true);
		}

		/// <summary>
		/// This function is the same as taking control of the object for the server.
		/// </summary>
		/// <param name="p_NetObj">The NetObject to take control of</param>
		public override void ReleaseControlOfObject(NetObject p_NetObj)
		{
			OnChangeObjectController(p_NetObj, m_ID, false);
		}


		/// <summary>
		/// Function called when the server receives a change object owner message.
		/// This is a request to take control of an object
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageObjectChangeOwner(LidNet.NetIncomingMessage p_Msg)
		{
			bool t_Take = p_Msg.ReadBoolean();
			long t_NetID = p_Msg.ReadInt64();

			NetObject t_NObj = m_NetObjectManager.GetNetObject(t_NetID);
			if (t_NObj == null)
			{
				Debug.LogWarning("ChangeObjectController warning: Net Object (" + t_NetID + ") does not exist.");
				return;
			}

			long t_PlayerID = p_Msg.SenderConnection.RemoteUniqueIdentifier;
			//if wants to release, make sure that he is allowed to even request this.
			if (!t_Take && t_NObj.GetControllerID() != t_PlayerID)
			{
				Debug.LogWarning("Object (" + t_NObj.name + ") is not Player (" + t_NetID + ")'s to release!");
				return;
			}
			OnChangeObjectController(t_NObj, t_PlayerID, t_Take);
		}

		/// <summary>
		/// Function called when an object needs to change owner.
		/// </summary>
		/// <param name="p_NetObj">The NetObject that needs to change owner</param>
		/// <param name="p_PlayerID">The ID of the new owner</param>
		/// <param name="p_Take">Take or release control</param>
		void OnChangeObjectController(NetObject p_NetObj, long p_PlayerID, bool p_Take)
		{
			if (!p_Take)
			{
				p_PlayerID = m_ID;
			}

			if (p_NetObj.m_ControllerID == p_PlayerID)
			{
				Debug.LogWarning("Object (" + p_NetObj.name + ") is already Player (" + p_PlayerID + ") to control!");
			}

			p_NetObj.SetControllerID(p_PlayerID, m_ID);

			LidNet.NetOutgoingMessage t_Msg = m_LNet.CreateMessage();
			t_Msg.Write((ushort)Message.Type.ObjectChangeOwner);

			t_Msg.Write(p_NetObj.m_NetID);
			t_Msg.Write(p_PlayerID);

			m_LNet.SendToAll(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
		}


		/// <summary>
		/// Function called when the server receives a grabbable object sync message.
		/// </summary>
		/// <param name="p_Msg"></param>
		void OnMessageGrabbableObjectSync(LidNet.NetIncomingMessage p_Msg)
		{
			bool t_Grab = p_Msg.ReadBoolean();
			long t_NetID = p_Msg.ReadInt64();

			NetObject t_NObj = m_NetObjectManager.GetNetObject(t_NetID);
			if (t_NObj == null)
			{
				Debug.LogWarning("OnMessageGrabbableObjectSync warning: Net Object (" + t_NetID + ") does not exist.");
				return;
			}

			int t_SyncID = p_Msg.ReadInt32();

			var t_Syncs = t_NObj.syncables;
			if(t_Syncs.Count <= t_SyncID || t_SyncID < 0)
			{
				Debug.LogWarning("OnMessageGrabbableObjectSync warning: Net Object Syncable(" + t_SyncID + ") does not exist.");
				return;
			}

			var t_GrObjS = t_Syncs[t_SyncID] as Sync.GrabbableObjectSync;
			if (t_GrObjS == null)
			{
				Debug.LogWarning("OnMessageGrabbableObjectSync warning: Net Object Syncable(" + t_SyncID + ") is not a GrabbableObjectSync.");
				return;
			}
			t_GrObjS.OnGrabbableObjectSyncMessage(t_Grab, p_Msg);
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
			m_LNet.SendToAll(t_Msg, LidNet.NetDeliveryMethod.ReliableOrdered, 0);
		}
	}
}
