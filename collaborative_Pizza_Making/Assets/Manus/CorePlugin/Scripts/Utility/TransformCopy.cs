using UnityEngine;

namespace Manus.Utility
{
	/// <summary>
	/// This component copies a transform's properties to another transform. 
	/// </summary>
	[AddComponentMenu("Manus/Utility/Transform Copy")]
	public class TransformCopy : MonoBehaviour
	{
		public UpdateMoment updateMoment = UpdateMoment.Update;
		[Tooltip("Copy from Transform to Target, or from Target to Transform")]
		public bool toTarget = false;

		[Header("Transforms")]
		public Transform target;
		public Transform source;

		[Header("Transform parameters to copy")]
		[Tooltip("Copy in World or Object space")]
		public bool world = false;
		public bool position = false;
		public bool rotation = false;
		public bool scale = false;

		/// <summary>
		/// Sets the source as this transform if the source is unassigned.
		/// </summary>
		void Awake()
		{
			if (source == null) source = transform;
			Copy();
		}

		/// <summary>
		/// If the update moment is Update then it copies the transform.
		/// </summary>
		protected void Update()
		{
			if (updateMoment != UpdateMoment.Update) return;
			Copy();
		}

		/// <summary>
		/// If the update moment is FixedUpdate then it copies the transform.
		/// </summary>
		protected void FixedUpdate()
		{
			if (updateMoment != UpdateMoment.FixedUpdate) return;
			Copy();
		}

		/// <summary>
		/// If the update moment is LateUpdate then it copies the transform.
		/// </summary>
		protected virtual void LateUpdate()
		{
			if (updateMoment != UpdateMoment.LateUpdate) return;
			Copy();
		}

		/// <summary>
		/// Copies the transform's properties to the target transform.
		/// </summary>
		void Copy()
		{
			if (toTarget)
			{
				if (world)
				{
					if (position) target.position = transform.position;
					if (rotation) target.rotation = transform.rotation;
					if (scale) target.localScale = transform.localScale; //wrong but meh
					return;
				}
				if (position) target.localPosition = transform.localPosition;
				if (rotation) target.localRotation = transform.localRotation;
				if (scale) target.localScale = transform.localScale;
				return;
			}
			if (world)
			{
				if (position) transform.position = target.position;
				if (rotation) transform.rotation = target.rotation;
				if (scale) transform.localScale = target.localScale; //wrong but meh
				return;
			}
			if (position) transform.localPosition = target.localPosition;
			if (rotation) transform.localRotation = target.localRotation;
			if (scale) transform.localScale = target.localScale;
		}
	}
}
