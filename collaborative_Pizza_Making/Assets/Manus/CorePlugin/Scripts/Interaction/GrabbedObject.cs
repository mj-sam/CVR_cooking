using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This component is usually on an object that is currently being grabbed by one or more hands.
	/// It should not be added via the editor since the linkage between it and any interacters would be broken.
	/// It handles all the IGrabbable implementations on the object.
	/// </summary>
	[DisallowMultipleComponent]
	public class GrabbedObject : MonoBehaviour
	{
		/// <summary>
		/// This class contains information on a certain grab.
		/// </summary>
		public class Info
		{
			public Info(HandGrabInteraction p_Hand)
			{
				interacter = p_Hand;
				collider = null;
				distance = 99999.99f;
				nearestColliderPoint = Vector3.zero;
				handToObject = Vector3.zero;
				objectToHand = Vector3.zero;
				handToObjectRotation = Quaternion.identity;
				objectInteractorForward = Vector3.forward;
			}
			public HandGrabInteraction interacter;
			public Collider collider;
			public float distance;
			public Vector3 nearestColliderPoint;
			public Vector3 handToObject;
			public Vector3 objectToHand;
			public Vector3 objectInteractorForward;
			public Quaternion handToObjectRotation;
		}

		/// <summary>
		/// All of the hands holding onto the object.
		/// </summary>
		public List<Info> hands = new List<Info>();

		/// <summary>
		/// The rigid body this object has.
		/// </summary>
		public Rigidbody rigidBody = null;

		/// <summary>
		/// All of the IGrabbles this object has.
		/// </summary>
		List<IGrabbable> m_Grabbables = new List<IGrabbable>();

		/// <summary>
		/// Is this object being destroyed?
		/// </summary>
		bool m_BeingDestroyed = false;

		/// <summary>
		/// Called by Unity.
		/// Finds a rigid body and IGrabbables.
		/// Also calls OnGrabbedStart on all the IGrabbables.
		/// </summary>
		private void Awake()
		{
			rigidBody = GetComponent<Rigidbody>();

			m_Grabbables.AddRange(GetComponents<IGrabbable>());
			foreach(IGrabbable t_G in m_Grabbables)
			{
				t_G.OnGrabbedStart(this);
			}
		}

		/// <summary>
		/// Called by Unity.
		/// Upon destruction the GrabbedObject will send OnGrabbedEnd to the IGrabbables
		/// and request any interacting hands to release the object.
		/// </summary>
		private void OnDestroy()
		{
			m_BeingDestroyed = true;
			foreach (IGrabbable t_G in m_Grabbables)
			{
				t_G.OnGrabbedEnd(this);
				// if (this.GetComponent<GrabbableObject>() != null) t_G.OnGrabbedEnd(this);
			}
			foreach (Info t_Hand in hands)
			{
				if(t_Hand.interacter.grabbedObject==this)
				{
					t_Hand.interacter.Release();
				}
			}
		}

		/// <summary>
		/// Called by Unity.
		/// If there are hands grabbing the object, do the fixed update on all IGrabbables.
		/// </summary>
		private void FixedUpdate()
		{
			if (hands.Count == 0) return;
			foreach (IGrabbable t_G in m_Grabbables)
			{
				t_G.OnGrabbedFixedUpdate(this);
			}
		}

		/// <summary>
		/// Add the Info of the interacting hand to the list of hands.
		/// </summary>
		/// <param name="p_GrabInfo">Info containing the interacting hand and it's grab details.</param>
		/// <returns>False if the Hand was already grabbing.</returns>
		public bool AddInteractingHand(Info p_GrabInfo)
		{
			for (int i = 0; i < hands.Count; i++)
			{
				if (hands[i].interacter == p_GrabInfo.interacter) return false;
			}
			hands.Add(p_GrabInfo);
			foreach (IGrabbable t_G in m_Grabbables)
			{
				t_G.OnAddedInteractingInfo(this, p_GrabInfo);
			}
			return true;
		}


		/// <summary>
		/// Removes an HandInteraction from the list of hands.
		/// </summary>
		/// <param name="p_Hand">The interacting hand to remove.</param>
		/// <returns>False if the Hand was not in the list.</returns>
		public bool RemoveInteractingHand(HandGrabInteraction p_Hand)
		{
			for (int i = 0; i < hands.Count; i++)
			{
				if (hands[i].interacter == p_Hand)
				{
					var t_Info = hands[i];
					hands.RemoveAt(i);
					foreach (IGrabbable t_G in m_Grabbables)
					{
						t_G.OnRemovedInteractingInfo(this, t_Info);
					}
					if (hands.Count == 0 && !m_BeingDestroyed) Destroy(this);
					return true;
				}
			}
			if (hands.Count == 0 && !m_BeingDestroyed) Destroy(this);
			return false;
		}
	}
}
