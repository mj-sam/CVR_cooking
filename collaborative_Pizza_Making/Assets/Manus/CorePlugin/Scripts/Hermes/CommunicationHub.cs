//#define PROFILE

using System;
using System.Collections.Generic;
using UnityEngine;
using HProt = Hermes.Protocol;
using HTools = Hermes.Tools;
using HClient = Hermes.ClientCore;
using Manus.Utility;
using NetMQ;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Hermes.Tools;
using System.Linq;

namespace Manus.Hermes
{
	/// <summary>
	/// This class is responsible for the communication between Hermes and the plugin.
	/// This component should not be added to the scene manually.
	/// Most of these functions are not to be accessed directly, the Hand data is automatically thrown into Hands that are initialized and registered.
	/// </summary>
	[DisallowMultipleComponent]
	public class CommunicationHub : MonoBehaviour
	{
		private static readonly log4net.ILog m_Log = log4net.LogManager.GetLogger("HermesSdkSampleClient");

		public HTools.CareTaker careTaker { get { return m_CareTaker; } }

		public Action<Polygon.PolygonData> polygonUpdate;

		public Action<HProt.TrackerData> trackerUpdate;

		public Action<HProt.Debugging.DebuggingData> newDebugData;

		public Action<HTools.CareTaker> onCareTakerCreated;

		public bool sendHapticsData = true;

		//TBI
		//public Hand prefabRightHand;
		//public Hand prefabLeftHand;

		Dictionary<uint, Glove.Data> m_Gloves = new Dictionary<uint, Glove.Data>();

		List<Glove.Data>[] m_GlovesType = new List<Glove.Data>[(int)HandType.Invalid];

		List<Hand.Hand> m_Hands = new List<Hand.Hand>();

		List<uint> m_HapticsDongles = new List<uint>();

		public Action<HProt.Devices> newDeviceData;

		/// <summary>
		/// Low-frequency device information, such as battery level and signal strength.
		/// </summary>
		public HProt.Hardware.DeviceLandscape deviceLandscape { get; private set; }

		private HTools.CareTaker m_CareTaker = null;
		private HClient.Hive m_Hive = null;


		private bool m_HandsChanged = true;

		static bool s_Forced = false;
		static bool s_IsShutdown = false;
		static bool s_AutoReconnect = true;

		public List<VR.Tracker> trackers = new List<VR.Tracker>();

		private Thread m_TrackerSenderThread = null;
		private Queue<HProt.Tracker[]> m_TrackersToSend = null;

		private string m_ApplicationString;

#if PROFILE
		DateTime m_GloveTime = DateTime.UtcNow;
		public class GloveProfileData
		{
			public uint id = 0;
			public uint count = 0;
			public double time = 0.0;
			public GloveProfileData(uint _ID)
			{
				id = _ID;
			}
		}

		Dictionary<uint, GloveProfileData> m_GlovePerformance = new Dictionary<uint, GloveProfileData>();
#endif

		void OnEnable()
		{
			if (s_Forced == false)
			{
				s_Forced = true;
				//log.Info("AsyncIO.ForceDotNet.Force()");
				AsyncIO.ForceDotNet.Force();
			}

			m_ApplicationString = $"{Application.productName} {Application.version}";

			Connect();
		}

		void OnDisable()
		{
			if (s_IsShutdown)
				return;

			s_IsShutdown = true;

			Disconnect();

			try
			{
				NetMQConfig.Cleanup(false);
				AsyncIO.ForceDotNet.Unforce();
				s_Forced = false;
			}
			catch (Exception _Ex)
			{
				Debug.LogWarning(_Ex.ToString());
			}
		}

		void Connect()
		{
			if (m_CareTaker != null)
				return;

			deviceLandscape = null;
			m_Gloves = new Dictionary<uint, Glove.Data>();
			m_GlovesType = new List<Glove.Data>[(int)HandType.Invalid];
			for (int i = 0; i < m_GlovesType.Length; i++)
			{
				m_GlovesType[i] = new List<Glove.Data>();
			}

			m_Hive?.Stop();

			try
			{
				m_CareTaker = m_CareTaker ?? new HTools.CareTaker();
				var t_Route = m_CareTaker.GetPresetRoute();

				int t_HivePort = (int)HProt.DefaultPorts.HivePort;
				int t_Port = m_CareTaker.GetHivePort();
				if (t_Port > 0) t_HivePort = t_Port;

				if (t_Route == null)
					m_Hive = HClient.Hive.FindLocalCoordinator("Hermes.Unity.Test", t_HivePort, (p_Node) => { InitConnection(p_Node.Coordinator); });
				else
					InitConnection(t_Route);

				Debug.Log("Searching for Hermes...");
			}
			catch (Exception _Ex)
			{
				Debug.LogError(_Ex.Message);
			}
		}

		void InitConnection(HProt.ServiceRoute p_Route)
		{
			Debug.Log("Found a Hermes...");
			if (m_CareTaker.connected)
				return;

			m_CareTaker.onConnectionChanged += OnConnectionChanged;

			m_CareTaker.AddDeviceDataListener(OnDeviceData);
			m_CareTaker.AddPolygonDataListener(OnPolygonData);
			m_CareTaker.AddTrackingDataListener(OnTrackingData);
			m_CareTaker.AddDebuggingDataListener(OnDebuggingData);
			m_CareTaker.AddDeviceLandscapeDataListener(OnDeviceLandscape);
			onCareTakerCreated?.Invoke(m_CareTaker);


			if (!m_CareTaker.Connect(p_Route, "Manus Unity Plugin", m_ApplicationString, OnFilterSetup))
				throw new Exception("No reply");

			StartTrackerSenderThread();

			Debug.Log("Connected to Hermes...");

			m_HandsChanged = true;
			m_Hive?.Stop();
			m_Hive = null;
		}
		async void Disconnect()
		{
			KillTrackerSenderThread();

			await Task.Run(() =>
			{
				m_Hive?.Stop();
				m_Hive = null;
				m_CareTaker?.Dispose();
				m_CareTaker = null;
			});

		}

		async void OnConnectionChanged(bool p_Connected)
		{
			if (s_IsShutdown) return;
			if (s_AutoReconnect && !p_Connected)
			{
				Disconnect();

				//Wait until succesful disconnect
				while (m_CareTaker != null)
					await Task.Delay(100);

				Debug.Log("Attempting to reconnect to Hermes...");
				Connect();
			}
		}

		private void OnFilterSetup(HProt.Pipeline p_Pipeline)
		{
			var t_Filterlist = m_CareTaker.Hermes.GetFilterInventory(new HProt.Void());
			if (t_Filterlist.Filters?.Count > 0)
			{
				m_Log.Info($"{t_Filterlist.Filters.Count} available filters:");
				foreach (var filter in t_Filterlist.Filters)
					m_Log.Info($"{filter.Name}: '{filter.Description}'");

				p_Pipeline.Filters.Clear();


				var t_MeshNodeConfig = new HProt.MeshNodeConfig { UpDirection = HProt.coor_axis_t.CoorAxisYpos, ForwardDirection = HProt.coor_axis_t.CoorAxisXpos, RightDirection = HProt.coor_axis_t.CoorAxisZneg };

				//	For this mesh, Right thumb and right finger coordinate systems are equal to right wrist.
				var meshConfig = new HProt.MeshConfig
				{
					RightWrist = t_MeshNodeConfig,
					RightThumb = t_MeshNodeConfig,
					RightFinger = t_MeshNodeConfig,
					LeftWrist = t_MeshNodeConfig,
					LeftThumb = t_MeshNodeConfig,
					LeftFinger = t_MeshNodeConfig,

					World = new HProt.MeshNodeConfig
					{
						UpDirection = HProt.coor_axis_t.CoorAxisYpos,
						ForwardDirection = HProt.coor_axis_t.CoorAxisZpos,
						RightDirection = HProt.coor_axis_t.CoorAxisXpos
					},

					NegateAxisX = true,
					NegateAxisY = false,
					NegateAxisZ = false
				};

				p_Pipeline.Filters.Add(new HProt.Filter { Name = "DataStandardization" });
				p_Pipeline.Filters.Add(new HProt.Filter { Name = @"CreepCompensation" });
				p_Pipeline.Filters.Add(new HProt.Filter { Name = "NormalizedHand", ParameterSet = new HProt.ParameterSet { Parameters = { new HProt.Parameter { Name = "generateHandLocalData", Boolean = true } } } });
				p_Pipeline.Filters.Add(new HProt.Filter { Name = @"HandModel" });
				//_Pipeline.Filters.Add(new HProt.Filter { Name = @"ThumbLimit" });
				//_Pipeline.Filters.Add(new HProt.Filter { Name = "Supersample", ParameterSet = new HProt.ParameterSet { Parameters = { new HProt.Parameter { Name = "Rate", Number = 60.0 } } } });
				//_Pipeline.Filters.Add(new HProt.Filter { Name = "FingerSpreading" });
				p_Pipeline.Filters.Add(new HProt.Filter { Name = @"BasisConversion", ParameterSet = new HProt.ParameterSet { Parameters = { new HProt.Parameter { Name = "serializedMeshConfig", Bytes = meshConfig.ToByteString() } } } });
				//	_Pipeline.Filters.Add(new HProt.Filter { Name = "QuaternionCompression" });
			}
		}

		private void OnDeviceData(HProt.Devices p_Devices)
		{
			newDeviceData?.Invoke(p_Devices);

			foreach (var t_Device in p_Devices.Gloves)
			{
#if PROFILE
				GloveProfileData t_Prof;
				if (!m_GlovePerformance.TryGetValue(t_Device.Info.DeviceID, out t_Prof))
				{
					t_Prof = new GloveProfileData(t_Device.Info.DeviceID);
					m_GlovePerformance.Add(t_Device.Info.DeviceID, t_Prof);
				}
				t_Prof.count++;
				t_Prof.time += DateTime.UtcNow.Subtract(t_Device.ProfilerData[0].Timestamp.ToDateTime()).TotalMilliseconds;
#endif
				Glove.Data t_Data;
				if (!m_Gloves.TryGetValue(t_Device.Info.DeviceID, out t_Data))
				{
					t_Data = AddGlove(t_Device.Info.DeviceID, t_Device.Info.HandType, t_Device.Info.UserIndex.ToArray());
				}

				if (t_Data.userIndex.Length != t_Device.Info.UserIndex.Count)
				{
					t_Data.userIndex = t_Device.Info.UserIndex.ToArray();
					m_HandsChanged = true;
				}
				else
				{
					for (int i = 0; i < t_Data.userIndex.Length; i++)
					{
						if (t_Data.userIndex[i] != t_Device.Info.UserIndex[i])
						{
							t_Data.userIndex = t_Device.Info.UserIndex.ToArray();
							m_HandsChanged = true;
							break;
						}
					}
				}

				t_Data.ApplyData(t_Device);
			}

#if PROFILE
			double t_TimeSpent = DateTime.UtcNow.Subtract(m_GloveTime).TotalSeconds;
			if (t_TimeSpent > 10.0)
			{
				string t_Log = "Sample Time (" + t_TimeSpent + ")";
				foreach (var t_Glv in m_GlovePerformance)
				{
					t_Log += "\n Glove " + t_Glv.Key.ToString()
						+ "\nCount:" + t_Glv.Value.count.ToString()
						+ "\nTotal Time:" + t_Glv.Value.time + "\nAvg: "
						+ (t_Glv.Value.time / t_Glv.Value.count);
				}
				m_GlovePerformance.Clear();
				Debug.Log(t_Log);
				m_GloveTime = DateTime.UtcNow;
			}
#endif
		}

		private void OnPolygonData(HProt.Polygon.Data p_Poly)
		{
			polygonUpdate?.Invoke((Polygon.PolygonData)p_Poly);
		}

		private void OnTrackingData(HProt.TrackerData p_Trackers)
		{
			foreach (var t_ProtoTracker in p_Trackers.Trackers)
			{
				HandType t_TrackerHandType = HandType.Invalid;

				if (t_ProtoTracker.Type == HProt.TrackerType.LeftHand)
					t_TrackerHandType = HandType.LeftHand;
				else if (t_ProtoTracker.Type == HProt.TrackerType.RightHand)
					t_TrackerHandType = HandType.RightHand;
				else
					continue;

				foreach (var t_Hand in m_Hands)
				{
					if (t_Hand.userIndex != t_ProtoTracker.UserIndex || t_Hand.type != t_TrackerHandType)
						continue;

					t_Hand.trackerPosition = t_ProtoTracker.Position.ToUnity();
					t_Hand.trackerRotation = t_ProtoTracker.Rotation.ToUnity();
				}
			}

			trackerUpdate?.Invoke(p_Trackers);
		}

		private void OnDebuggingData(HProt.Debugging.DebuggingData p_DebuggingData)
		{
			newDebugData?.Invoke(p_DebuggingData);
		}

		private void OnDeviceLandscape(HProt.Hardware.DeviceLandscape p_DeviceLandscape)
		{
			deviceLandscape = p_DeviceLandscape;

			//m_HapticsDongles.Clear();
			foreach (var t_Forest in deviceLandscape.Forest.Values)
			{
				foreach (var t_Tree in t_Forest.Trees.Values)
				{
					foreach (var t_Leaf in t_Tree.Leafs.Values)
					{
						if (t_Leaf.HapticsModule != null)
						{
							if (!m_HapticsDongles.Contains(t_Tree.Id))
							{
								m_HapticsDongles.Add(t_Tree.Id);
							}

							continue;
						}
						if (t_Leaf.Family == HProt.Embedded.DeviceFamily.SensorDongle
							|| t_Leaf.Family == HProt.Embedded.DeviceFamily.SensorLeftGlove
							|| t_Leaf.Family == HProt.Embedded.DeviceFamily.SensorRightGlove)
						{
							Glove.Data t_Data;
							if (!m_Gloves.TryGetValue(t_Leaf.Id, out t_Data))
							{
								continue;
							}
							t_Data.hapticsID = t_Tree.Id;
						}
					}
				}

				// Remove all non existing gloves
				if (t_Forest.ForestType == HProt.Hardware.DeviceForest.Types.ForestType.DevicesForest)
				{
					var t_GloveIDs = new List<uint>();

					foreach (var t_Tree in t_Forest.Trees.Values)
					{
						foreach (var t_Leaf in t_Tree.Leafs.Values)
						{
							t_GloveIDs.Add(t_Leaf.Id);
						}
					}

					foreach (var t_GloveID in m_Gloves.Keys)
					{
						if (!t_GloveIDs.Contains(t_GloveID))
							m_Gloves.Remove(t_GloveID);
					}
				}
			}
		}

		private Glove.Data AddGlove(uint p_ID, HProt.HandType p_HandType, int[] p_UserIndex)
		{
			Glove.Data t_Data = new Glove.Data(m_CareTaker, p_ID, p_HandType, p_UserIndex);
			m_Gloves.Add(p_ID, t_Data);
			m_HandsChanged = true;

			m_GlovesType[(int)t_Data.handType].Add(t_Data);
			return t_Data;
		}

		/// <summary>
		/// This function adds a Hand to the list of hands, this allows the HandData to be updated automatically.
		/// </summary>
		/// <param name="p_Hand"></param>
		public void RegisterHand(Hand.Hand p_Hand)
		{
			m_HandsChanged = true;
			if (!m_Hands.Contains(p_Hand)) m_Hands.Add(p_Hand);
		}

		/// <summary>
		/// This function removes a Hand from the list of hands, this stops the HandData from being updated automatically.
		/// </summary>
		/// <param name="p_Hand"></param>
		public void UnregisterHand(Hand.Hand p_Hand)
		{
			m_HandsChanged = true;
			m_Hands.Remove(p_Hand);
		}

		/// <summary>
		/// Updates the hands if required.
		/// </summary>
		public void Update()
		{
			if (!m_HandsChanged) return;
			m_HandsChanged = false;

			for (int i = 0; i < m_Hands.Count; i++)
			{
				m_Hands[i].data = null;

				foreach (var t_Glove in m_Gloves.Values.ToArray())
				{
					if (m_Hands[i].type == t_Glove.handType && t_Glove.userIndex.Contains(m_Hands[i].userIndex))
					{
						m_Hands[i].data = t_Glove;
						break;
					}
				}
			}
		}

		private void FixedUpdate()
		{
			if (m_CareTaker == null || !m_CareTaker.connected) return;
			if (sendHapticsData == false)
				return;

			int[] t_OldHapticsDongleIdx = { 0, 0 };
			foreach (var t_Glove in m_Gloves)
			{
				var t_Dat = t_Glove.Value;
				if (t_Dat == null) continue;
				var t_Handedness = t_Dat.handType == HandType.LeftHand ? HProt.HandType.Left : HProt.HandType.Right;
				if (t_Dat.hapticsID == 0)
				{
					int t_HDI = t_Dat.handType == HandType.LeftHand ? 0 : 1;
					if (m_HapticsDongles.Count > t_OldHapticsDongleIdx[t_HDI])
					{
						m_CareTaker.SendHapticData(m_HapticsDongles[t_OldHapticsDongleIdx[t_HDI]], t_Handedness, t_Dat.GetHaptics());
					}
					t_OldHapticsDongleIdx[t_HDI]++;
				}
				else
				{
					m_CareTaker.SendHapticData(t_Dat.hapticsID, t_Handedness, t_Dat.GetHaptics());
				}
			}
		}

		/// <summary>
		/// Sends all the tracker data from the tracker manager to hermes.
		/// </summary>
		public void GiveAllTrackerData()
		{
			if (m_CareTaker == null || !m_CareTaker.connected) return;
			HProt.TrackerData t_Data = new HProt.TrackerData();

			var t_Trackers = VR.TrackerManager.instance.trackersType;
			foreach (var t_TList in t_Trackers)
			{
				foreach (var t_T in t_TList)
				{
					t_Data.Trackers.Add(t_T.ToProto());
				}
			}

			m_CareTaker.Hermes.UpdateTrackersAsync(t_Data);
		}


		private void StartTrackerSenderThread()
		{
			if (m_TrackerSenderThread != null)
				KillTrackerSenderThread();

			m_TrackerSenderThread = new Thread(TrackerDataSenderThread);
			m_TrackersToSend = new Queue<HProt.Tracker[]>();
			m_TrackerSenderThread.Start();
		}
		private void KillTrackerSenderThread()
		{
			m_TrackerSenderThread?.Abort();
			m_TrackersToSend = new Queue<HProt.Tracker[]>();
		}
		/// <summary>
		/// Sends specific tracker data to hermes.
		/// </summary>
		/// <param name="p_Trackers"></param>
		public void GiveTrackerData(HProt.Tracker[] p_Trackers)
		{
			if (m_CareTaker == null || !m_CareTaker.connected || m_TrackersToSend == null) return;

			m_TrackersToSend.Enqueue(p_Trackers);
		}

		private void TrackerDataSenderThread()
		{
			HProt.Tracker[] t_Trackers;
			HProt.TrackerData t_Data;
			while (true)
			{
				while (m_TrackersToSend.Count > 0)
				{
					t_Trackers = m_TrackersToSend.Dequeue();
					t_Data = new HProt.TrackerData();
					foreach (var t_Tracker in t_Trackers)
					{
						t_Data.Trackers.Add(t_Tracker);
					}

					if (m_CareTaker == null || m_CareTaker.connected == false)
						return;

					m_CareTaker.Hermes.UpdateTrackers(t_Data);
				}
				Thread.Sleep(10);
			}

		}

		public uint GetHapticDongeIDForHand(int p_Idx)
		{
			if (m_HapticsDongles.Count > p_Idx)
			{
				return m_HapticsDongles[p_Idx];
			}

			return 0;
		}
	}
}
