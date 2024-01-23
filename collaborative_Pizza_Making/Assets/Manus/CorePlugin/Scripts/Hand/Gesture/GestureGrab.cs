using UnityEngine;
using HProt = Hermes.Protocol;
using Manus.Utility;

namespace Manus.Hand.Gesture
{
	/// <summary>
	/// This is a specifically tailored gesture for making a grab gesture.
	/// The gesture allows grabs to occur when not all fingers are bent,
	/// unlike the SimpleGesture where all fingers need to be in a certain state.
	/// </summary>
	[CreateAssetMenu(fileName = "Grab Gesture", menuName = "Manus/Grab Gesture", order = 1)]
	public class GestureGrab : GestureBase
	{
		/// <summary>
		/// Must all fingers be at least partially bent for this gesture to be realized?
		/// </summary>
		public bool allFingersMustBeAtLeastPartiallyBentForGrab = true;

		/// <summary>
		/// Is the thumb evaluated in this process?
		/// </summary>
		public bool includeThumbInBendCount = true;

		/// <summary>
		/// How many fingers need to be fully bent to realize this grab?
		/// </summary>
		[Range(1, 5)]
		public int numberOfFullyBentFingersRequiredForGrab = 3;

		/// <summary>
		/// Above what flex value are the joints considered partially bent?
		/// </summary>
		[Range(0, 1f)]
		public float valueAboveWhichFingerIsConsideredPartiallyBent = 0.5f;

		/// <summary>
		/// Above what flex value are the joints considered fully bent?
		/// </summary>
		[Range(0, 1f)]
		public float valueAboveWhichFingerIsConsideredFullyBent = 0.7f;

		/// <summary>
		/// This function evaluates the gesture and returns True if the gesture is being made.
		/// </summary>
		/// <param name="p_Hand">The Hand to evaluate.</param>
		/// <returns>True if the gesture is made.</returns>
		public override bool Evaluate(Hand p_Hand)
		{
			if (p_Hand.data == null) return false;

			int t_NumPartiallyBentFingers = 0;
			int t_NumFullyBentFingers = 0;

			for (int t_FIdx = includeThumbInBendCount ? 0 : 1; t_FIdx < (int)FingerType.Invalid; t_FIdx++)
			{
				float t_AFV =
					p_Hand.data.GetFinger(t_FIdx).GetJoint(FingerJointType.MCP).stretch
					+ p_Hand.data.GetFinger(t_FIdx).GetJoint(FingerJointType.PIP).stretch
					* 0.5f;

				if (t_AFV >= valueAboveWhichFingerIsConsideredPartiallyBent)
				{
					t_NumPartiallyBentFingers++;
				}

				if (t_AFV >= valueAboveWhichFingerIsConsideredFullyBent)
				{
					t_NumFullyBentFingers++;
				}
			}

			// Determine if a grabbing gesture is being made.
			bool t_AllFingersAtLeastPartiallyBent =
				(includeThumbInBendCount && t_NumPartiallyBentFingers >= 4)
				|| (!includeThumbInBendCount && t_NumPartiallyBentFingers == 5);
			bool t_EnoughFingersPartiallyBent =
				(allFingersMustBeAtLeastPartiallyBentForGrab && t_AllFingersAtLeastPartiallyBent)
				|| !allFingersMustBeAtLeastPartiallyBentForGrab;
			bool t_EnoughFingersFullyBent = t_NumFullyBentFingers >= numberOfFullyBentFingersRequiredForGrab;

			return t_EnoughFingersPartiallyBent && t_EnoughFingersFullyBent;
		}
	}
}
