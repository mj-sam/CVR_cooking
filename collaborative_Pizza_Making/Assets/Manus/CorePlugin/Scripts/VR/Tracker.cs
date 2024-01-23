using Hermes.Protocol;
using UnityEngine;

namespace Manus.VR
{
	/// <summary>
	/// Tracker information
	/// </summary>
	public class Tracker
	{
		/// <summary>
		/// The type that this tracker is.
		/// </summary>
		public Utility.VRTrackerType type;

		/// <summary>
		/// The unique ID of the tracker
		/// Its recommended to get this ID from the hardware, so it's inique and doesn't change  
		/// </summary>
		public string deviceID;

		/// <summary>
		/// Is true when its a VR headset
		/// </summary>
		public bool isHMD;

		/// <summary>
		/// What index does this tracker belong to, usually it's 0.
		/// When multiple trackers of the same type are connected this number increases depending on when it connected.
		/// </summary>
		public int typeIndex;

		/// <summary>
		/// What index does this tracker belong to, usually it's 0.
		/// When multiple trackers of the same type are connected this number increases depending on when it connected.
		/// </summary>
		public int userTypeIndex;

		/// <summary>
		/// The tracking system device index, this isn't always used.
		/// </summary>
		public uint deviceIndex;

		/// <summary>
		/// The tracker's position.
		/// </summary>
		public Vector3 position;

		/// <summary>
		/// The tracker's rotation.
		/// </summary>
		public Quaternion rotation;

		/// <summary>
		/// The quality of the tracking data
		/// </summary>
		public TrackingQuality trackingQuality;

		/// <summary>
		/// Instantiates the tracker class with 'invalid' values.
		/// </summary>
		public Tracker()
		{
			type = Utility.VRTrackerType.Max;
			typeIndex = -1;
			userTypeIndex = -1;
			deviceIndex = 6969;
			position = Vector3.zero;
			rotation = Quaternion.identity;
			trackingQuality = TrackingQuality.Untrackable;
		}

		/// <summary>
		/// Instantiates the tracker class with a device index and type.
		/// </summary>
		/// <param name="p_DeviceIndex"></param>
		/// <param name="p_Type"></param>
		public Tracker(uint p_DeviceIndex, Utility.VRTrackerType p_Type, string p_DeviceID)
		{
			deviceIndex = p_DeviceIndex;
			type = p_Type;
			deviceID = p_DeviceID;
			typeIndex = -1;

			position = Vector3.zero;
			rotation = Quaternion.identity;
			trackingQuality = TrackingQuality.Untrackable;
		}
	}
}
