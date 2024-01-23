using Manus.Utility;
using HProt = Hermes.Protocol;

namespace Manus.Hermes.Glove
{
	/// <summary>
	/// This class contains data for a Finger.
	/// </summary>
	public partial class Finger
	{
		#region Properties
		/// <summary>
		/// This finger's type.
		/// </summary>
		public FingerType type
		{
			get
			{
				return m_Type;
			}
		}
		#endregion

		#region Fields
		FingerType m_Type = FingerType.Invalid;
		protected FingerJoint[] m_Joints = null;
		#endregion

		/// <summary>
		/// Initializes the finger with a given type.
		/// The finger will have 4 joints, unless it is a thumb which gets 3 joints.
		/// </summary>
		/// <param name="p_Type">The finger's Type.</param>
		public Finger(FingerType p_Type)
		{
			m_Type = p_Type;
			m_Joints = new FingerJoint[p_Type == FingerType.Thumb ? 3 : 4];
			for (int i = 0; i < m_Joints.Length; i++)
			{
				m_Joints[i] = new FingerJoint();
			}
		}

		/// <summary>
		/// Returns the Finger Joint information at a given index.
		/// </summary>
		/// <param name="p_Idx">The joint index.</param>
		/// <returns>Finger joint data.</returns>
		public FingerJoint GetJoint(int p_Idx)
		{
			return m_Joints[p_Idx];
		}

		/// <summary>
		/// Returns the Finger Joint information of a specific FingerJointType.
		/// </summary>
		/// <param name="p_Type">The joint's type.</param>
		/// <returns>Finger joint data.</returns>
		public FingerJoint GetJoint(FingerJointType p_Type)
		{
			return m_Joints[(int)p_Type];
		}

		/// <summary>
		/// This function applies Hermes data to the finger.
		/// </summary>
		/// <param name="p_Data">The Hermes data to apply.</param>
		public void ApplyData(HProt.Glove p_Data)
		{
			int t_Offset = m_Type == FingerType.Thumb ? 0 : 1;
			for (int i = 0; i < p_Data.Fingers[(int)m_Type].Phalanges.Count; i++)
			{
				m_Joints[t_Offset + i].ApplyData(p_Data.Fingers[(int)m_Type].Phalanges[i]);
			}

			m_Joints[(int)FingerJointType.MCP].ApplyFlexData(p_Data.Raw.Flex[(int)m_Type].MCPFlex);
			m_Joints[(int)FingerJointType.PIP].ApplyFlexData(p_Data.Raw.Flex[(int)m_Type].PIPFlex);
			if (m_Joints.Length > (int)FingerJointType.DIP) m_Joints[(int)FingerJointType.DIP].ApplyFlexData(p_Data.Raw.Flex[(int)m_Type].PIPFlex); //FAKE, allow future support like this
		}
	}
}
