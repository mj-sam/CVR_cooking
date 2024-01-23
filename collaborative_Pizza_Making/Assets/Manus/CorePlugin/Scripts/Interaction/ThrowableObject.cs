using System.Collections.Generic;
using UnityEngine;

namespace Manus.Interaction
{
	/// <summary>
	/// This class makes an object throwable.
	/// It analyzes the last few positions and rotations and uses these to estimate the throwing speed when released.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu("Manus/Interaction/Throwable Object")]
	public class ThrowableObject : MonoBehaviour, IGrabbable
	{
		uint m_PreviousCount = 5;
		Queue<Vector3> m_PrevPositions = new Queue<Vector3>();
		Queue<Quaternion> m_PrevRotations = new Queue<Quaternion>();

		public void OnAddedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
		}

		public void OnGrabbedEnd(GrabbedObject p_Object)
		{
			var t_RB = p_Object.GetComponent<Rigidbody>();
			if (t_RB == null) return;
			float t_ToFixedSpeed = (1.0f / Time.fixedDeltaTime);
			//Debug.Log("ToFixedSpeed: " + t_ToFixedSpeed);
			if (m_PrevPositions.Count > 1)
			{
				List<Vector3> t_List = new List<Vector3>();
				while (m_PrevPositions.Count != 0)
				{
					t_List.Add(m_PrevPositions.Dequeue());
				}
				for (int i = 0; i < t_List.Count - 1; i++)
				{
					t_List[i] = t_List[i + 1] - t_List[i];
				}
				t_List.RemoveAt(t_List.Count - 1);

				Vector3 t_Velocity = Vector3.zero;
				for (int i = 0; i < t_List.Count; i++)
				{
					t_Velocity += t_List[i];
				}
				t_Velocity /= t_List.Count;
				t_Velocity *= t_ToFixedSpeed;
				//	Debug.Log("Velocity: " + t_Velocity);
				t_RB.velocity = t_Velocity;
			}
			while (m_PrevRotations.Count > 1)
			{
				List<Quaternion> t_List = new List<Quaternion>();
				while (m_PrevRotations.Count != 0)
				{
					t_List.Add(m_PrevRotations.Dequeue());
				}

				Quaternion t_RotVel = t_List[t_List.Count-1] * Quaternion.Inverse(t_List[0]);

				var t_R = new Vector3(Mathf.DeltaAngle(0, t_RotVel.eulerAngles.x), Mathf.DeltaAngle(0, t_RotVel.eulerAngles.y), Mathf.DeltaAngle(0, t_RotVel.eulerAngles.z));
				t_R *= (1.0f / Time.fixedDeltaTime / t_List.Count);
				t_RB.angularVelocity = t_R * Mathf.Deg2Rad;
			}
			m_PrevPositions = null;
			m_PrevRotations = null;
		}

		public void OnGrabbedFixedUpdate(GrabbedObject p_Object)
		{
			m_PrevPositions.Enqueue(p_Object.transform.position);
			while (m_PrevPositions.Count > m_PreviousCount)
			{
				m_PrevPositions.Dequeue();
			}
			m_PrevRotations.Enqueue(p_Object.transform.rotation);
			while (m_PrevRotations.Count > m_PreviousCount)
			{
				m_PrevRotations.Dequeue();
			}
		}

		public void OnGrabbedStart(GrabbedObject p_Object)
		{
			m_PrevPositions = new Queue<Vector3>();
			m_PrevRotations = new Queue<Quaternion>();
		}

		public void OnRemovedInteractingInfo(GrabbedObject p_Object, GrabbedObject.Info p_Info)
		{
		}
	}
}
