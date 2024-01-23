using System;
using System.Collections;
using UnityEngine;

using Manus.Utility; // FindDeepChild


namespace Manus.Interaction
{
	/// <summary>
	/// A simple button implementation, has two ways of being turned on or off.
	/// </summary>
	[AddComponentMenu("Manus/Interaction/Push Button")]
	public class PushButton : MonoBehaviour
	{
		#region Fields & Properties

		#region Public Properties
		/// <summary>
		/// Is the button pressed?
		/// </summary>
		public bool pressed
		{
			get { return m_Pressed; }
		}

		/// <summary>
		/// The current state of the button, True for On and False for off.
		/// </summary>
		public bool state
		{
			get { return m_State; }
		}
		#endregion // Public Properties

		#region Public Fields
		/// <summary>
		/// The button type determines what happens when the button gets pressed.
		/// PushToToggle simulates a button that remains 'pressed' after being pressed and can be released by pressing it again.
		/// PushAndHold simulates a simple button which sends an On signal when pressed and Off when released.
		/// </summary>
		public ButtonType type = ButtonType.PushToToggle;

		/// <summary>
		/// All of the collision areas need to be empty in order for the button to stop being pressed.
		/// </summary>
		public CollisionArea[] emptyAreasForRelease;

		/// <summary>
		/// At least one collision area in this list needs to be filled in order to start being pressed.
		/// </summary>
		public CollisionArea[] filledAreasForPress;

		/// <summary>
		/// The event triggered when the button is no longer being pressed.
		/// </summary>
		public Action<PushButton> onReleased;

		/// <summary>
		/// The event triggered when the button starts being pressed.
		/// </summary>
		public Action<PushButton> onPressed;

		/// <summary>
		/// The event triggered when the button state turns to On.
		/// </summary>
		public Action<PushButton> onStateOn;

		/// <summary>
		/// The event triggered when the button state turns to Off.
		/// </summary>
		public Action<PushButton> onStateOff;

		/// <summary>
		/// The button which is moved.
		/// </summary>
		public Transform movingPart = null;

		/// <summary>
		/// The position of the button when it is in the On state.
		/// </summary>
		public Vector3 localPositionStateOn;

		/// <summary>
		/// The position of the button when it is in the Off state.
		/// </summary>
		public Vector3 localPositionStateOff;

		/// <summary>
		/// The position of the button when it is in the pressed state.
		/// </summary>
		public Vector3 localPositionPressed;
		#endregion // Public Fields

		#region Public Types
		/// <summary>
		/// PushToToggle simulates a button that remains 'pressed' after being pressed and can be released by pressing it again.
		/// PushAndHold simulates a simple button which sends an On signal when pressed and Off when released.
		/// </summary>
		public enum ButtonType
		{
			PushAndHold, /*!< Sends an On signal when pressed and Off when released */
			PushToToggle /*!< Changes signal upon press */
		}

		#endregion // Public Types

		#region Protected Fields

		protected bool m_State = false;
		protected bool m_Pressed = false;

		protected Coroutine m_Reevaluate = null;

		#endregion // Protected Fields

		#endregion // Fields & Properties

		#region Methods

		#region Unity Messages
		/// <summary>
		/// Finds the moving part if it is not defined.
		/// Adds the OnCollisionAreasChanged to all the collision areas.
		/// </summary>
		void Awake()
		{
			if (movingPart == null)
			{
				movingPart = transform.FindDeepChild("Button");
				if (movingPart == null)
				{
					Debug.LogWarning("Moving Part not assigned!");
					enabled = false;
					return;
				}
			}

			foreach (var t_Area in emptyAreasForRelease)
			{
				t_Area.onCollisionChanged += OnCollisionAreasChanged;
			}
			foreach (var t_Area in filledAreasForPress)
			{
				t_Area.onCollisionChanged += OnCollisionAreasChanged;
			}
		}
		#endregion // Unity Messages

		#region Public Methods
		/// <summary>
		/// This function is called when a collision area changes.
		/// This could mean an object either started or stopped touching the collision area.
		/// </summary>
		/// <param name="p_Area">The area that changed</param>
		public void OnCollisionAreasChanged(CollisionArea p_Area)
		{
			if (m_Reevaluate == null)
			{
				m_Reevaluate = StartCoroutine(Reevaluate());
			}
		}

		#endregion // Public Methods
		#region Protected Methods
		/// <summary>
		/// This coroutine is called when the button needs to be reevaluated.
		/// It looks at the collision areas and changes the state if such a change is desired.
		/// </summary>
		IEnumerator Reevaluate()
		{
			yield return new WaitForFixedUpdate();
			m_Reevaluate = null;
			bool t_CanRelease = true;
			foreach (var t_Area in emptyAreasForRelease)
			{
				if (t_Area.collidingObjects != 0) t_CanRelease = false;
			}
			bool t_CanPress = false;
			foreach (var t_Area in filledAreasForPress)
			{
				if (t_Area.collidingObjects != 0) t_CanPress = true;
			}
			//release
			if (t_CanRelease && !t_CanPress)
			{
				if (m_Pressed)//on release
				{
					if (type == ButtonType.PushAndHold)
					{
						m_State = false;
						onStateOff?.Invoke(this);
					}
					m_Pressed = false;
					onReleased?.Invoke(this);
				}
			}
			//press
			if (t_CanPress)
			{
				if (!m_Pressed)//on release
				{
					m_State = !m_State;
					if (m_State)
					{
						onStateOn?.Invoke(this);
					}
					else
					{
						onStateOff?.Invoke(this);
					}
					m_Pressed = true;
					onPressed?.Invoke(this);
				}
			}

			UpdateMovingPart();
		}

		/// <summary>
		/// Updates the location of the moving part.
		/// </summary>
		protected void UpdateMovingPart()
		{
			if (m_Pressed)
			{
				movingPart.localPosition = localPositionPressed;
				return;
			}
			if (m_State)
			{
				movingPart.localPosition = localPositionStateOn;
				return;
			}

			movingPart.localPosition = localPositionStateOff;
		}

		#endregion // Protected Methods
		#endregion // Methods
	}
}
