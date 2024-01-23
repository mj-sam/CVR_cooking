using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.InteractionScene
{
	/// <summary>
	/// This class is used in the Demo to demonstrate interaction between all the interactables and other objects.
	/// This code is purely demonstrational and probably does not have much use outside this specific scenario.
	/// </summary>
	[AddComponentMenu("Manus/Interaction Scene/Object Influenced By Interactables")]
	public class ObjectInfluencedByInteractables : MonoBehaviour
	{
		#region Fields & Properties

		#region Public Properties

		public Interaction.PushButton buttonThatChangesMaterial = null;
		public Material materialWhenButtonPressed = null;
		public Material materialWhenButtonNotPressed = null;

		public Interaction.RockerSwitch switchThatTogglesGravity = null;

		public Interaction.DialKnob knobThatSetsRotation = null;

		#endregion // Public Properties

		#region Protected Variables

		protected Renderer m_Renderer = null;

		protected Rigidbody m_Rigidbody = null;
		protected Vector3 m_InitialPosition = Vector3.zero;
		protected Vector3 m_InitialLocalRotation = Vector3.zero;

		#endregion // Protected Variables

		#endregion // Fields & Properties

		#region Methods

		#region Unity Messages

		protected virtual void Awake()
		{
			m_Renderer = GetComponent<Renderer>();
			if (m_Renderer == null)
			{
				Debug.LogError($"Failed to find a renderer. This script needs one to function.");
				enabled = false;

				return;
			}

			m_Rigidbody = GetComponent<Rigidbody>();
			if (m_Rigidbody == null)
			{
				Debug.LogError($"Failed to find a RigidBody. This script needs one to function.");
				enabled = false;

				return;
			}

			m_InitialPosition = transform.position;
			m_InitialLocalRotation = transform.localRotation.eulerAngles;

			if (buttonThatChangesMaterial == null)
			{
				Debug.LogError($"No PushButton was given. This script needs one to function.");
				enabled = false;

				return;
			}

			if (switchThatTogglesGravity == null)
			{
				Debug.LogError($"No RockerSwitch was given. This script needs one to function.");
				enabled = false;

				return;
			}

			if (knobThatSetsRotation == null)
			{
				Debug.LogError($"No DialKnob was given. This script needs one to function.");
				enabled = false;

				return;
			}

			m_Renderer.material = materialWhenButtonNotPressed;
		}

		protected virtual void OnEnable()
		{
			if (buttonThatChangesMaterial != null)
			{
				buttonThatChangesMaterial.onPressed += ReactToPushButton;
			}

			if (switchThatTogglesGravity != null)
			{
				switchThatTogglesGravity.onStateOn += ReactToRockerSwitchEnabled;
				switchThatTogglesGravity.onStateOff += ReactToRockerSwitchDisabled;
			}

			if (knobThatSetsRotation != null)
			{
				knobThatSetsRotation.onDialValueChanged += OnDialValueChanged;
			}
		}

		protected virtual void OnDisable()
		{
			if (buttonThatChangesMaterial != null)
			{
				buttonThatChangesMaterial.onPressed -= ReactToPushButton;
			}

			if (switchThatTogglesGravity != null)
			{
				switchThatTogglesGravity.onStateOn -= ReactToRockerSwitchEnabled;
				switchThatTogglesGravity.onStateOff -= ReactToRockerSwitchDisabled;
			}

			if (knobThatSetsRotation != null)
			{
				knobThatSetsRotation.onDialValueChanged -= OnDialValueChanged;
			}
		}

		protected virtual void FixedUpdate()
		{
			if (m_Rigidbody.useGravity)
				return;

			Vector3 t_ToTarget = m_InitialPosition - transform.position;
			Vector3 t_UnclampedVelocity = t_ToTarget.normalized;
			float t_VelocityNeededToReachTarget = t_ToTarget.magnitude / Time.fixedDeltaTime;
			Vector3 t_ClampedVelocity = Vector3.ClampMagnitude(t_UnclampedVelocity, t_VelocityNeededToReachTarget);

			m_Rigidbody.velocity = t_ClampedVelocity;
		}

		#endregion // Unity Messages

		#region Protected Methods
		protected void ReactToPushButton(Interaction.PushButton p_Button)
		{
			if (p_Button.state)
			{
				m_Renderer.material = materialWhenButtonPressed;
			}
			else
			{
				m_Renderer.material = materialWhenButtonNotPressed;
			}
		}

		protected void ReactToPushButtonDisabled(Interaction.PushButton p_Button)
		{
		}

		protected void ReactToRockerSwitchEnabled(Interaction.RockerSwitch p_Switch)
		{
			m_Rigidbody.useGravity = false;
		}

		protected void ReactToRockerSwitchDisabled(Interaction.RockerSwitch p_Switch)
		{
			m_Rigidbody.useGravity = true;
		}

		protected void OnDialValueChanged(Interaction.DialKnob p_DialKnob)
		{
			float t_Rotation = p_DialKnob.value * -360.0f;

			transform.localRotation = Quaternion.Euler(m_InitialLocalRotation.x, m_InitialLocalRotation.y, t_Rotation);

			m_Rigidbody.velocity = Vector3.zero;
			m_Rigidbody.angularVelocity = Vector3.zero;
		}

		#endregion // Protected Methods

		#endregion // Methods
	}
}
