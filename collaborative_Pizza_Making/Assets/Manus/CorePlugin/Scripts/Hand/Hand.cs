using UnityEngine;

namespace Manus.Hand
{
	/// <summary>
	/// This is the class which needs to be on every hand in order for all the other hand related components to function correctly.
	/// This component is used to connect the data from hermes to a Hand in Unity.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Hand/Hand")]
	public class Hand : MonoBehaviour
	{
		private int m_LastUserIndex = 0;
		private Utility.HandType m_LastType = 0;

		/// <summary>
		/// The hand's current state, usually created from Hermes' data.
		/// </summary>
		public Hermes.Glove.Data data { get; set; }

		/// <summary>
		/// The hand type used by several components, most importantly the CommunicationHub uses this to
		/// identify what hand data needs to be applied to this hand.
		/// In order to get data from Hermes correctly this should either be Utility.HandType.Left or Utility.HandType.Right.
		/// </summary>
		public Utility.HandType type = Utility.HandType.Invalid;

		/// <summary>
		/// Which Hand to look at, index 0 is the first hand the communicationHub contains of the defined type to use.
		/// </summary>
		public int userIndex;

		/// <summary>
		/// If the hands are synced over the Network, this bool will be set to false if they do not belong to this instance of Unity.
		/// </summary>
		public bool isLocalPlayer = true;

		[Header("Transform")]
		/// <summary>
		/// If this is true, use the calibrated hand position from Manus Core
		/// </summary>
		public bool usePositionalData = true;

		/// <summary>
		/// Offset applied to the calibrated position of the hand in cm
		/// </summary>
		public Vector3 positionOffset = new Vector3(0,0,0);

		/// <summary>
		/// Offset applied to the calibrated rotation of the hand in degrees
		/// </summary>
		public Vector3 rotationOffset;

		[HideInInspector] public Vector3 trackerPosition;
		[HideInInspector] public Quaternion trackerRotation;

		/// <summary>
		/// Called by Unity.
		/// Adds the Hand to the CommunicationHub if possible.
		/// </summary>
		private void OnEnable()
		{
			if (userIndex < 0)
				return;

			RegisterHandAtCommunicationHub();
		}

		/// <summary>
		/// Called by Unity.
		/// Removes the Hand from the CommunicationHub if possible.
		/// </summary>
		private void OnDisable()
		{
			if (userIndex < 0)
				return;

			UnregisterHandAtCommunicationHub();
		}

		private void Update()
		{
			if (usePositionalData && data != null)
			{
				var t_Transform = data.GetWristTransform(userIndex);
				transform.localPosition = trackerPosition + trackerRotation * (t_Transform.position + positionOffset / 100f);
				transform.localRotation = trackerRotation * t_Transform.rotation * Quaternion.Euler(rotationOffset);
			}

			if (type != m_LastType || userIndex != m_LastUserIndex)
			{
				if (UnregisterHandAtCommunicationHub())
					RegisterHandAtCommunicationHub();
			}

			//TestHapticsWithFist(); //Uncomment this to apply the haptics data depending on how much of a fist is being made.
		}

		/// <summary>
		/// Adds the Hand to the Hands available in the CommunicationHub.
		/// </summary>
		/// <returns>True if it the hand has been added to the CommunicationHub.</returns>
		public bool RegisterHandAtCommunicationHub()
		{
			if (ManusManager.instance == null) return false;
			if (!ManusManager.instance.HasCommunicationHub()) return false;
			if (type == Utility.HandType.Invalid)
			{
				Debug.LogError("Hand on: " + name + " has Hand Type Invalid and will not be added to tracked hands!");
				return false;
			}

			m_LastType = type;
			m_LastUserIndex = userIndex;

			ManusManager.instance.communicationHub.RegisterHand(this);
			return true;
		}

		/// <summary>
		/// Removes the Hand from the Hands available in the CommunicationHub.
		/// </summary>
		public bool UnregisterHandAtCommunicationHub()
		{
			if (ManusManager.instance == null) return false;
			if (!ManusManager.instance.HasCommunicationHub()) return false;
			ManusManager.instance.communicationHub.UnregisterHand(this);
			return true;
		}

		//Uncomment this to evaluate all the gestures defined by the ManusManager's settings
		/*private void Update()
		{
			for(int i = 0; i < ManusManager.instance.settings.gestures.Length; i++)
			{
				Debug.Log(ManusManager.instance.settings.gestures[i].name + ManusManager.instance.settings.gestures[i].Evaluate(this));
			}
		}*/

		private void TestHapticsWithFist()
		{
			if (data == null) return;
			for (int i = 0; i < 5; i++)
			{
				float t_Hapt = data.GetFinger(i).GetJoint(Utility.FingerJointType.MCP).flex * data.GetFinger(i).GetJoint(Utility.FingerJointType.PIP).flex;
				data.SetFingerHaptic(i, t_Hapt);
			}
		}
	}
}
