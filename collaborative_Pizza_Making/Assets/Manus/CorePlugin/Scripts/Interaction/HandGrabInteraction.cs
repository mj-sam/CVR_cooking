using System.Collections.Generic;
using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This is the class used by the Hand in order to grab grabbable objects.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Interaction/Hand Grab Interaction")]
	public class HandGrabInteraction : MonoBehaviour
	{
		/// <summary>
		/// Gesture used to determine when a grab is being made.
		/// </summary>
		public Hand.Gesture.GestureBase grabGesture;

		/// <summary>
		/// Returns the hand this module belongs to.
		/// </summary>
		public Hand.Hand hand
		{
			get
			{
				return m_Hand;
			}
		}

		/// <summary>
		/// Returns the currently grabbed object.
		/// Returns NULL if there is no object being grabbed.
		/// </summary>
		public GrabbedObject grabbedObject
		{
			get
			{
				return m_GrabbedObject;
			}
		}

		//private
		Hand.Hand m_Hand;
		bool m_Grabbing = false;

		List<Collider> m_InteractableColliders;
		GrabbedObject m_GrabbedObject;

		public bool testGrab = false;

		/// <summary>
		/// Called by Unity.
		/// Locates the Hand in this gameobject or its parents.
		/// </summary>
		void Start()
		{
			m_InteractableColliders = new List<Collider>();
			m_Hand = GetComponentInParent<Hand.Hand>();
		}

		/// <summary>
		/// This function evaluates the gesture and handles accordingly.
		/// The function only evaluates if the hand belongs to the local player.
		/// </summary>
		void FixedUpdate()
		{
			if (!m_Hand.isLocalPlayer) return;
			bool t_Grab = grabGesture.Evaluate(m_Hand) || testGrab;
			if (m_Grabbing != t_Grab)
			{
				m_Grabbing = t_Grab;
				if (m_Grabbing)
				{
					Grab();
				}
				else
				{
					this.GetComponent<SphereCollider>().enabled = true;
					Release();
				}
			}
			m_InteractableColliders.Clear();
		}

		/// <summary>
		/// This function looks at the current triggers that are overlapping with the interactable
		/// and calculates which one is most likely the one the hand wishes to grab.
		/// </summary>
		public void Grab()
		{
			if (m_InteractableColliders.Count == 0) return;
			m_InteractableColliders.RemoveAll(t_Item => t_Item == null);
			Vector3 t_Point = transform.position;
			float t_D = 0.0f;
			GrabbedObject.Info t_Info = new GrabbedObject.Info(this);
			foreach (Collider t_Col in m_InteractableColliders)
			{
				Vector3 t_NCP = t_Col.ClosestPoint(t_Point);
				t_D = Vector3.Distance(t_NCP, t_Point);
				if (t_D < t_Info.distance)
				{
					t_Info.collider = t_Col;
					t_Info.nearestColliderPoint = t_NCP;
					t_Info.distance = t_D;
				}
			}
			if (t_Info.collider == null) return;
			var t_Obj = t_Info.collider.GetComponentInParent<IGrabbable>() as MonoBehaviour;
			if (t_Obj == null) Debug.LogWarning("This should not be happening!");
			if (m_GrabbedObject != null)
			{
				if (!m_GrabbedObject.RemoveInteractingHand(this))
				{
					Debug.LogWarning("The previously Grabbed Object was not tracking this hand!");
				}
			}
			m_GrabbedObject = t_Obj.GetComponent<GrabbedObject>();
			

			if (m_GrabbedObject == null) m_GrabbedObject = t_Obj.gameObject.AddComponent<GrabbedObject>();

			//Calculate info
			t_Info.nearestColliderPoint = t_Obj.transform.InverseTransformPoint(t_Info.nearestColliderPoint);
			t_Info.handToObject = m_Hand.transform.InverseTransformPoint(t_Obj.transform.position);
			t_Info.objectToHand = t_Obj.transform.InverseTransformPoint(m_Hand.transform.position);

			t_Info.objectInteractorForward = t_Obj.transform.InverseTransformDirection(transform.forward);

			t_Info.handToObjectRotation = Quaternion.Inverse(hand.transform.rotation) * t_Obj.transform.rotation; //Diff = Target * Inv(Src)

			if (!m_GrabbedObject.AddInteractingHand(t_Info))
			{
				Debug.LogWarning("The Grabbed Object was already tracking this hand!");
			}

			if(t_Info.collider.GetComponentInParent<PizzaDeformer>() != null)
			{
				t_Info.collider.GetComponentInParent<PizzaDeformer>().PickUp();
			}

			t_Obj.transform.gameObject.GetComponent<GrabbedItem>().GrabHaptic(t_Info.interacter.transform.gameObject);
		}

		/// <summary>
		/// This function releases the currently grabbed object.
		/// </summary>
		public void Release()
		{
			if (m_GrabbedObject != null)
			{
				if (!m_GrabbedObject.RemoveInteractingHand(this))
				{
					Debug.LogWarning("The previously Grabbed Object was not tracking this hand!");
				}
				if(m_GrabbedObject.GetComponentInParent<PizzaDeformer>() != null)
				{
					m_GrabbedObject.GetComponentInParent<PizzaDeformer>().LetGo();
				}
				m_GrabbedObject = null;
			}
		}

		/// <summary>
		/// This function enforces a grab on a given IGrabbable object.
		/// The GrabbedObject.Info is generated on the current location of the hand,
		/// these may want to be overwritten to create a better grab look.
		/// </summary>
		/// <param name="p_Grabbable">The Grabbable Object</param>
		public void GrabGrabbable(IGrabbable p_Grabbable)
		{
			Vector3 t_Point = transform.position;
			GrabbedObject.Info t_Info = new GrabbedObject.Info(this);
			var t_Obj = p_Grabbable as MonoBehaviour;
			if (t_Obj == null) Debug.LogWarning("This should not be happening!");
			if (m_GrabbedObject != null)
			{
				if (!m_GrabbedObject.RemoveInteractingHand(this))
				{
					Debug.LogWarning("The previously Grabbed Object was not tracking this hand!");
				}
			}
			m_GrabbedObject = t_Obj.GetComponent<GrabbedObject>();
			if (m_GrabbedObject == null) m_GrabbedObject = t_Obj.gameObject.AddComponent<GrabbedObject>();

			t_Info.collider = null;
			t_Info.nearestColliderPoint = Vector3.zero;
			t_Info.distance = Vector3.Distance(t_Point, m_GrabbedObject.transform.position);

			//Calculate info
			t_Info.nearestColliderPoint = t_Obj.transform.InverseTransformPoint(t_Info.nearestColliderPoint);
			t_Info.handToObject = m_Hand.transform.InverseTransformPoint(t_Obj.transform.position);
			
			t_Info.objectToHand = t_Obj.transform.InverseTransformPoint(m_Hand.transform.position);

			t_Info.objectInteractorForward = t_Obj.transform.InverseTransformDirection(transform.forward);

			t_Info.handToObjectRotation = Quaternion.Inverse(hand.transform.rotation) * t_Obj.transform.rotation; //Diff = Target * Inv(Src)

			if (!m_GrabbedObject.AddInteractingHand(t_Info))
			{
				Debug.LogWarning("The Grabbed Object was already tracking this hand!");
			}
		}

		/// <summary>
		/// This function is called by Unity when a trigger is entered, it will keep track of IGrabbable objects.
		/// </summary>
		/// <param name="p_Other"></param>
		void OnTriggerStay(Collider p_Other)
		{
			var t_Grabbable = p_Other.GetComponentInParent<IGrabbable>();
			if (t_Grabbable == null || m_InteractableColliders.Contains(p_Other)) return;
			m_InteractableColliders.Add(p_Other);
		}

		/// <summary>
		/// This function is called by Unity when a trigger is left, it will keep track of IGrabbable objects.
		/// </summary>
		/// <param name="p_Other"></param>
		void OnTriggerExit(Collider p_Other)
		{
			var t_Grabbable = p_Other.GetComponentInParent<IGrabbable>();
			if (t_Grabbable == null) return;
			m_InteractableColliders.Remove(p_Other);
		}
	}
}
