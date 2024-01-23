using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This is the Base Class of the Syncs.
	/// All these functions are expected to be implemented by the Sync classes.
	/// </summary>
	public abstract class BaseSync : MonoBehaviour
	{
		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public abstract void Initialize(NetObject p_Object);

		/// <summary>
		/// The function called when a Syncable needs to be cleaned.
		/// This function should make the IsDirty return false.
		/// </summary>
		public abstract void Clean();

		/// <summary>
		/// The function called to see if a Syncable is dirty.
		/// Returns true if it need to be Synced.
		/// </summary>
		/// <returns>Returns true if it need to be Synced.</returns>
		public abstract bool IsDirty();

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public abstract void WriteData(LidNet.NetBuffer p_Msg);

		/// <summary>
		/// Receives all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to read the data from</param>
		public abstract void ReceiveData(LidNet.NetBuffer p_Msg);

		/// <summary>
		/// Called when this game instance gets control of the NetObject.
		/// </summary>
		/// <param name="p_Object">The NetObject this game instance gets control of.</param>
		public virtual void OnGainOwnership(NetObject p_Object)
		{
		}

		/// <summary>
		/// Called when this game instance loses control of the NetObject.
		/// </summary>
		/// <param name="p_Object">The NetObject this game instance loses control of.</param>
		public virtual void OnLoseOwnership(NetObject p_Object)
		{
		}
	}
}
