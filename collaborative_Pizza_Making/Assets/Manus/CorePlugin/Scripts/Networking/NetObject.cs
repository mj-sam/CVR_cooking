using System.Collections.Generic;
using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// This is the Networked Object behaviour, it contains the most basic required information on a Networked Object.
	/// The NetObject has information on Ownership and Ownership changes.
	/// It keeps track of all the Syncables and helps manage data changes and gathering.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Networking/Net Object")]
	public class NetObject : MonoBehaviour
	{
		[Utility.ReadOnly]
		public long m_NetID = long.MinValue;
		[Utility.ReadOnly]
		public int m_PrefabID = int.MinValue; //not sure if this number is never used

		[Utility.ReadOnly]
		public long m_ControllerID = 0;

		public bool destroyOnControllerDisconnected = false;

		List<Sync.BaseSync> m_Syncables = new List<Sync.BaseSync>();

		public UnityEngine.Events.UnityEvent onGainOwnership;
		public UnityEngine.Events.UnityEvent onLoseOwnership;

		/// <summary>
		/// Get this objects syncables.
		/// </summary>
		/// <returns>A list of syncables</returns>
		public List<Sync.BaseSync> syncables
		{
			get
			{
				return m_Syncables;
			}
		}

		/// <summary>
		/// Does this instance own this NetObject
		/// </summary>
		public bool isOwnedByMe
		{
			get
			{
				var t_Peer = NetworkManager.instance.GetPeer();
				if (t_Peer == null) return false;
				return m_ControllerID == t_Peer.GetID();
			}
		}

		/// <summary>
		/// Get this object's controller ID
		/// </summary>
		/// <returns>Controller ID</returns>
		public long GetControllerID()
		{
			return m_ControllerID;
		}

		/// <summary>
		/// Sets the NetObject's ID, returns true if MyID owns this object.
		/// </summary>
		/// <param name="p_ControllerID"></param>
		/// <param name="p_MyID"></param>
		/// <returns>True if I own this object</returns>
		public bool SetControllerID(long p_ControllerID, long p_MyID)
		{
			if (m_ControllerID == p_ControllerID) return m_ControllerID == p_MyID;

			if (p_ControllerID == p_MyID)
			{
				m_ControllerID = p_ControllerID;
				for (int i = 0; i < m_Syncables.Count; i++)
				{
					m_Syncables[i].OnGainOwnership(this);
				}
				onGainOwnership.Invoke();
				return true;
			}
			if (m_ControllerID == p_MyID)
			{
				for (int i = 0; i < m_Syncables.Count; i++)
				{
					m_Syncables[i].OnLoseOwnership(this);
				}
				onLoseOwnership.Invoke();
			}
			m_ControllerID = p_ControllerID;
			return false;
		}

		/// <summary>
		/// Get a sync's Index
		/// </summary>
		/// <returns>Index</returns>
		public int GetSyncableIndex(Sync.BaseSync p_Syncable)
		{
			for (int i = 0; i < m_Syncables.Count; i++)
			{
				if (m_Syncables[i] == p_Syncable) return i;
			}
			return -1;
		}

		/// <summary>
		/// Get a syncable for a given index
		/// </summary>
		/// <returns>Syncable, NULL when not found</returns>
		public Sync.BaseSync GetSyncable(int p_Idx)
		{
			if (p_Idx >= m_Syncables.Count) return null;
			if (p_Idx < 0) return null;
			return m_Syncables[p_Idx];
		}

		/// <summary>
		/// Function called when initializing the object.
		/// This function should only be called by the Network Manager.
		/// </summary>
		/// <param name="p_ID">Object ID</param>
		/// <param name="p_PrefabID">Prefab ID</param>
		/// <param name="p_AutoDisable">Does this object automatically disable itself?</param>
		/// <returns></returns>
		public bool Initialize(long p_ID, int p_PrefabID = int.MinValue, bool p_AutoDisable = true)
		{
			m_NetID = p_ID;
			m_ControllerID = 0;
			m_PrefabID = p_PrefabID;
			m_Syncables.AddRange(GetComponentsInChildren<Sync.BaseSync>(true));
			for (int i = 0; i < m_Syncables.Count; i++)
			{
				m_Syncables[i].Initialize(this);
			}
			if (!p_AutoDisable) return true;
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Has the data changed since last Write?
		/// </summary>
		/// <returns></returns>
		public bool IsDirty()
		{
			for (int i = 0; i < m_Syncables.Count; i++)
			{
				if (m_Syncables[i].IsDirty())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Write the dirty data to a Net Buffer for sending.
		/// Automatically sets the data to clean!
		/// </summary>
		/// <param name="p_Msg"></param>
		public void WriteData(LidNet.NetBuffer p_Msg)
		{
			for (int i = 0; i < m_Syncables.Count; i++)
			{
				if (m_Syncables[i].IsDirty())
				{
					m_Syncables[i].Clean();
					p_Msg.Write(i);
					m_Syncables[i].WriteData(p_Msg);
				}
			}
			p_Msg.Write(-1);
		}

		/// <summary>
		/// Writes all the data to the Net Buffer for sending.
		/// </summary>
		/// <param name="p_Msg"></param>
		public void WriteAllData(LidNet.NetBuffer p_Msg)
		{
			for (int i = 0; i < m_Syncables.Count; i++)
			{
				m_Syncables[i].WriteData(p_Msg);
			}
		}

		/// <summary>
		/// Receives and applies all the data from specific Syncables the Net Buffer.
		/// </summary>
		/// <param name="p_Msg"></param>
		public void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			int t_Idx = p_Msg.ReadInt32();
			while (t_Idx != -1)
			{
				m_Syncables[t_Idx].ReceiveData(p_Msg);
				t_Idx = p_Msg.ReadInt32();
			}
		}

		/// <summary>
		/// Receives and applies all the data in the Net Buffer.
		/// Assumes all the data passed in exists as Syncable.
		/// </summary>
		/// <param name="p_Msg"></param>
		public void ReceiveAllData(LidNet.NetBuffer p_Msg)
		{
			for (int i = 0; i < m_Syncables.Count; i++)
			{
				m_Syncables[i].ReceiveData(p_Msg);
			}
		}
	}
}
