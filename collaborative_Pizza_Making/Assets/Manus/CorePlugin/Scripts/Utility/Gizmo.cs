using UnityEngine;

namespace Manus.Utility
{
	/// <summary>
	/// Gizmos are used to give visual debugging or setup aid in the Scene view.
	/// </summary>
	public static class Gizmo
	{
		static float[] s_UnitCircle_X = {  0.0f,  0.174f,  0.342f,  0.500f,  0.643f,  0.766f,  0.866f,  0.940f,  0.985f,
											 1.0f,  0.985f,  0.940f,  0.866f,  0.766f,  0.642f,  0.500f,  0.342f,  0.174f,
											 0.0f, -0.174f, -0.342f, -0.500f, -0.643f, -0.766f, -0.866f, -0.940f, -0.985f,
											-1.0f, -0.985f, -0.940f, -0.866f, -0.766f, -0.642f, -0.500f, -0.342f, -0.174f};
		static float[] s_UnitCircle_Y = {  1.0f,  0.985f,  0.940f,  0.866f,  0.766f,  0.642f,  0.500f,  0.342f,  0.174f,
											 0.0f, -0.174f, -0.342f, -0.500f, -0.643f, -0.766f, -0.866f, -0.940f, -0.985f,
											-1.0f, -0.985f, -0.940f, -0.866f, -0.766f, -0.642f, -0.500f, -0.342f, -0.174f,
											 0.0f,  0.174f,  0.342f,  0.500f,  0.643f,  0.766f,  0.866f,  0.940f,  0.985f};

		/// <summary>
		/// Draws a 2D circle on the XZ plane
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Radius"></param>
		/// <param name="p_Color"></param>
		static public void DrawCircle(Vector3 p_Position, float p_Radius, Color p_Color)
		{
			Gizmos.color = p_Color;
			Vector3 t_PosA, t_PosB;
			for (int i = 0; i < 35; i++)
			{
				t_PosA = p_Position + (new Vector3(s_UnitCircle_X[i], 0.0f, s_UnitCircle_Y[i]) * p_Radius);
				t_PosB = p_Position + (new Vector3(s_UnitCircle_X[i + 1], 0.0f, s_UnitCircle_Y[i + 1]) * p_Radius);
				Gizmos.DrawLine(t_PosA, t_PosB);
			}
			t_PosA = p_Position + (new Vector3(s_UnitCircle_X[35], 0.0f, s_UnitCircle_Y[35]) * p_Radius);
			t_PosB = p_Position + (new Vector3(s_UnitCircle_X[0], 0.0f, s_UnitCircle_Y[0]) * p_Radius);
			Gizmos.DrawLine(t_PosA, t_PosB);
		}

		static Vector3[] s_Unit2DStar = {
									new Vector3(-0.5f, 0.0f, 0.0f), new Vector3(0.5f, 0.0f, 0.0f),
									new Vector3(0.0f, 0.0f, -0.5f), new Vector3(0.0f, 0.0f, 0.5f),
									new Vector3(0.3535f, 0.0f, -0.3535f), new Vector3(-0.3535f, 0.0f, 0.3535f),
									new Vector3(0.3535f, 0.0f, 0.3535f), new Vector3(-0.3535f, 0.0f, -0.3535f),
								};

		/// <summary>
		/// Draws a 2D star on the XZ plane.
		/// This Gizmo is unit size (a diameter of 1 meter), scaled by the input scale.
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Scale"></param>
		/// <param name="p_Color"></param>
		static public void Draw2DStar(Vector3 p_Position, float p_Scale, Color p_Color)
		{
			Gizmos.color = p_Color;
			Vector3 t_PosA, t_PosB;
			for (int i = 0; i < s_Unit2DStar.Length / 2; i++)
			{
				t_PosA = p_Position + (s_Unit2DStar[i * 2] * p_Scale);
				t_PosB = p_Position + (s_Unit2DStar[(i * 2) + 1] * p_Scale);
				Gizmos.DrawLine(t_PosA, t_PosB);
			}
		}

		static Vector3[] s_Unit3DStar = {
									new Vector3(-0.5f, 0.0f, 0.0f), new Vector3(0.5f, 0.0f, 0.0f),
									new Vector3(0.0f, 0.0f, -0.5f), new Vector3(0.0f, 0.0f, 0.5f),
									new Vector3(0.0f, -0.5f, 0.0f), new Vector3(0.0f, 0.5f, 0.0f),

									new Vector3(0.3535f, 0.0f, -0.3535f), new Vector3(-0.3535f, 0.0f, 0.3535f),
									new Vector3(0.3535f, 0.0f, 0.3535f), new Vector3(-0.3535f, 0.0f, -0.3535f),
									new Vector3(0.0f, 0.3535f, -0.3535f), new Vector3(0.0f, -0.3535f, 0.3535f),
									new Vector3(0.0f, 0.3535f, 0.3535f), new Vector3(0.0f, -0.3535f, -0.3535f),
									new Vector3(0.3535f, -0.3535f, 0.0f), new Vector3(-0.3535f, 0.3535f, 0.0f),
									new Vector3(0.3535f, 0.3535f, 0.0f), new Vector3(-0.3535f, -0.3535f, 0.0f),
								};

		/// <summary>
		/// Draws a 3D star.
		/// This Gizmo is unit size (a diameter of 1 meter), scaled by the input scale.
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Scale"></param>
		/// <param name="p_Color"></param>
		static public void Draw3DStar(Vector3 p_Position, float p_Scale, Color p_Color)
		{
			Gizmos.color = p_Color;
			Vector3 t_PosA, t_PosB;
			for (int i = 0; i < s_Unit3DStar.Length / 2; i++)
			{
				t_PosA = p_Position + (s_Unit3DStar[i * 2] * p_Scale);
				t_PosB = p_Position + (s_Unit3DStar[(i * 2) + 1] * p_Scale);
				Gizmos.DrawLine(t_PosA, t_PosB);
			}
		}

		/// <summary>
		/// Draws a line from point A to Point B.
		/// Height is used to determine the offset from the points which will be drawn before making a connection.
		/// </summary>
		/// <param name="p_PointA"></param>
		/// <param name="p_PointB"></param>
		/// <param name="p_Height"></param>
		/// <param name="p_Color"></param>
		static public void DrawLineConnection(Vector3 p_PointA, Vector3 p_PointB, float p_Height, Color p_Color)
		{
			Gizmos.color = p_Color;
			Gizmos.DrawLine(p_PointA, p_PointA + new Vector3(0.0f, p_Height, 0.0f));
			Gizmos.DrawLine(p_PointA + new Vector3(0.0f, p_Height, 0.0f), p_PointB + new Vector3(0.0f, p_Height, 0.0f));
			Gizmos.DrawLine(p_PointB + new Vector3(0.0f, p_Height, 0.0f), p_PointB);
		}

		/// <summary>
		/// Draws a line with a directional arrow from point A to Point B.
		/// Height is used to determine the offset from the points which will be drawn before making a connection.
		/// </summary>
		/// <param name="p_PointA"></param>
		/// <param name="p_PointB"></param>
		/// <param name="p_Height"></param>
		/// <param name="p_ArrowScale"></param>
		/// <param name="p_Color"></param>
		static public void DrawDirectionalLineConnection(Vector3 p_PointA, Vector3 p_PointB, float p_Height, float p_ArrowScale, Color p_Color)
		{
			Gizmos.color = p_Color;
			Vector3 t_PointElevatedA = p_PointA + new Vector3(0.0f, p_Height, 0.0f);
			Vector3 t_PointElevatedB = p_PointB + new Vector3(0.0f, p_Height * 0.5f, 0.0f);

			Vector3 t_Direction = t_PointElevatedB - t_PointElevatedA;
			Vector3 t_TPivot = t_PointElevatedA + (t_Direction / 2.0f);

			t_Direction *= 0.5f;

			Quaternion t_Quat = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, 1.0f), t_Direction.normalized);
			Vector3 t_PA = t_Quat * new Vector3(0.25f, 0.0f, -0.5f) * p_ArrowScale;
			Vector3 t_PB = t_Quat * new Vector3(-0.25f, 0.0f, -0.5f) * p_ArrowScale;
			Vector3 t_PC = t_Quat * new Vector3(0.0f, 0.25f, -0.5f) * p_ArrowScale;
			Vector3 t_PD = t_Quat * new Vector3(0.0f, -0.25f, -0.5f) * p_ArrowScale;

			Gizmos.DrawLine(t_TPivot, t_TPivot + t_PA);
			Gizmos.DrawLine(t_TPivot, t_TPivot + t_PB);
			Gizmos.DrawLine(t_TPivot, t_TPivot + t_PC);
			Gizmos.DrawLine(t_TPivot, t_TPivot + t_PD);

			Gizmos.DrawLine(p_PointA, t_PointElevatedA);
			Gizmos.DrawLine(t_PointElevatedA, t_PointElevatedB);
			Gizmos.DrawLine(t_PointElevatedB, p_PointB);
		}

		/// <summary>
		/// Draws a line with directional arrows from point A to Point B.
		/// Height is used to determine the offset from the points which will be drawn before making a connection.
		/// </summary>
		/// <param name="p_PointA"></param>
		/// <param name="p_PointB"></param>
		/// <param name="p_Height"></param>
		/// <param name="p_ArrowScale">The size of the arrow</param>
		/// <param name="p_ArrowDist">Distance between the arrows</param>
		/// <param name="p_Color"></param>
		static public void DrawDirectionalLineMultiArrowConnection(Vector3 p_PointA, Vector3 p_PointB, float p_Height, float p_ArrowScale, float p_ArrowDist, Color p_Color)
		{
			Gizmos.color = p_Color;
			Vector3 t_PointElevatedA = p_PointA + new Vector3(0.0f, p_Height, 0.0f);
			Vector3 t_PointElevatedB = p_PointB + new Vector3(0.0f, p_Height * 0.5f, 0.0f);

			Vector3 t_Direction = t_PointElevatedB - t_PointElevatedA;
			int t_ArrowCount = (int)(t_Direction.magnitude / p_ArrowDist);

			t_Direction = t_Direction / (t_ArrowCount + 1);

			Vector3 t_TPivot;
			Quaternion t_Quat = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, 1.0f), t_Direction.normalized);
			Vector3 t_PA = t_Quat * new Vector3(0.25f, 0.0f, -0.5f) * p_ArrowScale;
			Vector3 t_PB = t_Quat * new Vector3(-0.25f, 0.0f, -0.5f) * p_ArrowScale;
			Vector3 t_PC = t_Quat * new Vector3(0.0f, 0.25f, -0.5f) * p_ArrowScale;
			Vector3 t_PD = t_Quat * new Vector3(0.0f, -0.25f, -0.5f) * p_ArrowScale;
			for (int i = 0; i < t_ArrowCount; i++)
			{
				t_TPivot = t_PointElevatedA + t_Direction * ((float)i + 1);
				Gizmos.DrawLine(t_TPivot, t_TPivot + t_PA);
				Gizmos.DrawLine(t_TPivot, t_TPivot + t_PB);
				Gizmos.DrawLine(t_TPivot, t_TPivot + t_PC);
				Gizmos.DrawLine(t_TPivot, t_TPivot + t_PD);
			}

			Gizmos.DrawLine(p_PointA, t_PointElevatedA);
			Gizmos.DrawLine(t_PointElevatedA, t_PointElevatedB);
			Gizmos.DrawLine(t_PointElevatedB, p_PointB);
		}

		static Vector3[] s_Unit2DArrow = {
									new Vector3(0.0f, 0.0f, 0.5f),
									new Vector3(0.5f, 0.0f, 0.0f),
									new Vector3(0.2f, 0.0f, 0.0f),
									new Vector3(0.2f, 0.0f, -0.5f),

									new Vector3(-0.2f, 0.0f, -0.5f),
									new Vector3(-0.2f, 0.0f, 0.0f),

									new Vector3(-0.5f, 0.0f, 0.0f),
									new Vector3(0.0f, 0.0f, 0.5f),
								};

		/// <summary>
		/// A 2D arrow on the XZ plane
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Rotation"></param>
		/// <param name="p_Scale"></param>
		/// <param name="p_Color"></param>
		static public void Draw2DArrow(Vector3 p_Position, Quaternion p_Rotation, float p_Scale, Color p_Color)
		{
			Gizmos.color = p_Color;
			Vector3 t_PosA, t_PosB;
			for (int i = 0; i < s_Unit2DArrow.Length - 1; i++)
			{
				t_PosA = p_Position + (p_Rotation * (s_Unit2DArrow[i] * p_Scale));
				t_PosB = p_Position + (p_Rotation * (s_Unit2DArrow[i + 1] * p_Scale));
				Gizmos.DrawLine(t_PosA, t_PosB);
			}
		}

		static Vector3[] s_Unit2DThinArrow = {
									new Vector3(0.0f, 0.0f, 0.5f),
									new Vector3(0.25f, 0.0f, 0.0f),
									new Vector3(0.0f, 0.0f, 0.5f),
									new Vector3(-0.25f, 0.0f, 0.0f),
									new Vector3(0.0f, 0.0f, 0.5f),
									new Vector3(0.0f, 0.0f, -0.5f),
								};

		/// <summary>
		/// Draw a 2D arrow in 3D space at a given position and orientation
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Rotation"></param>
		/// <param name="p_Scale"></param>
		/// <param name="p_Color"></param>
		static public void Draw2DThinArrow(Vector3 p_Position, Quaternion p_Rotation, float p_Scale, Color p_Color)
		{
			Gizmos.matrix = Matrix4x4.TRS(p_Position, p_Rotation, Vector3.one * p_Scale);
			Gizmos.color = p_Color;
			Vector3 t_PosA, t_PosB;
			for (int i = 0; i < s_Unit2DThinArrow.Length / 2; i++)
			{
				t_PosA = s_Unit2DThinArrow[(i * 2)];
				t_PosB = s_Unit2DThinArrow[(i * 2) + 1];
				Gizmos.DrawLine(t_PosA, t_PosB);
			}
			Gizmos.matrix = Matrix4x4.identity;
		}

		/// <summary>
		/// Draw a 3D cube in 3D space.
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Rotation"></param>
		/// <param name="p_Scale"></param>
		/// <param name="p_Color"></param>
		/// <param name="p_Wireframe"></param>
		/// <param name="p_FillColor"></param>
		/// <param name="p_Fill"></param>
		static public void Draw3DCube(Vector3 p_Position, Quaternion p_Rotation, Vector3 p_Scale, Color p_Color, bool p_Wireframe = true, Color p_FillColor = new Color(), bool p_Fill = false)
		{
			Gizmos.matrix = Matrix4x4.TRS(p_Position, p_Rotation, p_Scale);
			if (p_Fill)
			{
				Gizmos.color = p_FillColor;
				Gizmos.DrawCube(Vector3.zero, Vector3.one);
			}
			if (p_Wireframe)
			{
				Gizmos.color = p_Color;
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
			Gizmos.matrix = Matrix4x4.identity;
		}

		/// <summary>
		/// Draw a 3D cube in 3D space using the Min and Max corners.
		/// </summary>
		/// <param name="p_Min"></param>
		/// <param name="p_Max"></param>
		/// <param name="p_Color"></param>
		/// <param name="p_Wireframe"></param>
		/// <param name="p_FillColor"></param>
		/// <param name="p_Fill"></param>
		static public void Draw3DCube(Vector3 p_Min, Vector3 p_Max, Color p_Color, bool p_Wireframe = true, Color p_FillColor = new Color(), bool p_Fill = false)
		{
			Vector3 t_Position = (p_Min + p_Max) / 2.0f;
			Vector3 t_Size = p_Max - p_Min;
			if (p_Fill)
			{
				Gizmos.color = p_FillColor;
				Gizmos.DrawCube(t_Position, t_Size);
			}
			if (p_Wireframe)
			{
				Gizmos.color = p_Color;
				Gizmos.DrawWireCube(t_Position, t_Size);
			}
		}

		/// <summary>
		/// Draw a Sphere in 3D space.
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Radius"></param>
		/// <param name="p_Color"></param>
		/// <param name="p_Wireframe"></param>
		/// <param name="p_FillColor"></param>
		/// <param name="p_Fill"></param>
		static public void DrawSphere(Vector3 p_Position, float p_Radius, Color p_Color, bool p_Wireframe = true, Color p_FillColor = new Color(), bool p_Fill = false)
		{
			if (p_Fill)
			{
				Gizmos.color = p_FillColor;
				Gizmos.DrawSphere(p_Position, p_Radius);
			}
			if (p_Wireframe)
			{
				Gizmos.color = p_Color;
				Gizmos.DrawWireSphere(p_Position, p_Radius);
			}
		}

		/// <summary>
		/// Draw a text bubble in 3D space.
		/// </summary>
		/// <param name="p_Position"></param>
		/// <param name="p_Rotation"></param>
		/// <param name="p_Scale"></param>
		/// <param name="p_Color"></param>
		/// <param name="p_Text"></param>
		/// <param name="p_TextColor"></param>
		static public void DrawTextBubble(Vector3 p_Position, Quaternion p_Rotation, Vector3 p_Scale, Color p_Color, bool p_Text = false, Color p_TextColor = new Color())
		{
			Gizmos.matrix = Matrix4x4.TRS(p_Position, p_Rotation, p_Scale);
			Gizmos.color = p_Color;
			Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.1f, 0.0f));
			Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-0.1f, 0.1f, 0.0f));

			Gizmos.DrawLine(new Vector3(-0.1f, 0.1f, 0.0f), new Vector3(-0.45f, 0.1f, 0.0f));
			Gizmos.DrawLine(new Vector3(0.0f, 0.1f, 0.0f), new Vector3(0.45f, 0.1f, 0.0f));

			Gizmos.DrawLine(new Vector3(0.45f, 0.1f, 0.0f), new Vector3(0.5f, 0.15f, 0.0f));
			Gizmos.DrawLine(new Vector3(-0.45f, 0.1f, 0.0f), new Vector3(-0.5f, 0.15f, 0.0f));

			Gizmos.DrawLine(new Vector3(0.5f, 0.7f, 0.0f), new Vector3(0.5f, 0.15f, 0.0f));
			Gizmos.DrawLine(new Vector3(-0.5f, 0.7f, 0.0f), new Vector3(-0.5f, 0.15f, 0.0f));

			Gizmos.DrawLine(new Vector3(-0.5f, 0.7f, 0.0f), new Vector3(-0.45f, 0.75f, 0.0f));
			Gizmos.DrawLine(new Vector3(0.5f, 0.7f, 0.0f), new Vector3(0.45f, 0.75f, 0.0f));

			Gizmos.DrawLine(new Vector3(-0.45f, 0.75f, 0.0f), new Vector3(0.45f, 0.75f, 0.0f));

			if (p_Text)
			{
				Gizmos.color = p_TextColor;
				Gizmos.DrawLine(new Vector3(0.4f, 0.5f, 0.0f), new Vector3(-0.4f, 0.5f, 0.0f));
				Gizmos.DrawLine(new Vector3(0.4f, 0.65f, 0.0f), new Vector3(-0.4f, 0.65f, 0.0f));
				Gizmos.DrawLine(new Vector3(0.4f, 0.35f, 0.0f), new Vector3(-0.4f, 0.35f, 0.0f));
				Gizmos.DrawLine(new Vector3(0.4f, 0.2f, 0.0f), new Vector3(-0.4f, 0.2f, 0.0f));
			}
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
