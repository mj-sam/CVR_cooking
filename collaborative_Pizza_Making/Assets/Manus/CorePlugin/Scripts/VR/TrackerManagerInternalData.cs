using System.Collections.Generic;
using System.Linq;
using Manus.Utility;
using UnityEngine;

namespace Manus.VR
{
	/// <summary>
	/// This is the class used to share information between the TrackerManager and the ITrackerSystem implementations.
	/// It contains two lists of tracker data, a link to the TrackerManager and a boolean for when the tracker amounts or types change.
	/// </summary>
	public class TrackerManagerInternalData
	{
		/// <summary>
		/// The tracker manager who this 
		/// </summary>
		public TrackerManager trackerManager;

		/// <summary>
		/// The list of trackers.
		/// </summary>
		public List<Tracker> trackers = new List<Tracker>();

		/// <summary>
		/// The list of trackers sorted per Type.
		/// trackersType[Type][Index]
		/// </summary>
		public List<Tracker>[] trackersType = new List<Tracker>[(int)VRTrackerType.Max];

		public Dictionary<int, Tracker[]> userTrackers = new Dictionary<int, Tracker[]>();

		/// <summary>
		/// This boolean MUST be set when either the amount of active trackers change or when the types of trackers change!
		/// This is done in the build in TrackerManagerInternalData functions, however if these are not used it should be set to True.
		/// </summary>
		public bool trackersChanged = true;

		/// <summary>
		/// This function initalizes the data containers and the link to the TrackerManager.
		/// </summary>
		/// <param name="p_TrackerManager"></param>
		public TrackerManagerInternalData(TrackerManager p_TrackerManager)
		{
			trackerManager = p_TrackerManager;
			for (int i = 0; i < trackersType.Length; i++)
			{
				trackersType[i] = new List<Tracker>();
			}
		}

		/// <summary>
		/// Adds a tracker to the user list, finds the first empty slot in the trackersType list and adds it there.
		/// Returns the newly assigned tracker.
		/// </summary>
		/// <param name="p_Tracker"></param>
		/// <param name="p_User"></param>
		/// <returns>returns the newly assigned tracker</returns>
		public Tracker AddTrackerToUser(Tracker p_Tracker, int p_User, bool _TriggerUpdate = true)
		{
			if (p_User < -1 || p_Tracker == null)
				return null;

			while (!userTrackers.ContainsKey(p_User))
			{
				userTrackers.Add(p_User, new Tracker[0]);
			}

			// check if tracker already added
			foreach (var t_Tracker in userTrackers[p_User])
			{
				if (t_Tracker.deviceID == p_Tracker.deviceID)
					return null;
			}

			// get the type index
			int t_TypeCount = 0;
			foreach (var t_Tracker in userTrackers[p_User])
			{
				if (t_Tracker.type == p_Tracker.type)
					t_TypeCount++;
			}

			p_Tracker.userTypeIndex = t_TypeCount;

			userTrackers[p_User] = userTrackers[p_User].Concat(new Tracker[] { p_Tracker }).ToArray();

			if (_TriggerUpdate)
			{
				FromUserListToTypeList();
				trackersChanged = true;
			}

			return p_Tracker;
		}

		/// <summary>
		/// Removes a tracker from the user list
		/// Return if the tracker was removed succesfully
		/// </summary>
		/// <param name="p_TrackerID"></param>
		/// <param name="p_User"></param>
		/// <returns>return if the tracker was removed succesfully</returns>
		public bool RemoveTrackerFromUser(string p_TrackerID, int p_User)
		{
			if (!userTrackers.ContainsKey(p_User))
				return false;

			Tracker[] t_NewTrackerList = new Tracker[0];

			foreach (var t_Tracker in userTrackers[p_User])
			{
				if (t_Tracker.deviceID == p_TrackerID)
					continue;

				t_NewTrackerList = t_NewTrackerList.Concat(new Tracker[] { t_Tracker }).ToArray();
			}

			userTrackers[p_User] = t_NewTrackerList;

			FromUserListToTypeList();
			trackersChanged = true;

			return true;
		}

		/// <summary>
		/// Returns tracker from the user list with a specific ID
		/// </summary>
		/// <param name="p_User"></param>
		/// <param name="p_ID"></param>
		/// <returns>Returns tracker from the user list with a specific ID</returns>
		public Tracker GetTrackerFromUser(int p_User, string p_ID)
		{
			if (!userTrackers.ContainsKey(p_User))
				return null;

			foreach (var t_Tracker in userTrackers[p_User])
			{
				if (t_Tracker.deviceID == p_ID)
					return t_Tracker;
			}

			return null;
		}

		/// <summary>
		/// Returns a tracker from the user with a specific type and index
		/// </summary>
		/// <param name="p_User"></param>
		/// <param name="p_Type"></param>
		/// <param name="p_Index"></param>
		/// <returns>Returns a tracker from the user with a specific type and index</returns>
		public Tracker GetTrackerFromUser(int p_User, VRTrackerType p_Type, int p_Index = 0)
		{
			if (!userTrackers.ContainsKey(p_User))
				return null;

			foreach (var t_Tracker in userTrackers[p_User])
			{
				if (t_Tracker.type == p_Type)
					return t_Tracker;
			}

			return null;
		}


		/// <summary>
		/// Adds a tracker to the type list, finds the first empty slot in the trackersType list and adds it there.
		/// Returns the index that the tracker is assigned to.
		/// </summary>
		/// <param name="p_Tracker"></param>
		/// <returns>The assigned index</returns>
		public int AddTrackerToTypeList(Tracker p_Tracker, bool p_TriggerUpdate = true)
		{
			for (int i = 0; i < trackersType[(int)p_Tracker.type].Count; i++)
			{
				if (trackersType[(int)p_Tracker.type][i] == null)
				{
					trackersType[(int)p_Tracker.type][i] = p_Tracker;
					p_Tracker.typeIndex = i;
					trackersChanged = true;
					return i;
				}
			}
			p_Tracker.typeIndex = trackersType[(int)p_Tracker.type].Count;
			trackersType[(int)p_Tracker.type].Add(p_Tracker);

			if (p_TriggerUpdate)
			{
				FromTypeListToUserList();
				trackersChanged = true;
			}

			return p_Tracker.typeIndex;
		}

		/// <summary>
		/// Removes a tracker at a certain index of the trackers list.
		/// This tracker is automatically removed from the trackersType list.
		/// </summary>
		/// <param name="p_Idx"></param>
		public void RemoveTracker(int p_Idx)
		{
			RemoveTrackerFromTypeList(trackers[p_Idx]);
			trackers[p_Idx] = null;
			trackersChanged = true;
		}

		/// <summary>
		/// Removes a tracker from it's current location in the trackersType list.
		/// Do this before changing it's type!
		/// </summary>
		/// <param name="p_Tracker"></param>
		/// <returns></returns>
		public bool RemoveTrackerFromTypeList(Tracker p_Tracker)
		{
			if (p_Tracker.typeIndex < 0) return false;
			trackersType[(int)p_Tracker.type][p_Tracker.typeIndex] = null;

			FromTypeListToUserList();
			trackersChanged = true;
			return true;
		}

		private void FromUserListToTypeList()
		{
			foreach (var t_TypeList in trackersType)
			{
				t_TypeList.Clear();
			}

			for (int i = 0; i < trackers.Count; i++)
			{
				trackers[i] = null;
			}

			int t_TrackerIndex = 0;

			// Add trackers in this order
			List<int> t_UserIDs = userTrackers.Keys.ToList();
			t_UserIDs.OrderBy(x => x);

			if (t_UserIDs[0] == -1)
			{
				t_UserIDs.RemoveAt(0);
				t_UserIDs.Add(-1);
			}

			foreach (var t_UserID in t_UserIDs)
			{
				foreach (var t_Tracker in userTrackers[t_UserID])
				{
					trackers[t_TrackerIndex] = t_Tracker;
					t_TrackerIndex++;

					AddTrackerToTypeList(t_Tracker, false);
				}
			}
		}

		private void FromTypeListToUserList()
		{
			userTrackers.Clear();

			foreach (var t_Type in trackersType)
			{
				for (int i = 0; i < t_Type.Count; i++)
				{
					if (t_Type[i] == null)
					{
						continue;
					}

					AddTrackerToUser(t_Type[i], i, false);
				}
			}
		}
	}
}
