using LidNet = Lidgren.Network;
using HProt = Hermes.Protocol;
using Manus.Utility;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This is syncs the necessary Hand information.
	/// </summary>
	[UnityEngine.DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Hand (Sync)")]
	public class HandSync : BaseSync
	{
		Hand.Hand m_Hand;

		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public override void Initialize(Manus.Networking.NetObject p_Object)
		{
			m_Hand = GetComponent<Hand.Hand>();
			m_Hand.isLocalPlayer = false;
		}

		/// <summary>
		/// The function called when a Syncable needs to be cleaned.
		/// This function should make the IsDirty return false.
		/// </summary>
		public override void Clean()
		{
		}

		/// <summary>
		/// The function called to see if a Syncable is dirty.
		/// Returns true if it need to be Synced.
		/// </summary>
		/// <returns>Returns true if it need to be Synced.</returns>
		public override bool IsDirty()
		{
			return true;
		}

		/// <summary>
		/// Receives all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to read the data from</param>
		public override void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			if (m_Hand == null) m_Hand = GetComponent<Hand.Hand>();
			bool t_Data = p_Msg.ReadBoolean();
			if (!t_Data) return;
			if (m_Hand.data == null) m_Hand.data = new Hermes.Glove.Data(null, 0, HProt.HandType.UnknownChirality);
			m_Hand.data.ReceiveNetData(p_Msg);
		}

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
			if (m_Hand == null) m_Hand = GetComponent<Hand.Hand>();
			if (m_Hand.data == null)
			{
				p_Msg.Write(false);
				return;
			}
			p_Msg.Write(true);
			m_Hand.data.WriteNetData(p_Msg);
		}

		/// <summary>
		/// Called when this game instance gets control of the NetObject.
		/// </summary>
		/// <param name="p_Object">The NetObject this game instance gets control of.</param>
		public override void OnGainOwnership(NetObject p_Object)
		{
			m_Hand.isLocalPlayer = true;
			m_Hand.enabled = true;
		}

		/// <summary>
		/// Called when this game instance loses control of the NetObject.
		/// </summary>
		/// <param name="p_Object">The NetObject this game instance loses control of.</param>
		public override void OnLoseOwnership(NetObject p_Object)
		{
			m_Hand.isLocalPlayer = false;
			m_Hand.enabled = false;
		}
	}
}


namespace Manus.Hermes.Glove
{
	public partial class Data
	{
		public void ReceiveNetData(LidNet.NetBuffer p_Msg)
		{
			if (m_CareTaker != null) //dump said info...
			{
				p_Msg.ReadUInt32();
				p_Msg.ReadInt32();
				for (int i = 0; i < m_Fingers.Length; i++)
				{
					Finger t_Finger = new Finger((FingerType)i);
					t_Finger.ReceiveNetData(p_Msg);
				}
				return;
			}
			m_ID = p_Msg.ReadUInt32();
			m_HandType = (HandType)p_Msg.ReadInt32();

			for (int i = 0; i < m_Fingers.Length; i++)
			{
				m_Fingers[i].ReceiveNetData(p_Msg);
			}
		}

		public void WriteNetData(LidNet.NetBuffer p_Msg)
		{
			p_Msg.Write(m_ID);
			p_Msg.Write((int)m_HandType);

			for (int i = 0; i < m_Fingers.Length; i++)
			{
				m_Fingers[i].WriteNetData(p_Msg);
			}
		}
	}

	public partial class Finger
	{
		public void ReceiveNetData(LidNet.NetBuffer p_Msg)
		{
			for (int i = 0; i < m_Joints.Length; i++)
			{
				m_Joints[i].ReceiveNetData(p_Msg);
			}
		}

		public void WriteNetData(LidNet.NetBuffer p_Msg)
		{
			for (int i = 0; i < m_Joints.Length; i++)
			{
				m_Joints[i].WriteNetData(p_Msg);
			}
		}
	}

	public partial class FingerJoint
	{
		public void ReceiveNetData(LidNet.NetBuffer p_Msg)
		{
			m_Flex = p_Msg.ReadFloat();
			m_Stretch = p_Msg.ReadFloat();
			m_Spread = p_Msg.ReadFloat();
			m_Position = p_Msg.ReadVector3();
			m_Rotation = p_Msg.ReadQuaternion();
		}

		public void WriteNetData(LidNet.NetBuffer p_Msg)
		{
			p_Msg.Write(m_Flex);
			p_Msg.Write(m_Stretch);
			p_Msg.Write(m_Spread);
			p_Msg.Write(m_Position);
			p_Msg.Write(m_Rotation);
		}
	}
}
