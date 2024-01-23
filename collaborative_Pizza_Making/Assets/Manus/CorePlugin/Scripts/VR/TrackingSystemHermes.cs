using UnityEngine;
using Manus.Utility;
using Hermes.Protocol;
using Manus.Hermes;
using System.Threading.Tasks;
using System.Linq;

namespace Manus.VR
{
	/// <summary>
	/// This component manages all the trackers from Hermes
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/VR/Tracking System (Hermes)")]
	public class TrackingSystemHermes : MonoBehaviour, ITrackingSystem
	{
		const int s_MaxTrackedDeviceCount = 64;

		private float m_LastUpdate = 0;
		private TrackerData m_LastTrackingData = null;

		/// <summary>
		/// This is called when the Tracker Manager initializes the Tracking System.
		/// This function can be used to set a defined amount of trackers or subscribe to certain system specific events.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void Initialize(TrackerManagerInternalData p_Data)
		{
			p_Data.trackers.Clear();
			p_Data.trackers.Resize(s_MaxTrackedDeviceCount);
		}

		/// <summary>
		/// This is called when the Tracker Manager gets enabled or enables this Tracking System.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void OnEnabled(TrackerManagerInternalData p_Data)
		{
			ManusManager.instance.communicationHub.trackerUpdate += UpdateTrackers;
		}

		/// <summary>
		/// This is called when the Tracker Manager gets disabled or disables this Tracking System.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void OnDisabled(TrackerManagerInternalData p_Data)
		{
			if (ManusManager.instance?.communicationHub != null)
				ManusManager.instance.communicationHub.trackerUpdate -= UpdateTrackers;
		}

		/// <summary>
		/// This is called when the Tracker Manager updates the trackers.
		/// This happens every Update.
		/// The trackersChanged boolean is expected to be set in the _Data input if the tracker types or amounts are modified.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void UpdatePoses(TrackerManagerInternalData p_Data)
		{
			if (m_LastTrackingData == null)
			{
				if (Mathf.Abs(m_LastUpdate - Time.time) < .5f)
					return;

				m_LastTrackingData = new TrackerData();
			}

			m_LastUpdate = Time.time;

			for (int i = 0; i < m_LastTrackingData.Trackers.Count; i++)
			{
				int t_User = m_LastTrackingData.Trackers[i].UserIndex;
				var t_Tracker = new Tracker
				{
					deviceID = m_LastTrackingData.Trackers[i].ID,
					type = m_LastTrackingData.Trackers[i].Type.ToManusType(),
					isHMD = m_LastTrackingData.Trackers[i].IsHMD,
					typeIndex = 0,
					userTypeIndex = 0,
					position = m_LastTrackingData.Trackers[i].Position.ToUnity(),
					rotation = m_LastTrackingData.Trackers[i].Rotation.ToUnity(),
					trackingQuality = m_LastTrackingData.Trackers[i].Quality
				};

				Tracker t_TargetTracker = p_Data.GetTrackerFromUser(t_User, t_Tracker.deviceID);

				if (t_TargetTracker == null)
				{
					t_TargetTracker = p_Data.AddTrackerToUser(t_Tracker, t_User);
				}
				if (t_TargetTracker.type != t_Tracker.type)
				{
					p_Data.RemoveTrackerFromUser(t_Tracker.deviceID, t_User);
					t_TargetTracker = p_Data.AddTrackerToUser(t_Tracker, t_User);
				}

				if (t_TargetTracker != null)
				{
					t_TargetTracker.position = t_Tracker.position;
					t_TargetTracker.rotation = t_Tracker.rotation;
					t_TargetTracker.trackingQuality = t_Tracker.trackingQuality;
				}
			}

			// Remove non existing trackers
			int[] t_UserIDs = p_Data.userTrackers.Keys.ToArray();

			for (int i = 0; i < t_UserIDs.Length; i++)
			{
				int t_UserID = t_UserIDs[i];

				for (int j = 0; j < p_Data.userTrackers[t_UserID].Length; j++)
				{
					string t_ID = p_Data.userTrackers[t_UserID][j].deviceID;

					bool t_Exist = false;
					foreach (var t_Tracker in m_LastTrackingData.Trackers)
					{
						if (t_Tracker.ID == t_ID && t_Tracker.UserIndex == t_UserID)
						{
							t_Exist = true;
							break;
						}
					}

					if (!t_Exist)
					{
						if (p_Data.RemoveTrackerFromUser(t_ID, t_UserID))
							j--;
					}
				}
			}

			m_LastTrackingData = null;
		}

		private void UpdateTrackers(TrackerData p_TrackerData)
		{
			m_LastTrackingData = p_TrackerData;
		}
	}
}
