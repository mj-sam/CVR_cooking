using UnityEngine;

namespace Manus.Utility
{
	/// <summary>
	/// An attribute that allows unity to show an uneditable property
	/// </summary>
	public class ReadOnlyAttribute : PropertyAttribute
	{

	}

	/// <summary>
	/// An attribute that allows unity to show an uneditable property
	/// </summary>
#if UNITY_EDITOR
	[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
	{
		public override float GetPropertyHeight(UnityEditor.SerializedProperty p_Property, GUIContent p_Label)
		{
			return UnityEditor.EditorGUI.GetPropertyHeight(p_Property, p_Label, true);
		}

		public override void OnGUI(Rect p_Position, UnityEditor.SerializedProperty p_Property, GUIContent p_Label)
		{
			GUI.enabled = false;
			UnityEditor.EditorGUI.PropertyField(p_Position, p_Property, p_Label, true);
			GUI.enabled = true;
		}
	}
#endif
}
