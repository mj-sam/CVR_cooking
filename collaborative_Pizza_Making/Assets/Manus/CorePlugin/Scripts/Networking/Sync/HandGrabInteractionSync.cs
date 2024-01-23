using LidNet = Lidgren.Network;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This is syncs the necessary Hand information.
	/// </summary>
	[UnityEngine.DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Hand Grab Interaction (Sync)")]
	public class HandGrabInteractionSync : BaseSync
	{
		Interaction.HandGrabInteraction m_HandGrabInteraction;

		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public override void Initialize(NetObject p_Object)
		{
			m_HandGrabInteraction = GetComponent<Interaction.HandGrabInteraction>();
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
		}

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
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
	}
}
