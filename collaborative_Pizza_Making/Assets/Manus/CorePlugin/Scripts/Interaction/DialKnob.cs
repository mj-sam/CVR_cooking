using System;
using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This is a dial implementation, the dial is grabbable and rotatable.
	/// </summary>
	[AddComponentMenu("Manus/Interaction/Dial Knob")]
	public class DialKnob : MonoBehaviour, IGrabbable
	{
		#region Fields & Properties
		#region Public Properties
		/// <summary>
		/// The dial's rotation value, this value is between the rotationLimits.
		/// </summary>
		public float value
		{
			get { return m_Value; }
		}
		#endregion // Public Properties

		#region Public Fields
		/// <summary>
		/// The rotational limits in degrees.
		/// This value CAN be larger than 360 degrees!
		/// </summary>
		public Vector2 rotationLimits = new Vector3(0.0f, 90.0f);

		/// <summary>
		/// This event is triggered when the dial value changes.
		/// </summary>
		public Action<DialKnob> onDialValueChanged;
		#endregion // Public Fields

		#region Protected Types
		float m_Value = 0.0f;
		#endregion // Protected Fields

		#endregion // Fields & Properties

		/// <summary>
		/// Called when this starts getting grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedStart(GrabbedObject p_Object)
		{
		}

		/// <summary>
		/// Called when this stops being grabbed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedEnd(GrabbedObject p_Object)
		{
		}

		/// <summary>
		/// Called when a new grabber starts grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the added grabber</param>
		public void OnAddedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
		}

		/// <summary>
		/// Called when a grabber stops grabbing this.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		/// <param name="p_Info">Contains information about the removed grabber</param>
		public void OnRemovedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
		}

		/// <summary>
		/// Called every FixedUpdate when this is grabbed.
		/// This is where the rotation of the dial is calculated and changed.
		/// </summary>
		/// <param name="p_Object">Contains information about the grab</param>
		public void OnGrabbedFixedUpdate(GrabbedObject p_Object)
		{
			GrabbedObject.Info t_Info = p_Object.hands[0];

			Quaternion t_Rot = t_Info.interacter.hand.transform.rotation * t_Info.handToObjectRotation;

			Vector3 t_New = t_Rot * Vector3.forward;
			Vector3 t_Old = transform.forward;

			float t_Angle = Vector3.SignedAngle(t_Old, t_New, transform.up);

			m_Value = Mathf.Clamp(m_Value + t_Angle, rotationLimits.x, rotationLimits.y);
			transform.localRotation = Quaternion.Euler(0.0f, m_Value, 0.0f);

			t_Info.handToObjectRotation = Quaternion.Inverse(t_Info.interacter.hand.transform.rotation) * transform.transform.rotation; //Diff = Target * Inv(Src)
			onDialValueChanged?.Invoke(this);
		}
	}
}
