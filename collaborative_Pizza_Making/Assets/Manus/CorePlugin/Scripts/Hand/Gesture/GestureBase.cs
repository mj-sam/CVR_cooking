using System;
using UnityEngine;

namespace Manus.Hand.Gesture
{
	/// <summary>
	/// This is the base class for creating Gestures that other scripts can evaluate.
	/// </summary>
	//[CreateAssetMenu(fileName = "Gesture", menuName = "ScriptableObjects/Gesture", order = 1)]
	public abstract class GestureBase : ScriptableObject
	{
		/// <summary>
		/// This function evaluates the gesture and returns True if the gesture is being made.
		/// </summary>
		/// <param name="p_Hand">The Hand to evaluate.</param>
		/// <returns>True if the gesture is made.</returns>
		public abstract bool Evaluate(Hand p_Hand);
	}
}
