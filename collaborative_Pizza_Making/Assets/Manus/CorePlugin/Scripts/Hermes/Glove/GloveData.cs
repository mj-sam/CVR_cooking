using Manus.Utility;
using HProt = Hermes.Protocol;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Manus.Hermes.Glove
{
	/// <summary>
	/// This class contains all the Glove/Hand Data.
	/// </summary>
	public partial class Data
	{
		#region Properties
		/// <summary>
		/// The hand's ID.
		/// </summary>
		public uint id
		{
			get
			{
				return m_ID;
			}
		}

		/// <summary>
		/// Left or Right, should not be Invalid.
		/// </summary>
		public HandType handType
		{
			get
			{
				return m_HandType;
			}
		}

		/// <summary>
		/// Contains Glove information such as battery life and transmission strength.
		/// </summary>
		public State state
		{
			get
			{
				return m_State;
			}
		}

		/// <summary>
		/// The raw data gotten from Hermes.
		/// </summary>
		public HProt.Glove data
		{
			get
			{
				return m_Data;
			}
		}

		/// <summary>
		/// The wrist rotation.
		/// </summary>
		public Quaternion wristRotation
		{
			get
			{
				return m_WristRotation;
			}
		}

		/// <summary>
		/// Which Hand to look at, index 0 is the first hand the communicationHub contains of the defined type to use.
		/// </summary>
		public int[] userIndex
		{
			get
			{
				return m_UserIndex;
			}
			set
			{
				m_UserIndex = value;
			}
		}

		#endregion

		#region Fields
		uint m_ID;
		HandType m_HandType = HandType.Invalid;

		int[] m_UserIndex;

		HProt.Glove m_Data = null;
		Finger[] m_Fingers = null;
		float[] m_FingerHaptics = null;
		State m_State = null;

		Quaternion m_WristRotation;

		Dictionary<int, Tuple<Vector3, Quaternion>> m_Transforms = new Dictionary<int, Tuple<Vector3, Quaternion>>();

		global::Hermes.Tools.CareTaker m_CareTaker = null;

		public uint hapticsID;
		#endregion

		/// <summary>
		/// Initializes the Glove.Data class with certain parameters.
		/// This ensures all of the finger data is created.
		/// </summary>
		/// <param name="p_CareTaker">The Caretaker this glove belongs to.</param>
		/// <param name="p_ID">The glove's ID.</param>
		/// <param name="p_HandType">The glove's hand type.</param>
		public Data(global::Hermes.Tools.CareTaker p_CareTaker, uint p_ID, HProt.HandType p_HandType, int[] p_UserIndex = null)
		{
			m_CareTaker = p_CareTaker;
			m_ID = p_ID;
			m_HandType = p_HandType == HProt.HandType.Left ? HandType.LeftHand : HandType.RightHand;
			m_UserIndex = p_UserIndex;
			m_Fingers = new Finger[(int)FingerType.Invalid];
			m_FingerHaptics = new float[(int)FingerType.Invalid];

			for (FingerType f = 0; f < FingerType.Invalid; f++)
			{
				m_Fingers[(int)f] = new Finger(f);
			}
		}

		/// <summary>
		/// This function applies Hermes data to the Data.
		/// </summary>
		/// <param name="p_Data">The Glove data from Hermes.</param>
		public void ApplyData(HProt.Glove p_Data)
		{
			m_Data = p_Data;

			if (m_Data.Wrist != null) m_WristRotation = m_Data.Wrist.ToUnity();

			if (m_UserIndex != null)
			{
				foreach (var t_Index in m_UserIndex)
				{
					if (m_Data.WristTransforms.ContainsKey(t_Index))
					{
						if (!m_Transforms.ContainsKey(t_Index))
							m_Transforms.Add(t_Index, new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity));

						Vector3 t_Position = Vector3.zero;
						Quaternion t_Rotation = Quaternion.identity;
						if (m_Data.WristTransforms[t_Index].Position != null) t_Position = m_Data.WristTransforms[t_Index].Position.ToUnity();
						if (m_Data.WristTransforms[t_Index].Rotation != null) t_Rotation = m_Data.WristTransforms[t_Index].Rotation.ToUnity();

						m_Transforms[t_Index] = new Tuple<Vector3, Quaternion>(t_Position, t_Rotation);
					}
				}
			}

			foreach (Finger t_Finger in m_Fingers)
			{
				t_Finger.ApplyData(m_Data);
			}
		}

		/// <summary>
		/// Applies a glove state to this glove.
		/// </summary>
		/// <param name="p_State">The State to apply.</param>
		public void ApplyState(State p_State)
		{
			m_State = p_State;
		}

		/// <summary>
		/// Gets finger data for a finger at a given index, Thumb is 0, Pinky is 4.
		/// </summary>
		/// <param name="p_Idx">The desired finger's index, Thumb is 0, Pinky is 4.</param>
		/// <returns>Finger Data.</returns>
		public Finger GetFinger(int p_Idx)
		{
			return m_Fingers[p_Idx];
		}

		/// <summary>
		/// Gets finger data for a finger of the type FingerType.
		/// </summary>
		/// <param name="p_Type">The desired FingerType.</param>
		/// <returns>Finger Data.</returns>
		public Finger GetFinger(FingerType p_Type)
		{
			return m_Fingers[(int)p_Type];
		}

		/// <summary>
		/// Rumbles the glove.
		/// </summary>
		/// <param name="p_DurationMS">The amount of milliseconds that the glove should rumble for.</param>
		/// <param name="p_Power01">The amount of power to rumble the glove with, from 0(no power) to 1(max power).</param>
		public void Rumble(float p_DurationMS = 200, float p_Power01 = 1)
		{
			if (m_CareTaker == null) return;
			m_CareTaker.Hermes.RumbleAsync(new HProt.RumbleArgs
			{
				Device = m_ID,
				Duration = (uint)p_DurationMS,
				//Power = (uint)Mathf.Clamp01(_Power01) * ushort.MaxValue
				Power = (double)Mathf.Clamp01(p_Power01) * double.MaxValue
			});
		}

		/// <summary>
		/// Sets finger haptic value for a finger at a given index, Thumb is 0, Pinky is 4.
		/// The Haptic range is from 0.0 to 1.0.
		/// </summary>
		/// <param name="p_Type">The desired FingerType.</param>
		/// <param name="p_Amount">The desired Haptic value, should be between 0.0 and 1.0.</param>
		public void SetFingerHaptic(FingerType p_Type, float p_Amount)
		{
			SetFingerHaptic((int)p_Type, p_Amount);
		}

		/// <summary>
		/// Sets finger haptic value for a finger at a given index, Thumb is 0, Pinky is 4.
		/// The Haptic range is from 0.0 to 1.0.
		/// </summary>
		/// <param name="p_Idx">The desired finger's index, Thumb is 0, Pinky is 4.</param>
		/// <param name="p_Amount">The desired Haptic value, should be between 0.0 and 1.0.</param>
		public void SetFingerHaptic(int p_Idx, float p_Amount)
		{
			if (p_Idx >= (int)FingerType.Invalid) return;
			if (p_Idx < 0) return;
			m_FingerHaptics[p_Idx] = Mathf.Clamp01(p_Amount);
		}

		/// <summary>
		/// Returns the Haptics values for the fingers.
		/// </summary>
		/// <returns>Haptic values for the fingers.</returns>
		public float[] GetHaptics()
		{
			return m_FingerHaptics;
		}

		public (Vector3 position, Quaternion rotation) GetWristTransform(int p_UserIndex)
		{
			if (m_Transforms.ContainsKey(p_UserIndex))
				return (m_Transforms[p_UserIndex].Item1, m_Transforms[p_UserIndex].Item2);

			return (Vector3.zero, Quaternion.identity);
		}
	}
}
