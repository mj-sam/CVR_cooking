using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.Hermes.Polygon
{
	public class PolygonData
	{
		public long time;
		public List<Skeleton> skeletons = new List<Skeleton>();

		#region Conversion

		public static explicit operator HProt.Polygon.Data(PolygonData p_Data)
		{
			var t_Data = new HProt.Polygon.Data { };

			foreach (var t_Skeleton in p_Data.skeletons)
			{
				t_Data.Skeletons.Add((HProt.Polygon.Skeleton)t_Skeleton);
			}

			return t_Data;
		}

		public static explicit operator PolygonData(HProt.Polygon.Data p_Data)
		{
			var t_Data = new PolygonData { time = p_Data.PublishTimestamp.Seconds };
			t_Data.skeletons = new List<Skeleton>();

			foreach (var t_Skeleton in p_Data.Skeletons)
			{
				t_Data.skeletons.Add((Skeleton)t_Skeleton);
			}

			return t_Data;
		}

		#endregion
	}

	public class Skeleton
	{
		public uint id;
		public int userIndex;
		public List<Bone> bones;

		public Skeleton()
		{
			bones = new List<Bone>();
		}

		#region Conversion

		public static explicit operator HProt.Polygon.Skeleton(Skeleton p_Skeleton)
		{
			var t_Skeleton = new HProt.Polygon.Skeleton { DeviceID = p_Skeleton.id, UserIndex = p_Skeleton.userIndex };

			foreach (var t_Bone in p_Skeleton.bones)
			{
				t_Skeleton.Bones.Add(t_Bone);
			}

			return t_Skeleton;
		}

		public static explicit operator Skeleton(HProt.Polygon.Skeleton p_Skeleton)
		{
			var t_Skeleton = new Skeleton { id = p_Skeleton.DeviceID, userIndex = p_Skeleton.UserIndex };

			foreach (var t_Bone in p_Skeleton.Bones)
			{
				t_Skeleton.bones.Add(t_Bone);
			}

			return t_Skeleton;
		}

		#endregion
	}

	public class Bone
	{
		public HProt.Polygon.BoneType type;
		public Vector3 position;
		public Quaternion rotation;
		public float scale;

		public Bone()
		{
			position = Vector3.zero;
			rotation = Quaternion.identity;
			scale = 1f;
		}

		#region Conversion

		public static implicit operator HProt.Polygon.Bone(Bone p_Bone)
		{
			return new HProt.Polygon.Bone()
			{
				Position = new HProt.Translation() { Full = p_Bone.position.ToProto() },
				Rotation = new HProt.Orientation() { Full = p_Bone.rotation.ToProto() },
				Scale = p_Bone.scale,
				Type = p_Bone.type
			};
		}

		public static implicit operator Bone(HProt.Polygon.Bone p_Bone)
		{
			return new Bone()
			{
				position = p_Bone.Position.ToUnity(),
				rotation = p_Bone.Rotation.ToUnity(),
				scale = p_Bone.Scale,
				type = p_Bone.Type
			};
		}

		#endregion
	}
}