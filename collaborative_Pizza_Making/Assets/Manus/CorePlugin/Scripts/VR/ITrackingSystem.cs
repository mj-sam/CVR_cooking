
namespace Manus.VR
{
	public interface ITrackingSystem
	{
		/// <summary>
		/// This is called when the Tracker Manager initializes the Tracking System.
		/// This function can be used to set a defined amount of trackers or subscribe to certain system specific events.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		void Initialize(TrackerManagerInternalData p_Data);

		/// <summary>
		/// This is called when the Tracker Manager gets enabled or enables this Tracking System.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		void OnEnabled(TrackerManagerInternalData p_Data);

		/// <summary>
		/// This is called when the Tracker Manager gets disabled or disables this Tracking System.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		void OnDisabled(TrackerManagerInternalData p_Data);

		/// <summary>
		/// This is called when the Tracker Manager updates the trackers.
		/// This happens every Update.
		/// The trackersChanged boolean is expected to be set in the _Data input if the tracker types or amounts are modified.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		void UpdatePoses(TrackerManagerInternalData p_Data);
	}
}
