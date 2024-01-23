using LidNet = Lidgren.Network;
using Manus.Interaction;
using System.Collections.Generic;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This component syncs the necessary Hand information between the server and clients.
	/// </summary>
	[UnityEngine.DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Grabbable Object (Sync)")]
	public class GrabbableObjectSync : BaseSync, Interaction.IGrabbable
	{
		GrabbedObject m_GrabbedObject = null;

		bool m_Dirty = true;
		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public override void Initialize(NetObject p_Object)
		{

		}

		/// <summary>
		/// The function called when a Syncable needs to be cleaned.
		/// This function should make the IsDirty return false.
		/// </summary>
		public override void Clean()
		{
			m_Dirty = false;
		}

		/// <summary>
		/// The function called to see if a Syncable is dirty.
		/// Returns true if it need to be Synced.
		/// </summary>
		/// <returns>Returns true if it need to be Synced.</returns>
		public override bool IsDirty()
		{
			return m_Dirty;
		}

		/// <summary>
		/// Receives all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to read the data from</param>
		public override void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			//all hands filter existing
			bool t_DataAvailable = p_Msg.ReadBoolean();
			if (!t_DataAvailable)
			{
				if (m_GrabbedObject != null)
				{
					Destroy(m_GrabbedObject);
				}
				return;
			}
			if (m_GrabbedObject == null) m_GrabbedObject = gameObject.AddComponent<GrabbedObject>();

			int t_Cnt = p_Msg.ReadInt32();

			List<GrabbedObject.Info> t_Hands = new List<GrabbedObject.Info>();

			for (int i = 0; i < t_Cnt; i++)
			{
				t_Hands.Add(ReadGrabbedObjectInfo(ref p_Msg));
			}

			m_GrabbedObject.hands = t_Hands;
		}

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
			//all hands
			bool t_DataAvailable = m_GrabbedObject != null;
			p_Msg.Write(t_DataAvailable);
			if (!t_DataAvailable) return;
			p_Msg.Write((int)m_GrabbedObject.hands.Count);

			for (int i = 0; i < m_GrabbedObject.hands.Count; i++)
			{
				GrabbedObject.Info t_Info = m_GrabbedObject.hands[i];
				p_Msg.Write(WriteGrabbedObjectInfo(ref t_Info));
			}
		}

		/// <summary>
		/// Called when this game instance gets control of the NetObject.
		/// </summary>
		/// <param name="p_Object">The NetObject this game instance gets control of.</param>
		public override void OnGainOwnership(NetObject p_Object)
		{
		}

		/// <summary>
		/// Called when this game instance loses control of the NetObject.
		/// </summary>
		/// <param name="p_Object">The NetObject this game instance loses control of.</param>
		public override void OnLoseOwnership(NetObject p_Object)
		{
		}

		/// <summary>
		/// Called when this starts getting grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedStart(GrabbedObject p_Object)
		{
			var t_TS = GetComponent<TransformSync>();
			if (t_TS != null)
			{
				t_TS.enabled = false;
			}
			m_GrabbedObject = p_Object;
			m_Dirty = true;
		}

		/// <summary>
		/// Called when this stops being grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedEnd(GrabbedObject p_Object)
		{
			var t_TS = GetComponent<TransformSync>();
			if (t_TS != null)
			{
				t_TS.enabled = true;
			}
			m_GrabbedObject = null;
			m_Dirty = true;
		}

		/// <summary>
		/// Called every FixedUpdate when this is grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedFixedUpdate(GrabbedObject p_Object)
		{
		}

		/// <summary>
		/// Called when a new grabber starts grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the added grabber</param>
		public void OnAddedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
			//tell everyone to add me...?
			if (NetworkManager.instance.GetServer() != null)
			{
				m_Dirty = true;
				return;
			}
			NetObject t_InteracterNObj = p_Info.interacter.GetComponentInParent<NetObject>();
			if (t_InteracterNObj.isOwnedByMe)
			{
				//Peer sends request to server, server adds stuff and then sends results to everyone
				NetObject t_NObj = GetComponentInParent<NetObject>();
				if (t_NObj == null)
				{
					UnityEngine.Debug.LogError("Net Object not found!");
					return;
				}

				LidNet.NetBuffer t_Buff = new LidNet.NetBuffer();
				t_Buff.Write(true); //grab
				t_Buff.Write(t_NObj.m_NetID);
				int t_SyncID = t_NObj.GetSyncableIndex(this);
				if (t_SyncID == -1)
				{
					UnityEngine.Debug.LogWarning("Interacter does not have a GrabbableObjectSync!");
				}
				t_Buff.Write(t_SyncID);
				t_Buff.Write(WriteGrabbedObjectInfo(ref p_Info));
				NetworkManager.instance.SendNetMessage(Message.Type.GrabbableObjectSync, t_Buff);
			}
		}

		/// <summary>
		/// Called when a grabber stops grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the removed grabber</param>
		public void OnRemovedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
			//tell everyone to remove me...?
			if (NetworkManager.instance.GetServer() != null)
			{
				m_Dirty = true;
				return;
			}
			if (p_Info.interacter == null) return;
			NetObject t_InteracterNObj = p_Info.interacter.GetComponentInParent<NetObject>();
			if (t_InteracterNObj.isOwnedByMe)
			{
				//Peer sends request to server, server adds stuff and then sends results to everyone
				NetObject t_NObj = GetComponentInParent<NetObject>();
				if (t_NObj == null)
				{
					UnityEngine.Debug.LogError("Net Object not found!");
					return;
				}

				LidNet.NetBuffer t_Buff = new LidNet.NetBuffer();
				t_Buff.Write(false); //release
				t_Buff.Write(t_NObj.m_NetID);
				int t_SyncID = t_NObj.GetSyncableIndex(this);
				if (t_SyncID == -1)
				{
					UnityEngine.Debug.LogWarning("Interacter does not have a GrabbableObjectSync!");
				}
				t_Buff.Write(t_SyncID);
				t_Buff.Write(WriteGrabbedObjectInfo(ref p_Info));
				NetworkManager.instance.SendNetMessage(Message.Type.GrabbableObjectSync, t_Buff);
			}
		}

		/// <summary>
		/// Writes all the GrabbedObject.Info to a buffer and returns it.
		/// </summary>
		/// <param name="p_Info">The Info</param>
		/// <returns>A NetBuffer</returns>
		LidNet.NetBuffer WriteGrabbedObjectInfo(ref GrabbedObject.Info p_Info)
		{
			LidNet.NetBuffer t_Buff = new LidNet.NetBuffer();
			NetObject t_InteracterNObj = p_Info.interacter.GetComponentInParent<NetObject>();
			long t_NID = long.MinValue;
			int t_SyncID = -1;
			if (t_InteracterNObj != null)
			{
				t_NID = t_InteracterNObj.m_NetID;
				HandGrabInteractionSync t_Synx = p_Info.interacter.GetComponent<HandGrabInteractionSync>();
				t_SyncID = t_InteracterNObj.GetSyncableIndex(t_Synx);
				if (t_SyncID == -1)
				{
					UnityEngine.Debug.LogWarning("Interacter does not have a HandGrabInteractionSync!");
				}
			}
			t_Buff.Write(t_NID);
			t_Buff.Write(t_SyncID);
			t_Buff.Write(p_Info.distance);
			t_Buff.Write(p_Info.nearestColliderPoint);
			t_Buff.Write(p_Info.handToObject);
			t_Buff.Write(p_Info.objectToHand);
			t_Buff.Write(p_Info.objectInteractorForward);
			t_Buff.Write(p_Info.handToObjectRotation);
			return t_Buff;
		}

		/// <summary>
		/// Reads a GrabbedObject.Info from a NetBuffer
		/// </summary>
		/// <param name="p_Msg">A NetBuffer</param>
		/// <returns>The Info</returns>
		GrabbedObject.Info ReadGrabbedObjectInfo(ref LidNet.NetBuffer p_Msg)
		{
			GrabbedObject.Info t_Info = new GrabbedObject.Info(null);

			long t_NID = p_Msg.ReadInt64();
			int t_SyncID = p_Msg.ReadInt32();
			NetObject t_NObj = NetworkManager.instance.GetNetObject(t_NID);
			if (t_NObj != null)
			{
				var t_Sync = t_NObj.GetSyncable(t_SyncID);
				if (t_Sync)
				{
					t_Info.interacter = t_Sync.GetComponent<Interaction.HandGrabInteraction>();
					if(t_Info.interacter==null)
					{
						UnityEngine.Debug.LogWarning("Interacter does not have a HandGrabInteraction!");
					}
				}
			}

			t_Info.distance = p_Msg.ReadFloat();
			t_Info.nearestColliderPoint = p_Msg.ReadVector3();
			t_Info.handToObject = p_Msg.ReadVector3();
			t_Info.objectToHand = p_Msg.ReadVector3();
			t_Info.objectInteractorForward = p_Msg.ReadVector3();
			t_Info.handToObjectRotation = p_Msg.ReadQuaternion();
			return t_Info;
		}

		/// <summary>
		/// This message is received when another player requests a grab
		/// </summary>
		/// <param name="p_Grab">To grab or not to grab</param>
		/// <param name="p_Msg">The NetBuffer containing information on the grab</param>
		public void OnGrabbableObjectSyncMessage(bool p_Grab, LidNet.NetBuffer p_Msg)
		{
			GrabbedObject.Info t_Info = ReadGrabbedObjectInfo(ref p_Msg);

			if (p_Grab)
			{
				if (m_GrabbedObject == null) m_GrabbedObject = gameObject.AddComponent<GrabbedObject>();

				for (int i = 0; i < m_GrabbedObject.hands.Count; i++)
				{
					if (t_Info.interacter == m_GrabbedObject.hands[i].interacter)
					{
						m_GrabbedObject.hands[i] = t_Info;
						return;
					}
				}
				m_GrabbedObject.AddInteractingHand(t_Info);
				return;
			}
			if (m_GrabbedObject == null) return;
			m_GrabbedObject.RemoveInteractingHand(t_Info.interacter);
		}
	}
}
