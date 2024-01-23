using UnityEngine;
using Manus.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Manus.VR
{
	/// <summary>
	/// This component allows an object to be moved according to a tracker position and orientation.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/VR/Tracked Object")]
	public class TrackedObject : MonoBehaviour
	{
		/// <summary>
		/// Sets active to false when tracker does not exist
		/// </summary>
		public bool autoToggle = true;
		public Utility.VRTrackerType type;
		public int user;
		public int index;
		public Tracker tracker;

		/// <summary>
		/// Removes itself from the tracked object list
		/// </summary>
		private void OnDestroy()
		{
			if (TrackerManager.instance != null) TrackerManager.instance.RemoveTrackedObject(this);
		}

		/// <summary>
		/// Add the tracked object to the tracker manager
		/// </summary>
		private void OnEnable()
		{
			TrackerManager.instance.AddTrackedObject(this);
		}

		/// <summary>
		/// Updates the position and rotation if a tracker is available.
		/// </summary>
		private void Update()
		{
			if (tracker == null) return;
			transform.localPosition = tracker.position;
			transform.localRotation = tracker.rotation;
		}

		/// <summary>
		/// Sets the tracker to use, NULL ensures the gameobject gets disabled.
		/// </summary>
		/// <param name="p_Tracker"></param>
		public void SetTracker(Tracker p_Tracker)
		{
			tracker = p_Tracker;

			if (autoToggle)
				gameObject.SetActive(tracker != null);
		}
	}


}

#if UNITY_EDITOR

[CustomEditor(typeof(Manus.VR.TrackedObject))]
public class TrackedObjectEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Manus.VR.TrackedObject t_Object = target as Manus.VR.TrackedObject;

		t_Object.autoToggle = EditorGUILayout.Toggle(new GUIContent("Auto Toggle"), t_Object.autoToggle);
		t_Object.type = (VRTrackerType)EditorGUILayout.EnumPopup(new GUIContent("Tracker Type"), t_Object.type);
		t_Object.user = EditorGUILayout.IntField(new GUIContent("User"), t_Object.user);

		if (t_Object.user == -1 ||
			t_Object.type == VRTrackerType.Other ||
			t_Object.type == VRTrackerType.Camera ||
			t_Object.type == VRTrackerType.Controller ||
			t_Object.type == VRTrackerType.LightHouse)
			t_Object.index = EditorGUILayout.IntField(new GUIContent("Index"), t_Object.index);
	}
}

#endif