#if MANUS_STEAMVR
using UnityEngine;
using Manus.Utility;

namespace Manus.VR
{
	/// <summary>
	/// This component manages all the trackers for SteamVR/OpenVR.
	/// </summary>
 	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/VR/Tracking System (Steam VR)")]
	public class TrackingSystemSteamVR : MonoBehaviour, ITrackingSystem
	{
		Valve.VR.SteamVR_Events.Action m_InputFocusAction, m_TrackedDeviceRoleChangedAction;

		const int s_MaxTrackedDeviceCount = (int)Valve.VR.OpenVR.k_unMaxTrackedDeviceCount;

		Valve.VR.TrackedDevicePose_t[] m_Poses = new Valve.VR.TrackedDevicePose_t[s_MaxTrackedDeviceCount];
		Valve.VR.TrackedDevicePose_t[] m_GamePoses = new Valve.VR.TrackedDevicePose_t[0];

		TrackerManagerInternalData m_Data = null;

		/// <summary>
		/// This is called when the Tracker Manager initializes the Tracking System.
		/// SteamVR has a max amount of tracked devices, these are already set here for optimization purposes.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void Initialize(TrackerManagerInternalData p_Data)
		{
			m_Data = p_Data;

			p_Data.trackers.Clear();
			p_Data.trackers.Resize(s_MaxTrackedDeviceCount);

			m_InputFocusAction = Valve.VR.SteamVR_Events.InputFocusAction(OnInputFocus);
			m_TrackedDeviceRoleChangedAction = Valve.VR.SteamVR_Events.SystemAction(Valve.VR.EVREventType.VREvent_TrackedDeviceRoleChanged, OnTrackedDeviceRoleChanged);
		}

		/// <summary>
		/// This is called when the Tracker Manager gets enabled or enables this Tracking System.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void OnEnabled(TrackerManagerInternalData p_Data)
		{
			m_InputFocusAction.enabled = true;
			m_TrackedDeviceRoleChangedAction.enabled = true;
		}

		/// <summary>
		/// This is called when the Tracker Manager gets disabled or disables this Tracking System.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void OnDisabled(TrackerManagerInternalData p_Data)
		{
			m_InputFocusAction.enabled = false;
			m_TrackedDeviceRoleChangedAction.enabled = false;
		}

		private void OnInputFocus(bool p_HasFocus)
		{
		}

		/// <summary>
		/// This is called when the Tracker Manager updates the trackers.
		/// This happens every Update.
		/// The trackersChanged boolean is expected to be set in the _Data input if the tracker types or amounts are modified.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		public void UpdatePoses(TrackerManagerInternalData p_Data)
		{
			var compositor = Valve.VR.OpenVR.Compositor;
			if (compositor != null)
			{
				compositor.GetLastPoses(m_Poses, m_GamePoses);
				for (int i = 0; i < m_Poses.Length; i++)
				{
					if (!m_Poses[i].bDeviceIsConnected)
					{
						if (p_Data.trackers[i] != null)
						{
							p_Data.RemoveTracker(i);
						}
						continue;
					}
					if (p_Data.trackers[i] == null)
					{
						if (!AddSteamVRTracker(p_Data, i)) continue;
					}
					//if(_Data.trackers[i].type!=)
					var t_PR = new SteamVR_Utils.RigidTransform(m_Poses[i].mDeviceToAbsoluteTracking);
					p_Data.trackers[i].position = t_PR.pos;
					p_Data.trackers[i].rotation = t_PR.rot;
				}
			}
		}

		/// <summary>
		/// This is called when a new tracker needs to be added.
		/// </summary>
		/// <param name="p_Data">The Tracker Data</param>
		/// <param name="p_Index">The device index</param>
		bool AddSteamVRTracker(TrackerManagerInternalData p_Data, int p_Index)
		{
			uint t_Index = (uint)p_Index;
			var t_System = Valve.VR.OpenVR.System;
			if (t_System == null) return false;

			var t_DeviceClass = t_System.GetTrackedDeviceClass(t_Index);
			switch (t_DeviceClass)
			{
				case Valve.VR.ETrackedDeviceClass.Controller:
					{
						p_Data.trackers[p_Index] = new Tracker(t_Index, VRTrackerType.Controller, GetDeviceID(t_Index));
						p_Data.AddTrackerToTypeList(p_Data.trackers[p_Index]);
						return true;
					}
				case Valve.VR.ETrackedDeviceClass.HMD:
					{
						p_Data.trackers[p_Index] = new Tracker(t_Index, VRTrackerType.Head, GetDeviceID(t_Index));
						p_Data.trackers[p_Index].isHMD = true;
						p_Data.AddTrackerToTypeList(p_Data.trackers[p_Index]);
						return true;
					}
				case Valve.VR.ETrackedDeviceClass.GenericTracker:
					{
						var t_S = new System.Text.StringBuilder(100);
						var t_Error = Valve.VR.ETrackedPropertyError.TrackedProp_Success;
						t_System.GetStringTrackedDeviceProperty(t_Index, Valve.VR.ETrackedDeviceProperty.Prop_ControllerType_String, t_S, 100, ref t_Error);

						VRTrackerType t_Type = VRTrackerType.Other;
						switch (t_S.ToString())
						{
							case "vive_tracker_handed":
								int t_Handed = t_System.GetInt32TrackedDeviceProperty(t_Index, Valve.VR.ETrackedDeviceProperty.Prop_ControllerRoleHint_Int32, ref t_Error);
								if (t_Handed == 1) t_Type = VRTrackerType.LeftHand;
								if (t_Handed == 2) t_Type = VRTrackerType.RightHand;
								break;
							case "vive_tracker_waist":
								t_Type = VRTrackerType.Waist;
								break;
							case "vive_tracker_left_foot":
								t_Type = VRTrackerType.LeftFoot;
								break;
							case "vive_tracker_right_foot":
								t_Type = VRTrackerType.RightFoot;
								break;
							case "vive_tracker_left_shoulder":
								t_Type = VRTrackerType.LeftUpperArm;
								break;
							case "vive_tracker_right_shoulder":
								t_Type = VRTrackerType.RightUpperArm;
								break;
							case "vive_tracker_right_camera":
								t_Type = VRTrackerType.Camera;
								break;
						}

						p_Data.trackers[p_Index] = new Tracker(t_Index, t_Type, GetDeviceID(t_Index));
						p_Data.AddTrackerToTypeList(p_Data.trackers[p_Index]);
						return true;
					}
				case Valve.VR.ETrackedDeviceClass.TrackingReference:
					{
						var t_S = new System.Text.StringBuilder(100);
						var t_Error = Valve.VR.ETrackedPropertyError.TrackedProp_Success;
						t_System.GetStringTrackedDeviceProperty(t_Index, Valve.VR.ETrackedDeviceProperty.Prop_ControllerType_String, t_S, 100, ref t_Error);

						p_Data.trackers[p_Index] = new Tracker(t_Index, VRTrackerType.LightHouse, GetDeviceID(t_Index));
						p_Data.AddTrackerToTypeList(p_Data.trackers[p_Index]);

						return true;
					}
				default:
					return false;
			}
		}

		private string GetDeviceID(uint p_Index)
		{
			Valve.VR.ETrackedPropertyError t_Error = new Valve.VR.ETrackedPropertyError();
			System.Text.StringBuilder t_SB = new System.Text.StringBuilder();
			Valve.VR.OpenVR.System.GetStringTrackedDeviceProperty(p_Index, Valve.VR.ETrackedDeviceProperty.Prop_SerialNumber_String, t_SB, Valve.VR.OpenVR.k_unMaxPropertyStringSize, ref t_Error);
			return t_SB.ToString();
		}

		/// <summary>
		/// This is called by SteamVR when a tracker changes it's role/type
		/// </summary>
		/// <param name="p_VREvent">Event Data</param>
		private void OnTrackedDeviceRoleChanged(Valve.VR.VREvent_t p_VREvent)
		{
			var t_System = Valve.VR.OpenVR.System;
			if (t_System == null) return;
			for (int i = 0; i < m_Data.trackers.Count; i++)
			{
				if (m_Data.trackers[i] == null) continue;
				uint t_Index = (uint)i;

				var t_DeviceClass = t_System.GetTrackedDeviceClass(t_Index);
				if (t_DeviceClass != Valve.VR.ETrackedDeviceClass.GenericTracker)
				{
					if (m_Data.trackers[i] != null)
					{
						m_Data.RemoveTracker(i);
					}
					continue;
				}

				var t_S = new System.Text.StringBuilder(100);
				var t_Error = Valve.VR.ETrackedPropertyError.TrackedProp_Success;
				t_System.GetStringTrackedDeviceProperty(t_Index, Valve.VR.ETrackedDeviceProperty.Prop_ControllerType_String, t_S, 100, ref t_Error);

				VRTrackerType t_Type = VRTrackerType.Other;
				switch (t_S.ToString())
				{
					case "vive_tracker_handed":
						int t_Handed = t_System.GetInt32TrackedDeviceProperty(t_Index, Valve.VR.ETrackedDeviceProperty.Prop_ControllerRoleHint_Int32, ref t_Error);
						if (t_Handed == 1) t_Type = VRTrackerType.LeftHand;
						if (t_Handed == 2) t_Type = VRTrackerType.RightHand;
						break;
					case "vive_tracker_waist":
						t_Type = VRTrackerType.Waist;
						break;
					case "vive_tracker_left_foot":
						t_Type = VRTrackerType.LeftFoot;
						break;
					case "vive_tracker_right_foot":
						t_Type = VRTrackerType.RightFoot;
						break;
				}

				if (m_Data.trackers[i].type != t_Type)
				{
					m_Data.RemoveTrackerFromTypeList(m_Data.trackers[i]);
					m_Data.AddTrackerToTypeList(m_Data.trackers[i]);
				}
			}
		}
	}
}
#endif
