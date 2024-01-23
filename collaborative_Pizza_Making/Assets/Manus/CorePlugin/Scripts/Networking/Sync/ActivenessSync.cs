using LidNet = Lidgren.Network;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This syncs a GameObject's activeness state
	/// </summary>
	[UnityEngine.DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Activeness (Sync)")]
	public class ActivenessSync : BaseSync
	{
		bool m_Active;

		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public override void Initialize(Manus.Networking.NetObject p_Object)
		{
			m_Active = gameObject.activeSelf;
		}

		/// <summary>
		/// The function called when a Syncable needs to be cleaned.
		/// This function should make the IsDirty return false.
		/// </summary>
		public override void Clean()
		{
			m_Active = gameObject.activeSelf;
		}

		/// <summary>
		/// The function called to see if a Syncable is dirty.
		/// Returns true if it need to be Synced.
		/// </summary>
		/// <returns>Returns true if it need to be Synced.</returns>
		public override bool IsDirty()
		{
			if (m_Active != gameObject.activeSelf) return true;
			return false;
		}

		/// <summary>
		/// Receives all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to read the data from</param>
		public override void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			m_Active = p_Msg.ReadBoolean();

			gameObject.SetActive(m_Active);
		}

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
			p_Msg.Write(m_Active);
		}
	}
}
