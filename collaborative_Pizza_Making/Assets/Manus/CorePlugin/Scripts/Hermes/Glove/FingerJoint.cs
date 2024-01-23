using Manus.Utility;
using HProt = Hermes.Protocol;
using UnityEngine;

namespace Manus.Hermes.Glove
{
	public partial class FingerJoint
	{
		#region Properties
		/// <summary>
		/// Returns which data is valid on this joint.
		/// </summary>
		public ValidFingerJointData validData
		{
			get
			{
				return m_ValidData;
			}
		}

		/// <summary>
		/// The raw flex sensor value.
		/// </summary>
		public float flex
		{
			get
			{
				return m_Flex;
			}
		}

		/// <summary>
		/// The calculated position of the joint.
		/// </summary>
		public Vector3 position
		{
			get
			{
				return m_Position;
			}
		}

		/// <summary>
		/// The calculated rotation of the joint.
		/// </summary>
		public Quaternion rotation
		{
			get
			{
				return m_Rotation;
			}
		}

		/// <summary>
		/// The stretch sensor value.
		/// </summary>
		public float stretch
		{
			get
			{
				return m_Stretch;
			}
		}

		/// <summary>
		/// The stretch from the rest position in degrees.
		/// The resting position is a flat hand with the palm facing downwards.
		/// </summary>
		public float stretchDegrees
		{
			get
			{
				return m_StretchDegrees;
			}
		}

		/// <summary>
		/// The spread sensor value.
		/// </summary>
		public float spread
		{
			get
			{
				return m_Spread;
			}
		}

		/// <summary>
		/// The spread from the rest position in degrees.
		/// The resting position is a flat hand with the palm facing downwards.
		/// </summary>
		public float spreadDegrees
		{
			get
			{
				return m_SpreadDegrees;
			}
		}
		#endregion

		#region Fields
		ValidFingerJointData m_ValidData = ValidFingerJointData.None;

		Vector3 m_Position = Vector3.zero;
		Quaternion m_Rotation = Quaternion.identity;

		float m_Flex = 0.0f;
		float m_Stretch = 0.0f;
		float m_StretchDegrees = 0.0f;
		float m_Spread = 0.0f;
		float m_SpreadDegrees = 0.0f;
		#endregion

		/// <summary>
		/// Initializes the joint with the basic information.
		/// </summary>
		public FingerJoint()
		{
			m_ValidData = ValidFingerJointData.None;
			m_Flex = 0.0f; //invalid
			m_Position = Vector3.zero;
			m_Rotation = Quaternion.identity;
		}

		/// <summary>
		/// Applies Hermes Phalange data to the joint.
		/// </summary>
		/// <param name="p_Data">The Hermes Phalange data.</param>
		public void ApplyData(HProt.Phalange p_Data)
		{
			m_ValidData |= ValidFingerJointData.Rotation | ValidFingerJointData.Position;
			m_Position = p_Data.Position.ToUnity();
			m_Rotation = p_Data.Rotation.ToUnity();
			m_Stretch = p_Data.Stretch;
			m_StretchDegrees = p_Data.StretchDegrees;
			m_Spread = p_Data.Spread;
			m_SpreadDegrees = p_Data.SpreadDegrees;
		}

		/// <summary>
		/// Applies a flex value to the joint.
		/// </summary>
		/// <param name="p_Data">The flex value, is usually between 0 and 1.</param>
		public void ApplyFlexData(float p_Data)
		{
			m_ValidData |= ValidFingerJointData.Flex;
			m_Flex = p_Data;
		}

		/// <summary>
		/// Applies a stretch value to the joint.
		/// </summary>
		/// <param name="p_Raw">The stretch value, is usually between 0 and 1.</param>
		/// <param name="p_Degrees">The stretch value in degrees from a resting flat handed position.</param>
		public void ApplyStretchData(float p_Raw, float p_Degrees = 0.0f)
		{
			m_ValidData |= ValidFingerJointData.Stretch;
			m_Stretch = p_Raw;
			m_StretchDegrees = p_Degrees;
		}

		/// <summary>
		/// Applies a spread value to the joint.
		/// </summary>
		/// <param name="p_Raw">The spread value, is usually between 0 and 1.</param>
		/// <param name="p_Degrees">The spread value in degrees from a resting flat handed position.</param>
		public void ApplySpreadData(float p_Raw, float p_Degrees = 0.0f)
		{
			m_ValidData |= ValidFingerJointData.Spread;
			m_Spread = p_Raw;
			m_SpreadDegrees = p_Degrees;
		}
	}
}
