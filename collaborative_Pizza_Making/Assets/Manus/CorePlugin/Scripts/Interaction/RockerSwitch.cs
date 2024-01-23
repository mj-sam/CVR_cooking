using System;
using System.Collections;
using UnityEngine;

using Manus.Utility; // FindDeepChild

namespace Manus.Interaction
{
	/// <summary>
	/// A rocker switch implementation, has an On and Off state.
	/// </summary>
	[AddComponentMenu("Manus/Interaction/Rocker Switch")]
	public class RockerSwitch : MonoBehaviour
	{
		#region Fields & Properties

		#region Public Properties

		#endregion // Public Properties

		#region Public Fields
		/// <summary>
		/// Collision areas that are used to trigger the On state.
		/// </summary>
		public CollisionArea[] collisionAreasOn;

		/// <summary>
		/// Collision areas that are used to trigger the Off state.
		/// </summary>
		public CollisionArea[] collisionAreasOff;

		/// <summary>
		/// The action called when the state changes to On.
		/// </summary>
		public Action<RockerSwitch> onStateOn;

		/// <summary>
		/// The action called when the state changes to Off.
		/// </summary>
		public Action<RockerSwitch> onStateOff;

		/// <summary>
		/// The pivot used to rotate around when changing states.
		/// </summary>
		public Transform pivot = null;

		/// <summary>
		/// The rotational extents used to rotate when changing states.
		/// </summary>
		public Vector3 pivotExtentlocalRotation = new Vector3(10.0f, 0.0f, 0.0f);

		/// <summary>
		/// This is the initial state of the object when it is enabled, true for On and false for Off.
		/// </summary>
		public bool initialState = false;

		#endregion // Public Fields

		#region Protected Fields

		protected bool m_State = false;

		Coroutine m_Reevaluate = null;

		#endregion // Protected Fields

		#endregion // Fields & Properties

		#region Methods

		#region Unity Messages
		/// <summary>
		/// Finds the pivot if not assigned.
		/// </summary>
		void Awake()
		{
			if (pivot == null)
			{
				pivot = transform.FindDeepChild("Pivot");
				if (pivot == null)
				{
					Debug.LogError($"No rocker pivot could be found.");
					enabled = false;
					return;
				}
			}

			foreach (var t_Area in collisionAreasOn)
			{
				t_Area.onCollisionChanged += OnCollisionAreasChanged;
			}
			foreach (var t_Area in collisionAreasOff)
			{
				t_Area.onCollisionChanged += OnCollisionAreasChanged;
			}
		}

		void OnEnable()
		{
			SetState(initialState);
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
		/// This coroutine is called when the switch needs to be reevaluated.
		/// It looks at the collision areas and changes the state if such a change is desired.
		/// </summary>
		IEnumerator Reevaluate()
		{
			yield return new WaitForFixedUpdate();
			m_Reevaluate = null;
			bool t_GoToOn = false;
			foreach (var t_Area in collisionAreasOn)
			{
				if (t_Area.collidingObjects != 0) t_GoToOn = true;
			}
			bool t_GoToOff = false;
			foreach (var t_Area in collisionAreasOff)
			{
				if (t_Area.collidingObjects != 0) t_GoToOff = true;
			}

			if (t_GoToOff == t_GoToOn)
			{
				yield break;
			}

			SetState(t_GoToOn);
		}

		/// <summary>
		/// Sets the switch's state and invokes the desired actions.
		/// </summary>
		/// <param name="p_State">True for On and False for Off</param>
		protected void SetState(bool p_State)
		{
			m_State = p_State;

			if (m_State)
			{
				pivot.localRotation = Quaternion.Euler(pivotExtentlocalRotation);

				onStateOn?.Invoke(this);
			}
			else
			{
				pivot.localRotation = Quaternion.Euler(-pivotExtentlocalRotation);

				onStateOff?.Invoke(this);
			}
		}

		#endregion // Protected Methods

		#endregion // Methods
	}
}
