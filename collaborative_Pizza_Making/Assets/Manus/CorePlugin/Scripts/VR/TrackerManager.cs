using System.Collections.Generic;
using UnityEngine;
using Manus.Utility;
using System;

namespace Manus.VR
{
	/// <summary>
	/// This component manages all the trackers and trackable objects.
	/// If it does not exist in a scene then it is automatically instantiated when an instance is requested.
	/// A ITrackingSystem should be put on the same Gameobject as the TrackerManager in order to load a specific TrackingSystem.
	/// If no tracking system is detected the TrackerManager will add the SteamVR Tracking System if SteamVR is available,
	/// otherwise it will add the first ITrackingSystem implementation it finds.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/VR/Tracker Manager")]
	public class TrackerManager : MonoBehaviour
	{
		/// <summary>
		/// Returns the current instance of the tracker manager.
		/// It will instantiate a tracker manager according to certain defines if it does not exist yet.
		/// </summary>
		public static TrackerManager instance
		{
			get
			{
				if (s_Instance != null) return s_Instance;
				if (s_IsShutdown) return null;
				s_Instance = ComponentUtil.FindOrInstantiateComponent<TrackerManager>();
				return s_Instance;
			}
		}

		private static TrackerManager s_Instance;
		static bool s_IsShutdown = false;

		/// <summary>
		/// Returns a list of all the Trackers connected.
		/// Items in the list can be NULL!
		/// Do not modify this list.
		/// </summary>
		public List<Tracker> trackers
		{
			get
			{
				return m_Data.trackers;
			}
		}

		/// <summary>
		/// Returns a list of all the Trackers connected sorted by type and index.
		/// Items in the list can be NULL!
		/// Do not modify this list.
		/// </summary>
		public List<Tracker>[] trackersType
		{
			get
			{
				return m_Data.trackersType;
			}
		}

		public Action onTrackersChanged;

		ITrackingSystem m_CurrentTrackingSystem;
		List<TrackedObject> m_TrackedObjects = new List<TrackedObject>();

		TrackerManagerInternalData m_Data = null;

		/// <summary>
		/// Initializes the tracker manager data and sets up the current tracking system
		/// </summary>
		private void Awake()
		{
			m_Data = new TrackerManagerInternalData(this);

			m_CurrentTrackingSystem = FindOrAddATrackingSystem();

			m_CurrentTrackingSystem.Initialize(m_Data);

			OnUpdate();
		}

		/// <summary>
		/// Finds the tracking system on the object, otherwise it will add a default one.
		/// Default is SteamVR if SteamVR is enabled.
		/// </summary>
		/// <returns>Tracking System</returns>
		ITrackingSystem FindOrAddATrackingSystem()
		{
			ITrackingSystem t_TS = GetComponent<ITrackingSystem>();
			if (t_TS != null) return t_TS;
#if MANUS_STEAMVR
			return gameObject.AddComponent<TrackingSystemSteamVR>() as ITrackingSystem;
#else
			var t_TSType = typeof(ITrackingSystem);
			var t_MBType = typeof(MonoBehaviour);
			foreach (var t_Assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var t_Type in t_Assembly.GetTypes())
				{
					if (t_TSType.IsAssignableFrom(t_Type) && t_MBType.IsAssignableFrom(t_Type))
					{
						return gameObject.AddComponent(t_Type) as ITrackingSystem;
					}
				}
			}
			return null;
#endif
		}

		/// <summary>
		/// Calls the OnEnabled function in the current tracking system.
		/// </summary>
		private void OnEnable()
		{
			m_CurrentTrackingSystem.OnEnabled(m_Data);
		}

		/// <summary>
		/// Calls the OnDisabled function in the current tracking system.
		/// </summary>
		private void OnDisable()
		{
			m_CurrentTrackingSystem.OnDisabled(m_Data);
		}

		/// <summary>
		/// Disables the creation of new Tracker Manager intances.
		/// </summary>
		private void OnApplicationQuit()
		{
			s_IsShutdown = true;
		}

		/// <summary>
		/// Updates the current tracking system's poses and handles the object trackers
		/// </summary>
		private void Update()
		{
			OnUpdate();
		}

		/// <summary>
		/// This function updates the current tracking system and handles changes in trackers
		/// </summary>
		void OnUpdate()
		{
			m_CurrentTrackingSystem.UpdatePoses(m_Data);
			if (m_Data.trackersChanged)
			{
				onTrackersChanged?.Invoke();
				UpdateTrackedObjectTrackers();
			}
		}

		/// <summary>
		/// This function is used to add tracked objects which need to be updated by the TrackerManager.
		/// It only adds the tracked object if it's not being tracked.
		/// </summary>
		/// <param name="p_Object">The object that should be tracked</param>
		public void AddTrackedObject(TrackedObject p_Object)
		{
			if (m_Data != null) m_Data.trackersChanged = true;
			if (!m_TrackedObjects.Contains(p_Object)) m_TrackedObjects.Add(p_Object);
		}

		/// <summary>
		/// This function is used to remove tracked objects which no longer need to be updated by the TrackerManager.
		/// </summary>
		/// <param name="p_Object">The object that should no longer be tracked</param>
		public void RemoveTrackedObject(TrackedObject p_Object)
		{
			if (m_Data != null) m_Data.trackersChanged = true;
			m_TrackedObjects.Remove(p_Object);
		}

		/// <summary>
		/// Get tracker from user
		/// </summary>
		/// <param name="p_UserIndex">user index</param>
		/// <param name="p_TrackerType">type of tracker to get</param>
		/// <param name="p_Index">index of the tracker assigned to that user, default is the first</param>
		/// <returns>tracker for user, null when not existent</returns>
		public Tracker GetTrackerFromUser(int p_UserIndex, VRTrackerType p_TrackerType, int p_Index = 0)
		{
			return m_Data.GetTrackerFromUser(p_UserIndex, p_TrackerType, p_Index);
		}

		/// <summary>
		/// This function updates the tracked object's trackers.
		/// </summary>
		public void UpdateTrackedObjectTrackers()
		{
			m_Data.trackersChanged = false;

			for (int i = 0; i < m_TrackedObjects.Count; i++)
			{
				m_TrackedObjects[i].SetTracker(m_Data.GetTrackerFromUser(m_TrackedObjects[i].user, m_TrackedObjects[i].type, m_TrackedObjects[i].index));
			}
		}
	}
}
