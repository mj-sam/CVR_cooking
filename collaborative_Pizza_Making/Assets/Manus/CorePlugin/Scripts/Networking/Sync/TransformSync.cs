using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This syncs the transform position, rotation and scale.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Networking/Sync/Transform (Sync)")]
	public class TransformSync : BaseSync
	{
		Vector3 m_Position;
		Quaternion m_Rotation;
		Vector3 m_Scale;

		public bool smooth = false;
		Coroutine m_SmoothRoutine = null;

		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public override void Initialize(Manus.Networking.NetObject p_Object)
		{
			m_Position = transform.localPosition;
			m_Rotation = transform.localRotation;
			m_Scale = transform.localScale;
		}

		/// <summary>
		/// The function called when a Syncable needs to be cleaned.
		/// This function should make the IsDirty return false.
		/// </summary>
		public override void Clean()
		{
			m_Position = transform.localPosition;
			m_Rotation = transform.localRotation;
			m_Scale = transform.localScale;
		}

		/// <summary>
		/// The function called to see if a Syncable is dirty.
		/// Returns true if it need to be Synced.
		/// </summary>
		/// <returns>Returns true if it need to be Synced.</returns>
		public override bool IsDirty()
		{
			if (m_SmoothRoutine != null) return false; //still busy, probably
			if (m_Position != transform.localPosition) return true;
			if (m_Rotation != transform.localRotation) return true;
			if (m_Scale != transform.localScale) return true;
			return false;
		}

		/// <summary>
		/// Receives all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to read the data from</param>
		public override void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			m_Position.x = p_Msg.ReadFloat();
			m_Position.y = p_Msg.ReadFloat();
			m_Position.z = p_Msg.ReadFloat();

			m_Rotation.x = p_Msg.ReadFloat();
			m_Rotation.y = p_Msg.ReadFloat();
			m_Rotation.z = p_Msg.ReadFloat();
			m_Rotation.w = p_Msg.ReadFloat();

			m_Scale.x = p_Msg.ReadFloat();
			m_Scale.y = p_Msg.ReadFloat();
			m_Scale.z = p_Msg.ReadFloat();

			if (!enabled) return;
			if (smooth)
			{
				if (m_SmoothRoutine != null) StopCoroutine(m_SmoothRoutine);
				m_SmoothRoutine = StartCoroutine(SmoothTransform());
				return;
			}
			transform.localPosition = m_Position;
			transform.localRotation = m_Rotation;
			transform.localScale = m_Scale;
		}

		/// <summary>
		/// Smooths the transformation with a simple lerp.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator SmoothTransform()
		{
			float t_Time = 0.0f;
			Vector3 t_StartPosition = transform.localPosition;
			Quaternion t_StartRotation = transform.localRotation;
			Vector3 t_StartScale = transform.localScale;
			while (t_Time < 0.03f)
			{
				t_Time += Time.deltaTime;
				float t_Perc = t_Time / 0.03f;
				transform.localPosition = Vector3.Lerp(t_StartPosition, m_Position, t_Perc);
				transform.localRotation = Quaternion.Lerp(t_StartRotation, m_Rotation, t_Perc);
				transform.localScale = Vector3.Lerp(t_StartScale, m_Scale, t_Perc);
				yield return new WaitForEndOfFrame();
			}
			transform.localPosition = m_Position;
			transform.localRotation = m_Rotation;
			transform.localScale = m_Scale;

			m_SmoothRoutine = null;
		}

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
			p_Msg.Write(m_Position.x);
			p_Msg.Write(m_Position.y);
			p_Msg.Write(m_Position.z);

			p_Msg.Write(m_Rotation.x);
			p_Msg.Write(m_Rotation.y);
			p_Msg.Write(m_Rotation.z);
			p_Msg.Write(m_Rotation.w);

			p_Msg.Write(m_Scale.x);
			p_Msg.Write(m_Scale.y);
			p_Msg.Write(m_Scale.z);
		}

		/// <summary>
		/// On disable stops the coroutine.
		/// </summary>
		private void OnDisable()
		{
			if (m_SmoothRoutine != null) StopCoroutine(m_SmoothRoutine);
			m_SmoothRoutine = null;
		}
	}
}
