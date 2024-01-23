using UnityEngine;

namespace Manus.Utility
{
	public static class QuaternionExtensions
	{
		public static bool IsValid(this Quaternion p_Rotation)
		{
			return !((Mathf.Approximately(p_Rotation.x, 0)
					 && Mathf.Approximately(p_Rotation.y, 0)
					 && Mathf.Approximately(p_Rotation.z, 0)
					 && Mathf.Approximately(p_Rotation.w, 0))
					 || float.IsNaN(p_Rotation.x)
					 || float.IsNaN(p_Rotation.y)
					 || float.IsNaN(p_Rotation.z)
					 || float.IsNaN(p_Rotation.w));
		}
	}
}