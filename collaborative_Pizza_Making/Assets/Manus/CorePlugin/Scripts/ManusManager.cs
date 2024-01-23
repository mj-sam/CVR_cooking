using UnityEngine;

namespace Manus
{
	/// <summary>
	/// This is the central location for all communication between certain aspects of the Manus Plugin.
	/// It contains a link to the communication hub which is used to connect to Hermes.
	/// There is also a link to the general settings of the plugin.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Manus Manager")]
	public class ManusManager : MonoBehaviour
	{
		/// <summary>
		/// Finds or instantiates and returns the Manus Manager.
		/// Can return NULL if shutting down!
		/// </summary>
		public static ManusManager instance
		{
			get
			{
				if (s_Instance != null) return s_Instance;
				if (s_IsShutdown) return null;
				s_Instance = Utility.ComponentUtil.FindOrInstantiateComponent<ManusManager>();
				return s_Instance;
			}
		}

		private static ManusManager s_Instance;
		static bool s_IsShutdown = false;

		/// <summary>
		/// Finds or instantiates and returns the Communication Hub.
		/// Can return NULL if shutting down!
		/// </summary>
		public Hermes.CommunicationHub communicationHub
		{
			get
			{
				if (m_CommunicationHub != null) return m_CommunicationHub;
				if (s_IsShutdown) return null;
				m_CommunicationHub = Utility.ComponentUtil.FindOrInstantiateComponent<Hermes.CommunicationHub>();
				return m_CommunicationHub;
			}
		}

		Hermes.CommunicationHub m_CommunicationHub = null;

		public ManusSettings settings
		{
			get
			{
				if (m_Settings != null) return m_Settings;
				ManusSettings t_Settings = Resources.Load<ManusSettings>("DefaultManusSettings");
				if (t_Settings != null)
				{
					m_Settings = t_Settings;
					return m_Settings;
				}
				ManusSettings[] t_SettingsList = Resources.LoadAll<ManusSettings>("");
				if (t_SettingsList.Length > 0) return t_SettingsList[0];
				return m_Settings;
			}
			set
			{
				m_Settings = value;
			}
		}

		[SerializeField]
		ManusSettings m_Settings = null;

		protected virtual void Awake()
		{
			s_Instance = this;
			if (settings == null) Debug.LogError("No Manus Settings found!");
			if (communicationHub == null) Debug.LogError("Communication hub could not be set up!");
		}

		void OnEnable()
		{
		}

		void OnDisable()
		{
		}

		private void OnApplicationQuit()
		{
			s_IsShutdown = true;
		}

		private void Update()
		{
		}

		/// <summary>
		/// Has the CommunicationHub been initialized?
		/// </summary>
		/// <returns></returns>
		public bool HasCommunicationHub()
		{
			return m_CommunicationHub != null;
		}

	}
}

#if UNITY_EDITOR
namespace Manus.Editor.Core
{
	using UnityEditor;

	[CustomEditor(typeof(Manus.ManusManager))]
	public class ManusManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Settings"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
