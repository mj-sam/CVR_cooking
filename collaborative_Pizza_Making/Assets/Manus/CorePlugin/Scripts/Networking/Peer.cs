using System.Collections.Generic;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	public delegate void ReceiveMessageFunc(LidNet.NetIncomingMessage _Message);

	/// <summary>
	/// This class is what the Client and Server are based upon.
	/// It contains information and functions required for basic functionality.
	/// </summary>
	public abstract class Peer
	{
		/// <summary>
		/// The NetworkManager that this peer has been created by.
		/// </summary>
		protected NetworkManager m_Manager = null;

		/// <summary>
		/// The NetObjectManager that this peer uses to manage its objects.
		/// </summary>
		protected NetObjectManager m_NetObjectManager;

		/// <summary>
		/// The ID assigned to this Peer by itself or the peer who controls ID assignment.
		/// </summary>
		protected long m_ID = 0;

		protected Dictionary<ushort, ReceiveMessageFunc> m_Messages = new Dictionary<ushort, ReceiveMessageFunc>();

		/// <summary>
		/// Gets the ID of this peer.
		/// </summary>
		/// <returns>The ID of this peer.</returns>
		public long GetID()
		{
			return m_ID;
		}

		/// <summary>
		/// Register a message to listen for.
		/// Custom message types should start with ID's LARGER than Message.Type.CustomMessage.
		/// </summary>
		/// <param name="p_ID">A Message Type</param>
		/// <param name="p_Msg">The Function to call when receiving this message</param>
		public void RegisterMessage(Message.Type p_ID, ReceiveMessageFunc p_Msg)
		{
			RegisterMessage((ushort)p_ID, p_Msg);
		}

		/// <summary>
		/// Register a message to listen for.
		/// Custom message types should start with ID's LARGER than Message.Type.CustomMessage.
		/// </summary>
		/// <param name="p_ID">A Message Type</param>
		/// <param name="p_Msg">The Function to call when receiving this message</param>
		public void RegisterMessage(ushort p_ID, ReceiveMessageFunc p_Msg)
		{
			m_Messages.Add(p_ID, p_Msg);
		}

		/// <summary>
		/// Stop listening for a certain message.
		/// Custom message types should start with ID's LARGER than Message.Type.CustomMessage.
		/// </summary>
		/// <param name="p_ID">A Message Type</param>
		public void UnregisterMessage(Message.Type p_ID)
		{
			m_Messages.Remove((ushort)p_ID);
		}

		/// <summary>
		/// Stop listening for a certain message.
		/// Custom message types should start with ID's LARGER than Message.Type.CustomMessage.
		/// </summary>
		/// <param name="p_ID">A Message Type</param>
		public void UnregisterMessage(ushort p_ID)
		{
			m_Messages.Remove(p_ID);
		}

		/// <summary>
		/// Sets the NetObjectManager for managing the NetObjects.
		/// </summary>
		/// <param name="p_Manager">The NetObjectManager</param>
		public void SetNetObjectManager(NetObjectManager p_Manager)
		{
			m_NetObjectManager = p_Manager;
		}

		/// <summary>
		/// This function must be implemented to start the Peer.
		/// </summary>
		public abstract void Start();

		/// <summary>
		/// This function must be implemented to shut the Peer down.
		/// </summary>
		public abstract void Shutdown();

		/// <summary>
		/// This function must be implemented to handle the Incoming Messages.
		/// </summary>
		public abstract void HandleIncomingMessages();

		/// <summary>
		/// Update function, updates whatever is desired on this Peer.
		/// </summary>
		/// <param name="p_DT">(Delta Time) Time passed since last Update Call</param>
		public abstract void Update(float p_DT);

		/// <summary>
		/// This function must be implemented to allow objects to be created.
		/// </summary>
		/// <param name="p_NetObj">The prefab of the NetObject to create.</param>
		/// <param name="p_Position">The initial position of the spawned object.</param>
		/// <param name="p_Rotation">The initial rotation of the spawned object.</param>
		/// <returns>NULL if NetObject not found or not instantly created</returns>
		public abstract NetObject CreateObject(NetObject p_NetObj, UnityEngine.Vector3 p_Position, UnityEngine.Quaternion p_Rotation);

		/// <summary>
		/// This function must be implemented to allow objects to be destroyed.
		/// </summary>
		/// <param name="p_NetObj">The prefab of the NetObject to destroy.</param>
		public abstract void DestroyObject(NetObject p_NetObj);

		/// <summary>
		/// This function must be implemented to allow objects to be controlled by others.
		/// </summary>
		/// <param name="p_NetObj">The prefab of the NetObject to take control of.</param>
		public abstract void TakeControlOfObject(NetObject p_NetObj);

		/// <summary>
		/// This function must be implemented to allow objects to be released by others.
		/// </summary>
		/// <param name="p_NetObj">The prefab of the NetObject to release control of.</param>
		public abstract void ReleaseControlOfObject(NetObject p_NetObj);

		/// <summary>
		/// Get the NetObject with a given NetID.
		/// </summary>
		/// <param name="p_NetID">NetID to get</param>
		/// <returns></returns>
		public NetObject GetNetObject(long p_NetID)
		{
			if (m_NetObjectManager == null) return null;
			return m_NetObjectManager.GetNetObject(p_NetID);
		}

		/// <summary>
		/// Returns all the NetObjects.
		/// </summary>
		/// <returns>A List of NetObjects</returns>
		public List<NetObject> GetNetObjects()
		{
			if (m_NetObjectManager == null) return null;
			return m_NetObjectManager.GetNetObjects();
		}

		/// <summary>
		/// Returns all the NetObjects that a given player controls.
		/// </summary>
		/// <param name="p_PlayerNetID">The controller's ID</param>
		/// <returns>A List of NetObjects</returns>
		public List<NetObject> GetNetObjectsControlledByPlayer(long p_PlayerNetID)
		{
			if (m_NetObjectManager == null) return null;
			return m_NetObjectManager.GetNetObjectsControlledByPlayer(p_PlayerNetID);
		}

		/// <summary>
		/// Call this function to send messages.
		/// </summary>
		/// <param name="p_Type">The Message Type</param>
		/// <param name="p_Msg">The Message</param>
		public void SendMessage(Message.Type p_Type, LidNet.NetBuffer p_Msg)
		{
			SendMessage((ushort)p_Type, p_Msg);
		}

		/// <summary>
		/// Call this function to send custom messages to the server.
		/// </summary>
		/// <param name="p_Type">The Message Type</param>
		/// <param name="p_Msg">The Message</param>
		public abstract void SendMessage(ushort p_Type, LidNet.NetBuffer p_Msg);
	}
}
