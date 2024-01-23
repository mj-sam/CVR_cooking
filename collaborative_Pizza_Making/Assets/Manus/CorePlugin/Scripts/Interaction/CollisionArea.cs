using System;
using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This class keeps track of collisions that are detected between it and other objects.
	/// These objects are allowed to be triggers IF they belong to a HandCollider.
	/// </summary>
	[AddComponentMenu("Manus/Interaction/Collision Area")]
	public class CollisionArea : MonoBehaviour
	{
		/// <summary>
		/// The amount of objects colliding with this collision area.
		/// </summary>
		public int collidingObjects
		{
			get { return m_CollidingObjects; }
		}

		/// <summary>
		/// The event to call when the collision amount changed.
		/// </summary>
		public Action<CollisionArea> onCollisionChanged;


		int m_CollidingObjects = 0;

		/// <summary>
		/// This is called when a collision starts.
		/// </summary>
		public void StartCollision()
		{
			m_CollidingObjects++;
			onCollisionChanged?.Invoke(this);
		}

		/// <summary>
		/// This is called when a collision ends.
		/// </summary>
		public void EndCollision()
		{
			m_CollidingObjects--;
			onCollisionChanged?.Invoke(this);
		}

		/// <summary>
		/// Called by Unity.
		/// Makes Physical collisions also count towards the amount of colliding objects.
		/// </summary>
		/// <param name="p_Collision"></param>
		private void OnCollisionEnter(Collision p_Collision)
		{
			StartCollision();
		}

		/// <summary>
		/// Called by Unity.
		/// Makes Physical collisions also count towards the amount of colliding objects.
		/// </summary>
		/// <param name="p_Collision"></param>
		private void OnCollisionExit(Collision p_Collision)
		{
			EndCollision();
		}

		/// <summary>
		/// Called by Unity.
		/// Makes Trigger collisions also count towards the amount of colliding objects.
		/// Ignores other triggers UNLESS they belong to a HandCollider.
		/// </summary>
		/// <param name="p_Other"></param>
		void OnTriggerEnter(Collider p_Other)
		{
			if (p_Other.isTrigger)
			{
				var t_HandInfo = HandCollision.GetHandColliderInfo(p_Other);
				if (t_HandInfo == null)
				{
					return;
				}
			}
			StartCollision();
		}

		/// <summary>
		/// Called by Unity.
		/// Makes Trigger collisions also count towards the amount of colliding objects.
		/// Ignores other triggers UNLESS they belong to a HandCollider.
		/// </summary>
		/// <param name="p_Other"></param>
		void OnTriggerExit(Collider p_Other)
		{
			if (p_Other.isTrigger)
			{
				var t_HandInfo = HandCollision.GetHandColliderInfo(p_Other);
				if (t_HandInfo == null)
				{
					return;
				}
			}
			EndCollision();
		}
	}
}
