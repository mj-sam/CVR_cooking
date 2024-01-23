using UnityEngine;

namespace Manus.Sample
{
	/// <summary>
	/// This sample shows the hand data gotten from the Hand Component.
	/// It is required to have a Hand component on either the same object as this class or on one that is a parent of this object!
	/// </summary>
	public class HandDataSample : MonoBehaviour
	{
		// Finger Data
		// Thumb
		[Header("Finger Data")]
		public float thumbCMCAbductionAdduction; //down-up
		public float thumbCMCExtensionFlexion; //bending

		public float thumbMCPExtensionFlexion; //bending
		public float thumbIPExtensionFlexion; //bending

		[Space]
		// Index Finger
		public float indexMCPAbductionAdduction; //outward-inward spreading
		public float indexMCPExtensionFlexion; //bending
		public float indexPIPExtensionFlexion; //bending
		public float indexDIPExtensionFlexion; //bending

		[Space]
		// Middle Finger
		public float middleMCPAbductionAdduction; //outward-inward spreading
		public float middleMCPExtensionFlexion; //bending
		public float middlePIPExtensionFlexion; //bending
		public float middleDIPExtensionFlexion; //bending

		[Space]
		// Ring Finger
		public float ringMCPAbductionAdduction; //outward-inward spreading
		public float ringMCPExtensionFlexion; //bending
		public float ringPIPExtensionFlexion; //bending
		public float ringDIPExtensionFlexion; //bending

		[Space]
		// Pinky Finger
		public float pinkyMCPAbductionAdduction; //outward-inward spreading
		public float pinkyMCPExtensionFlexion; //bending
		public float pinkyPIPExtensionFlexion; //bending
		public float pinkyDIPExtensionFlexion; //bending

		Hand.Hand m_Hand;

		/// <summary>
		/// Called by Unity.
		/// Locates the Hand in this gameobject or its parents.
		/// </summary>
		private void Awake()
		{
			m_Hand = GetComponentInParent<Hand.Hand>();
		}

		// Update is called once per frame
		void Update()
		{
			if (m_Hand.data == null) return;

			thumbCMCAbductionAdduction = m_Hand.data.GetFinger(Utility.FingerType.Thumb).GetJoint(Utility.FingerJointType.CMC).spreadDegrees;
			thumbCMCExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Thumb).GetJoint(Utility.FingerJointType.CMC).stretchDegrees;

			thumbMCPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Thumb).GetJoint(Utility.FingerJointType.MCP).stretchDegrees;
			thumbIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Thumb).GetJoint(Utility.FingerJointType.PIP).stretchDegrees; //IP

			indexMCPAbductionAdduction = m_Hand.data.GetFinger(Utility.FingerType.Index).GetJoint(Utility.FingerJointType.MCP).spreadDegrees;
			indexMCPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Index).GetJoint(Utility.FingerJointType.MCP).stretchDegrees;
			indexPIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Index).GetJoint(Utility.FingerJointType.PIP).stretchDegrees;
			indexDIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Index).GetJoint(Utility.FingerJointType.DIP).stretchDegrees;

			middleMCPAbductionAdduction = m_Hand.data.GetFinger(Utility.FingerType.Middle).GetJoint(Utility.FingerJointType.MCP).spreadDegrees;
			middleMCPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Middle).GetJoint(Utility.FingerJointType.MCP).stretchDegrees;
			middlePIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Middle).GetJoint(Utility.FingerJointType.PIP).stretchDegrees;
			middleDIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Middle).GetJoint(Utility.FingerJointType.DIP).stretchDegrees;

			ringMCPAbductionAdduction = m_Hand.data.GetFinger(Utility.FingerType.Ring).GetJoint(Utility.FingerJointType.MCP).spreadDegrees;
			ringMCPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Ring).GetJoint(Utility.FingerJointType.MCP).stretchDegrees;
			ringPIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Ring).GetJoint(Utility.FingerJointType.PIP).stretchDegrees;
			ringDIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Ring).GetJoint(Utility.FingerJointType.DIP).stretchDegrees;

			pinkyMCPAbductionAdduction = m_Hand.data.GetFinger(Utility.FingerType.Pinky).GetJoint(Utility.FingerJointType.MCP).spreadDegrees;
			pinkyMCPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Pinky).GetJoint(Utility.FingerJointType.MCP).stretchDegrees;
			pinkyPIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Pinky).GetJoint(Utility.FingerJointType.PIP).stretchDegrees;
			pinkyDIPExtensionFlexion = m_Hand.data.GetFinger(Utility.FingerType.Pinky).GetJoint(Utility.FingerJointType.DIP).stretchDegrees;
		}
	}
}
