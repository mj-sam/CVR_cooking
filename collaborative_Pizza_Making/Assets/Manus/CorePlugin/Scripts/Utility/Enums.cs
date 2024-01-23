using Hermes.Protocol;
/// <summary>
/// Manus Utility enumerators
/// </summary>
namespace Manus.Utility
{
	/// <summary>
	/// This enum is used to define certain Unity update moments.
	/// </summary>
	public enum UpdateMoment
	{
		Update, /*!< Unity's Update call */
		FixedUpdate, /*!< Unity's FixedUpdate call */
		LateUpdate /*!< Unity's LateUpdate call */
	}

	/// <summary>
	/// Finger types used by the animator and hand data.
	/// </summary>
    public enum FingerType
    {
        Thumb = 0, /*!< A thumb */
        Index, /*!< An index finger */
        Middle, /*!< A middle finger */
        Ring, /*!< A ring finger */
        Pinky, /*!< A pinky */
        Invalid /*!< An invalid or undefined finger */
    }

	/// <summary>
	/// Hand types used by the animator and hand data.
	/// </summary>
	public enum HandType
    {
        LeftHand, /*!< A left hand */
		RightHand, /*!< A right hand */
		Invalid /*!< An invalid or undefined hand */
	}

	/// <summary>
	/// Finger joint types used by the animator and hand data.
	/// </summary>
	public enum FingerJointType
	{
		CMC = 0, /*!< The carpometacarpal joint */
		MCP, /*!< The metacarpophalangeal joint */
		PIP, /*!< The proximal interphalangeal joint */
		DIP, /*!< The distal interphalangeal joint */
		Invalid /*!< An invalid or undefined joint */
	}

	/// <summary>
	/// Is used to determine which data is available in the hand data.
	/// </summary>
	[System.Flags]
	public enum ValidFingerJointData
	{
		None = 0, /*!< No valid data */
		All = -1, /*!< All data is valid */
		Flex = 1 << 0, /*!< Flex data available */
		Position = 1 << 1, /*!< Positional data available */
		Rotation = 1 << 2, /*!< Rotational data available */
		Stretch = 1 << 0, /*!< Stretch data available */
		Spread = 1 << 3, /*!< Spread data available */
	}

	/// <summary>
	/// The types of VR Trackers supported.
	/// </summary>
	public enum VRTrackerType
    {
        Other, /*!< An undefined/other tracker */
		LeftHand, /*!< A left hand tracker */
		RightHand, /*!< A right hand tracker */
		LeftFoot, /*!< A left foot tracker */
		RightFoot, /*!< A right foot tracker */
		Waist, /*!< A waist tracker */
		Head, /*!< A head tracker */
		LeftUpperArm,
		RightUpperArm,
		Controller, /*!< A controller tracker */
		LightHouse,
		Gamepad, /*!< A gamepad tracker */
		Camera, /*!< A camera tracker */
		Keyboard, /*!< A keyboard tracker */
		Treadmill, /*!< A treadmill tracker */
		Max, /*!< The max amount of tracker types */
	}

	/// <summary>
	/// Points on the body used by Polygon
	/// </summary>
    public enum BodyPoints
    {
		Head, /*!< A head */
		Waist, /*!< A waist */
		LeftHand, /*!< A left hand */
		RightHand, /*!< A right hand */
		LeftFoot, /*!< A left foot */
		RightFoot /*!< A right foot */
	}

	/// <summary>
	/// Polygon offset to trackers enum
	/// </summary>
    public enum OffsetsToTrackers
	{
		HeadTrackerToHead,
		LeftHandTrackerToWrist,
		RightHandTrackerToWrist,
		LeftElbowTrackerToElbow,
		RightElbowTrackerToElbow,
		LeftElbowTrackerToShoulder,
		RightElbowTrackerToShoulder,
		HipTrackerToHip,
		HipTrackerToLeftLeg,
		HipTrackerToRightLeg,
		LeftFootTrackerToAnkle,
		RightFootTrackerToAnkle
	}

	/// <summary>
	/// The three axes in 3d space
	/// </summary>
	public enum Axis
	{
		X, /*!< The x axis */
		Y, /*!< The y axis */
		Z /*!< The z axis */
	}

	public static class EnumConversion
	{
		public static VRTrackerType ToManusType(this TrackerType p_Type)
		{
			switch (p_Type)
			{
				case TrackerType.Head:
					return VRTrackerType.Head;
				case TrackerType.Waist:
					return VRTrackerType.Waist;
				case TrackerType.LeftHand:
					return VRTrackerType.LeftHand;
				case TrackerType.RightHand:
					return VRTrackerType.RightHand;
				case TrackerType.LeftFoot:
					return VRTrackerType.LeftFoot;
				case TrackerType.RightFoot:
					return VRTrackerType.RightFoot;
				case TrackerType.LeftUpperArm:
					return VRTrackerType.LeftUpperArm;
				case TrackerType.RightUpperArm:
					return VRTrackerType.RightUpperArm;
				case TrackerType.Controller:
					return VRTrackerType.Controller;
				case TrackerType.Camera:
					return VRTrackerType.Camera;
				case TrackerType.LeftUpperLeg:
				case TrackerType.RightUpperLeg:
				default:
				case TrackerType.Unknown:
					return VRTrackerType.Other;
			}
		}
	}
}