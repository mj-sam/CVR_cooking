using UnityEngine;
using System;

namespace Manus.Haptics
{
	/// <summary>
	/// This is the class which needs to be on every hand with haptics in order for all the other haptic related components to function correctly.
	/// In order for the haptics to function each of the fingers on the hand will need a FingerHaptics class with the correct finger type set.
	/// The FingerHaptics will generate haptic values for this class to give to the hand.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Haptics/Hand (Haptics)")]
	public class HandHaptics : MonoBehaviour
	{
		Hand.Hand m_Hand;
		FingerHaptics[] m_Fingers;

		float hapticValue = 0;
		int counter = 0;

		void Start()
		{
			m_Hand = GetComponentInParent<Hand.Hand>();
			m_Fingers = GetComponentsInChildren<FingerHaptics>();
		}

		void Update()
		{
			if (m_Hand.data == null) return;
			for (int i = 0; i < m_Fingers.Length; i++)
			{
				m_Hand.data.SetFingerHaptic(m_Fingers[i].fingerType, Math.Min(hapticValue + m_Fingers[i].GetHapticValue(),1f));
				if (hapticValue != 0) counter++;
			}
		}

		void FixedUpdate(){
			if (counter > 5){
				hapticValue = 0;
				counter = 0;
			}
		}

		public void SetHapticVibration(float value){
			hapticValue = value;
		}
	}
}
