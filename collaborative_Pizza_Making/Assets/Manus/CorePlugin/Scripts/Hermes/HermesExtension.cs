using UnityEngine;
using Protocol = Hermes.Protocol;

namespace Manus.Hermes
{
	/// <summary>
	/// Expands upon Hermes, GLM and Unity classes for conversion.
	/// </summary>
	internal static class HermesExtension
	{
		/// <summary>
		/// Converts a Protocol.Vec3 (Hermes) to a Vector3 (Unity)
		/// </summary>
		/// <param name="p_Vec"></param>
		/// <returns>A Vector3 (Unity)</returns>
		internal static Vector3 ToUnity(this Protocol.Vec3 p_Vec)
		{
			return new Vector3(p_Vec.X, p_Vec.Y, p_Vec.Z);
		}

		/// <summary>
		/// Converts a GlmSharp.vec3 (GLM) to a Vector3 (Unity)
		/// </summary>
		/// <param name="p_Vec"></param>
		/// <returns>A Vector3 (Unity)</returns>
		internal static Vector3 ToUnity(this GlmSharp.vec3 p_Vec)
		{
			return new Vector3(p_Vec.x, p_Vec.y, p_Vec.z);
		}

		/// <summary>
		/// Converts a Protocol.Translation (Hermes) to a Vector3 (Unity)
		/// </summary>
		/// <param name="p_Translation"></param>
		/// <returns>A Vector3 (Unity)</returns>
		internal static Vector3 ToUnity(this Protocol.Translation p_Translation)
		{
			if (p_Translation == null || p_Translation.Full == null)
			{
				return Vector3.zero;
			}
			return new Vector3(p_Translation.Full.X, p_Translation.Full.Y, p_Translation.Full.Z);
		}

		internal static Color ToUnity(this Protocol.Color p_Color)
		{
			return new Color(p_Color.R, p_Color.G, p_Color.B, p_Color.A);
		}

		/// <summary>
		/// Converts a Vector3 (Unity) to a Protocol.Vec3 (Hermes)
		/// </summary>
		/// <param name="p_Vector"></param>
		/// <returns>A Protocol.Vec3 (Hermes)</returns>
		internal static Protocol.Vec3 ToProto(this Vector3 p_Vector)
		{
			return new Protocol.Vec3 { X = p_Vector.x, Y = p_Vector.y, Z = p_Vector.z };
		}

		/// <summary>
		/// Converts a Vector3 (Unity) to a GlmSharp.vec3 (GLM)
		/// </summary>
		/// <param name="p_Vector"></param>
		/// <returns>A GlmSharp.vec3 (GLM)</returns>
		internal static GlmSharp.vec3 ToGlm(this Vector3 p_Vector)
		{
			return new GlmSharp.vec3 { x = p_Vector.x, y = p_Vector.y, z = p_Vector.z };
		}

		/// <summary>
		/// Converts a Protocol.Quat (Hermes) to a Quaternion (Unity)
		/// </summary>
		/// <param name="p_Quat"></param>
		/// <returns>A Quaternion (Unity)</returns>
		internal static Quaternion ToUnity(this Protocol.Quat p_Quat)
		{
			return new Quaternion(p_Quat.X, p_Quat.Y, p_Quat.Z, p_Quat.W);
		}

		/// <summary>
		/// Converts a Protocol.Orientation (Hermes) to a Quaternion (Unity)
		/// </summary>
		/// <param name="p_Orientation"></param>
		/// <returns>A Quaternion (Unity)</returns>
		internal static Quaternion ToUnity(this Protocol.Orientation p_Orientation)
		{
			if (p_Orientation == null)
			{
				return Quaternion.identity;
			}

			var t_Full = p_Orientation.Full;
			if (t_Full != null)
				return t_Full.ToUnity();

			var t_Quat = global::ManusCore.Math.Mathf.DecompressQuat(p_Orientation.Compressed);
			return new Quaternion(t_Quat.x, t_Quat.y, t_Quat.z, t_Quat.w);
		}

		/// <summary>
		/// Converts a GlmSharp.quat (GLM) to a Quaternion (Unity)
		/// </summary>
		/// <param name="p_Quat"></param>
		/// <returns>A Quaternion (Unity)</returns>
		internal static Quaternion ToUnity(this GlmSharp.quat p_Quat)
		{
			return new Quaternion { x = p_Quat.x, y = p_Quat.y, z = p_Quat.z, w = p_Quat.w };
		}

		/// <summary>
		/// Converts a Quaternion (Unity) to a Protocol.Quat (Hermes)
		/// </summary>
		/// <param name="p_Quat"></param>
		/// <returns>A Protocol.Quat (Hermes)</returns>
		internal static Protocol.Quat ToProto(this Quaternion p_Quat)
		{
			return new Protocol.Quat { X = p_Quat.x, Y = p_Quat.y, Z = p_Quat.z, W = p_Quat.w };
		}

		/// <summary>
		/// Converts a Quaternion (Unity) to a GlmSharp.quat (GLM)
		/// </summary>
		/// <param name="p_Quat"></param>
		/// <returns>A GlmSharp.quat (GLM)</returns>
		internal static GlmSharp.quat ToGlm(this Quaternion p_Quat)
		{
			return new GlmSharp.quat { x = p_Quat.x, y = p_Quat.y, z = p_Quat.z, w = p_Quat.w };
		}

		/// <summary>
		/// Converts a Protocol.Tracker (Hermes) to a VR.Tracker (Unity)
		/// </summary>
		/// <param name="p_Tracker"></param>
		/// <returns>A VR.Tracker (Unity)</returns>
		internal static VR.Tracker ToTracker(this Protocol.Tracker p_Tracker)
		{
			VR.Tracker t_Tracker = new VR.Tracker();

			switch (p_Tracker.Type)
			{
				case Protocol.TrackerType.LeftHand:
					t_Tracker.type = Utility.VRTrackerType.LeftHand;
					break;
				case Protocol.TrackerType.RightHand:
					t_Tracker.type = Utility.VRTrackerType.RightHand;
					break;
				case Protocol.TrackerType.LeftFoot:
					t_Tracker.type = Utility.VRTrackerType.LeftFoot;
					break;
				case Protocol.TrackerType.RightFoot:
					t_Tracker.type = Utility.VRTrackerType.RightFoot;
					break;
				case Protocol.TrackerType.Waist:
					t_Tracker.type = Utility.VRTrackerType.Waist;
					break;
				case Protocol.TrackerType.Head:
					t_Tracker.type = Utility.VRTrackerType.Head;
					break;
				default:
					t_Tracker.type = Utility.VRTrackerType.Other;
					break;
			}
			t_Tracker.position = p_Tracker.Position.ToUnity();
			t_Tracker.rotation = p_Tracker.Rotation.ToUnity();
			t_Tracker.trackingQuality = p_Tracker.Quality;

			return t_Tracker;
		}

		/// <summary>
		/// Converts a VR.Tracker (Unity) to a Protocol.Tracker (Hermes)
		/// </summary>
		/// <param name="p_Tracker"></param>
		/// <returns>A Hermes Protocol</returns>
		internal static Protocol.Tracker ToProto(this VR.Tracker p_Tracker)
		{
			Protocol.Tracker t_Tracker = new Protocol.Tracker();

			switch (p_Tracker.type)
			{
				case Utility.VRTrackerType.LeftHand:
					t_Tracker.Type = Protocol.TrackerType.LeftHand;
					break;
				case Utility.VRTrackerType.RightHand:
					t_Tracker.Type = Protocol.TrackerType.RightHand;
					break;
				case Utility.VRTrackerType.LeftFoot:
					t_Tracker.Type = Protocol.TrackerType.LeftFoot;
					break;
				case Utility.VRTrackerType.RightFoot:
					t_Tracker.Type = Protocol.TrackerType.RightFoot;
					break;
				case Utility.VRTrackerType.Waist:
					t_Tracker.Type = Protocol.TrackerType.Waist;
					break;
				case Utility.VRTrackerType.Head:
					t_Tracker.Type = Protocol.TrackerType.Head;
					break;
				default:
					t_Tracker.Type = Protocol.TrackerType.Unknown;
					break;
			}
			Protocol.Translation t_Trans = new Protocol.Translation();
			t_Trans.Full = p_Tracker.position.ToProto();
			t_Tracker.Position = t_Trans;
			Protocol.Orientation t_Orient = new Protocol.Orientation();
			t_Orient.Full = p_Tracker.rotation.ToProto();
			t_Tracker.Rotation = t_Orient;

			return t_Tracker;
		}
	}
}