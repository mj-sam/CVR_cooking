using UnityEngine;
using LidNet = Lidgren.Network;

namespace Manus.Networking.Sync
{
	/// <summary>
	/// This class synchronizes the rotation of a Transform.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Networking/Sync/Rotation (Sync)")]
	public class RotationSync : BaseSync
	{
		Quaternion m_Rotation;

		public bool smooth = false;
		Coroutine m_SmoothRoutine = null;

		/// <summary>
		/// The function called when a NetObject is Initialized.
		/// </summary>
		/// <param name="p_Object">The Net Object this Sync belongs to.</param>
		public override void Initialize(Manus.Networking.NetObject p_Object)
		{
			m_Rotation = transform.localRotation;
		}

		/// <summary>
		/// The function called when a Syncable needs to be cleaned.
		/// This function should make the IsDirty return false.
		/// </summary>
		public override void Clean()
		{
			m_Rotation = transform.localRotation;
		}

		/// <summary>
		/// The function called to see if a Syncable is dirty.
		/// Returns true if it need to be Synced.
		/// </summary>
		/// <returns>Returns true if it need to be Synced.</returns>
		public override bool IsDirty()
		{
			if (m_SmoothRoutine != null) return false; //still busy, probably
			if (m_Rotation != transform.localRotation) return true;
			return false;
		}

		/// <summary>
		/// Receives all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to read the data from</param>
		public override void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			m_Rotation.x = p_Msg.ReadFloat();
			m_Rotation.y = p_Msg.ReadFloat();
			m_Rotation.z = p_Msg.ReadFloat();
			m_Rotation.w = p_Msg.ReadFloat();

			if (smooth)
			{
				if (m_SmoothRoutine != null) StopCoroutine(m_SmoothRoutine);
				m_SmoothRoutine = StartCoroutine(SmoothTransform());
				return;
			}
			transform.localRotation = m_Rotation;
		}

		/// <summary>
		/// Smooths the transformation with a simple lerp.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator SmoothTransform()
		{
			float t_Time = 0.0f;
			Quaternion t_StartRotation = transform.localRotation;
			while (t_Time < 0.03f)
			{
				t_Time += Time.deltaTime;
				float t_Perc = t_Time / 0.03f;
				transform.localRotation = Quaternion.Lerp(t_StartRotation, m_Rotation, t_Perc);
				yield return new WaitForEndOfFrame();
			}
			transform.localRotation = m_Rotation;

			m_SmoothRoutine = null;
		}

		/// <summary>
		/// Writes all information that needs to be synced.
		/// </summary>
		/// <param name="p_Msg">The buffer to write the data to</param>
		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
			p_Msg.Write(m_Rotation.x);
			p_Msg.Write(m_Rotation.y);
			p_Msg.Write(m_Rotation.z);
			p_Msg.Write(m_Rotation.w);
		}
	}
}
