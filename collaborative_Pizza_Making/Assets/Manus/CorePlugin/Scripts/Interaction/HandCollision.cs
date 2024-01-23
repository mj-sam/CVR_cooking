using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This class is used to detect collisions with the hand.
	/// It is required on a hand in order for CollisionAreas to find a hand.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Interaction/Hand Collision")]
	public class HandCollision : MonoBehaviour
	{
		public class Info
		{
			public HandCollision handCollision;
			public Collider collider;
		}

		#region Public Properties
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
		#endregion

		//private
		Hand.Hand m_Hand;

		/// <summary>
		/// The start function gets called by Unity and locates the Hand in this component or its parent.
		/// </summary>
		void Start()
		{
			m_Hand = GetComponentInParent<Hand.Hand>();
		}

		/// <summary>
		/// A function that can be run to find a HandCollision and generate info according to the HandCollision found.
		/// Returns NULL if no HandCollision can be found.
		/// </summary>
		/// <param name="p_Collider"></param>
		/// <returns>Information on the Hand Collision</returns>
		public static Info GetHandColliderInfo(Collider p_Collider)
		{
			var t_Hand = p_Collider.GetComponentInParent<HandCollision>();
			if (t_Hand == null) return null;
			return new Info() { handCollision = t_Hand, collider = p_Collider };
		}
	}
}
