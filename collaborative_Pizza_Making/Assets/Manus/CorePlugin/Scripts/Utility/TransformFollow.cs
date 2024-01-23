using UnityEngine;

namespace Manus.Utility
{
	/// <summary>
	/// This component makes a transform follow another transform.
	/// </summary>
	[AddComponentMenu("Manus/Utility/Transform Follow")]
	public class TransformFollow : MonoBehaviour
	{
		public UpdateMoment m_UpdateMoment = UpdateMoment.Update;

		[Header("Position Variables")]
		public bool followPosition = true;
		public bool smoothPosition = false;
		public float maxDistanceDeltaPerFrame = 0.003f;

		[Header("Rotation Variables")]
		public bool followRotation = true;

		[Header("Offsets")]
		public Vector3 positionOffset;
		public Vector3 rotationOffset;

		[Header("Transforms")]
		public Transform target;
		public Transform source;

		/// <summary>
		/// Sets the source as this transform if the source is unassigned.
		/// </summary>
		void Awake()
		{
			if (source == null) source = transform;
			Follow();
		}

		/// <summary>
		/// If the update moment is Update then it copies the transform.
		/// </summary>
		protected void Update()
		{
			if (m_UpdateMoment != UpdateMoment.Update) return;
			Follow();
		}

		/// <summary>
		/// If the update moment is FixedUpdate then it copies the transform.
		/// </summary>
		protected void FixedUpdate()
		{
			if (m_UpdateMoment != UpdateMoment.FixedUpdate) return;
			Follow();
		}

		/// <summary>
		/// If the update moment is LateUpdate then it copies the transform.
		/// </summary>
		protected virtual void LateUpdate()
		{
			if (m_UpdateMoment != UpdateMoment.LateUpdate) return;
			Follow();
		}

		/// <summary>
		/// Makes the source transform follow the target transform.
		/// </summary>
		protected void Follow()
		{
			if (followPosition)
			{
				FollowPosition();
			}

			if (followRotation)
			{
				FollowRotation();
			}
		}

		/// <summary>
		/// Makes the source transform follow the target transform's position.
		/// </summary>
		protected void FollowPosition()
		{
			Vector3 t_TargetTransformPosition = target.TransformPoint(positionOffset);
			Vector3 t_NewPosition;

			if (smoothPosition)
			{
				float t_Alpha = Mathf.Clamp01(Vector3.Distance(source.position, t_TargetTransformPosition) / maxDistanceDeltaPerFrame);
				t_NewPosition = Vector3.Lerp(source.position, t_TargetTransformPosition, t_Alpha);
			}
			else
			{
				t_NewPosition = t_TargetTransformPosition;
			}
			source.position = t_NewPosition;
		}

		/// <summary>
		/// Makes the source transform follow the target transform's rotation.
		/// </summary>
		protected void FollowRotation()
		{
			Quaternion t_TargetTransformRotation = target.rotation * Quaternion.Euler(rotationOffset);
			Quaternion t_NewRotation;

			if (smoothPosition)
			{
				float t_Alpha = Mathf.Clamp01(Quaternion.Angle(source.rotation, t_TargetTransformRotation) / maxDistanceDeltaPerFrame);
				t_NewRotation = Quaternion.Lerp(source.rotation, t_TargetTransformRotation, t_Alpha);
			}
			else
			{
				t_NewRotation = t_TargetTransformRotation;
			}
			source.rotation = t_NewRotation;
		}
	}
}
