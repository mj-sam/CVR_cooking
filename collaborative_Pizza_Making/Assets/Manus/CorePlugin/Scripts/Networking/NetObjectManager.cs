using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking
{
	/// <summary>
	/// This Manager ensures that all NetObjects (if used correctly) have Unique IDs.
	/// The NetObjectManager should only be used via the NetworkManager, Server and Client.
	/// Using it elsewhere may cause out of sync problems.
	/// </summary>
	public class NetObjectManager
	{
		Dictionary<long, NetObject> m_NetObjects = new Dictionary<long, NetObject>();
		//Net ID
		long m_NextNetID;

		List<NetObject> m_EnableNetObjectsOnServer = new List<NetObject>();


		public NetObjectManager()
		{
			m_NextNetID = long.MinValue + 1;
		}

		/// <summary>
		/// Get the next NetID.
		/// The current implementation has a range of the Long type, which should be more than enough. (-9,223,372,036,854,775,808 to 9,223,372,036,854,775,807)
		/// </summary>
		/// <returns>Next ID</returns>
		public long GetNextNetID()
		{
			m_NextNetID++;
			return m_NextNetID;
		}

		/// <summary>
		/// Add NetObjects, initialize them with the correct NetID's and PrefabIDs.
		/// Ensures that NetObjects get tracked when starting as a Server for enabling.
		/// </summary>
		/// <param name="p_NetObjs">The objects to add and initialize</param>
		public void AddNetObjects(NetObject[] p_NetObjs)
		{
			for (int n = 0; n < p_NetObjs.Length; n++)
			{
				if (p_NetObjs[n].Initialize(GetNextNetID(), p_NetObjs[n].m_PrefabID))
				{
					m_EnableNetObjectsOnServer.Add(p_NetObjs[n]);
				}
				m_NetObjects[p_NetObjs[n].m_NetID] = p_NetObjs[n];
			}
		}

		/// <summary>
		/// Get the NetObject with a given NetID.
		/// </summary>
		/// <param name="p_NetID">NetID to get</param>
		/// <returns></returns>
		public NetObject GetNetObject(long p_NetID)
		{
			NetObject t_NObj;
			if (m_NetObjects.TryGetValue(p_NetID, out t_NObj)) return t_NObj;
			return null;
		}

		/// <summary>
		/// Adds a NetObject to the list of NetObjects.
		/// Uses the NetID defined in the NetObject.
		/// </summary>
		/// <param name="p_Obj">NetObject to add</param>
		public void AddNetObject(NetObject p_Obj)
		{
			m_NetObjects[p_Obj.m_NetID] = p_Obj;
		}

		/// <summary>
		/// Adds a NetObject with a given NetID to the list of NetObjects.
		/// Does not assign the NetObject's NetID!
		/// </summary>
		/// <param name="p_NetID">NetID to use</param>
		/// <param name="p_Obj">NetObject to add</param>
		public void AddNetObject(long p_NetID, NetObject p_Obj)
		{
			m_NetObjects[p_NetID] = p_Obj;
		}

		/// <summary>
		/// Removes a NetObject with a given NetID from the list of NetObjects.
		/// </summary>
		/// <param name="p_NetID">NetID to remove</param>
		/// <returns>Return true if successful</returns>
		public bool RemoveNetObject(long p_NetID)
		{
			return m_NetObjects.Remove(p_NetID);
		}

		/// <summary>
		/// Sets all the NetObjects' controllers to a given PlayerNetID.
		/// </summary>
		/// <param name="p_PlayerNetID">The Player ID to set as the controller of all the NetObjects</param>
		public void InitializeControlOfAllObjects(long p_PlayerNetID)
		{
			foreach (KeyValuePair<long, NetObject> t_Entry in m_NetObjects)
			{
				t_Entry.Value.SetControllerID(p_PlayerNetID, p_PlayerNetID);
			}
		}

		/// <summary>
		/// Enables all the NetObjects that are defined to be activated when hosting as a Server.
		/// </summary>
		public void EnableNetObjectsOnServer()
		{
			for (int i = 0; i < m_EnableNetObjectsOnServer.Count; i++)
			{
				m_EnableNetObjectsOnServer[i].gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Returns all the NetObjects.
		/// </summary>
		/// <returns>A List of NetObjects</returns>
		public List<NetObject> GetNetObjects()
		{
			List<NetObject> t_Objs = new List<NetObject>(m_NetObjects.Values);
			return t_Objs;
		}

		/// <summary>
		/// Returns all the NetObjects that a given player controls.
		/// </summary>
		/// <param name="p_PlayerNetID">The controller's ID</param>
		/// <returns>A List of NetObjects</returns>
		public List<NetObject> GetNetObjectsControlledByPlayer(long p_PlayerNetID)
		{
			List<NetObject> t_Objs = new List<NetObject>();
			foreach (KeyValuePair<long, NetObject> t_Entry in m_NetObjects)
			{
				if (t_Entry.Value.GetControllerID() == p_PlayerNetID)
				{
					t_Objs.Add(t_Entry.Value);
				}
			}
			return t_Objs;
		}

		/// <summary>
		/// Writes all NetObject data to a NetBuffer.
		/// Does not clean dirty data.
		/// It returns the amount of NetObjects written to the buffer.
		/// </summary>
		/// <param name="p_Buffer"></param>
		/// <returns>Amount of NetObjects written to the buffer</returns>
		public int WriteAllNetObjectData(ref LidNet.NetBuffer p_Buffer)
		{
			foreach (KeyValuePair<long, NetObject> t_Entry in m_NetObjects)
			{
				p_Buffer.Write(t_Entry.Key);
				p_Buffer.Write(t_Entry.Value.m_PrefabID);
				p_Buffer.Write(t_Entry.Value.GetControllerID());
				p_Buffer.Write(t_Entry.Value.gameObject.activeSelf);

				LidNet.NetBuffer t_Buff = new LidNet.NetBuffer();
				t_Entry.Value.WriteAllData(t_Buff);
				p_Buffer.Write(t_Buff.LengthBits);
				p_Buffer.Write(t_Buff);
			}
			return m_NetObjects.Count;
		}

		/// <summary>
		/// Writes all dirty NetObject data to a NetBuffer.
		/// Automatically cleans the dirty data.
		/// It returns the amount of NetObjects written to the buffer.
		/// </summary>
		/// <param name="p_Buffer"></param>
		/// <returns>Amount of NetObjects written to the buffer</returns>
		public int WriteDirtyNetObjectData(ref LidNet.NetBuffer p_Buffer)
		{
			int t_ObjCount = 0;
			foreach (KeyValuePair<long, NetObject> t_Entry in m_NetObjects)
			{
				if (t_Entry.Value.IsDirty())
				{
					t_ObjCount++;
					p_Buffer.Write(t_Entry.Key);
					LidNet.NetBuffer t_Buff = new LidNet.NetBuffer();
					t_Entry.Value.WriteData(t_Buff);
					p_Buffer.Write(t_Buff.LengthBits);
					p_Buffer.Write(t_Buff);
				}
			}
			return t_ObjCount;
		}
	}
}
