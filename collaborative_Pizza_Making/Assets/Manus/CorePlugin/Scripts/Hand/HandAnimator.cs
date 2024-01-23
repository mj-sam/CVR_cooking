using Manus.Hermes;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Hand
{
	/// <summary>
	/// The hand animator, it uses the hand data from the Hand class to animate the hand using the stretch and spread values.
	/// When using the Hand Animator and generating the axes it is advised to have the Thumb somewhat bent inward,
	/// parallel to the other fingers, for the best results.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Manus/Hand/Hand Animator")]
	public class HandAnimator : MonoBehaviour
	{
		/// <summary>
		/// The wrist transform/bone.
		/// </summary>
		public Transform wrist;

		/// <summary>
		/// Finger Bone data
		/// </summary>
		[System.Serializable]
		public class FingerBones
		{
			/// <summary>
			/// Finger Joint Bone data
			/// </summary>
			[System.Serializable]
			public class FingerBone
			{
				public Transform transform;
				public Vector3 right;
				public Vector3 forward;
				public Vector3 up;
				public Vector2 extentStretch;
				public Vector2 extentSpread;
				public Quaternion restRotation;
				public FingerBone(Transform _Trans)
				{
					transform = _Trans;
					right = Vector3.right;
					forward = Vector3.forward;
					up = Vector3.up;
					extentStretch = Vector2.zero;
					extentSpread = Vector2.zero;
					restRotation = Quaternion.identity;
				}
			}
			public List<FingerBone> bones = new List<FingerBone>();

			public FingerBones()
			{
				for (int i = 0; i < 3; i++)
				{
					bones.Add(new FingerBone(null));
				}
			}

			public FingerBones(Transform p_Root)
			{
				bones.Add(new FingerBone(p_Root));
			}

			public void AddBone(Transform p_Trans)
			{
				bones.Add(new FingerBone(p_Trans));
			}
		}
		/// <summary>
		/// All the fingers.
		/// </summary>
		public List<FingerBones> fingers = new List<FingerBones>();

		/// <summary>
		/// The size of the gizmo.
		/// </summary>
		public float gizmoLength = 0.01f;

		/// <summary>
		/// What model type is this hand based on (not taking scaling into account)?
		/// Negative scales do NOT change the model's hand type!
		/// </summary>
		public Utility.HandType handModelType = Utility.HandType.Invalid;

		static Vector2[] s_DefaultMinMaxStretchValuesFingers = new Vector2[] { new Vector2(0.0f /*-10*/, 80.0f), new Vector2(0.0f, 100.0f), new Vector2(0.0f, 90.0f) };
		static Vector2[] s_DefaultMinMaxSpreadValuesFingers = new Vector2[] { new Vector2(-20.0f, 20.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

		static Vector2[] s_DefaultMinMaxStretchValuesThumb = new Vector2[] { new Vector2(-20.0f /*IMU*/, 25.0f /*IMU*/), new Vector2(-20.0f, 45.0f), new Vector2(-15.0f, 80.0f) };
		static Vector2[] s_DefaultMinMaxSpreadValuesThumb = new Vector2[] { new Vector2(-10.0f, 35.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

		public Hand m_Hand;

		/// <summary>
		/// Called by Unity.
		/// Locates the Hand in this gameobject or its parents.
		/// </summary>
		private void Awake()
		{
			m_Hand = GetComponentInParent<Hand>();
		}

		/// <summary>
		/// Called by Unity.
		/// Animates the hand if the data is not NULL.
		/// </summary>
		private void Update()
		{
			if (m_Hand.data == null) return;
			Animate(m_Hand.data);
		}

		/// <summary>
		/// Reset the hand to the resting position.
		/// </summary>
		public void ResetHand()
		{
			for (int f = 0; f < 5; f++)
			{
				for (int j = 0; j < 3; j++)
				{
					fingers[f].bones[j].transform.localRotation = fingers[f].bones[j].restRotation;
				}
			}
		}

		/// <summary>
		/// Get the currently applied handdata
		/// </summary>
		/// <returns>Hand data</returns>
		public Hermes.Glove.Data GetHandData()
		{
			return m_Hand.data;
		}

		/// <summary>
		/// Animates the hand according to the Data given.
		/// </summary>
		/// <param name="p_Data">The Glove data.</param>
		public void Animate(Hermes.Glove.Data p_Data)
		{
			if (p_Data == null)
				return;

			float t_Modifier = 1.0f;
			float t_HandednessModifier = 1.0f;

			if (handModelType == Utility.HandType.RightHand) t_HandednessModifier = -1.0f;

			if (wrist.lossyScale.x * wrist.lossyScale.y * wrist.lossyScale.z < 0.0f)
			{
				t_Modifier *= -1.0f;
			}

			{
				float t_Stretch = p_Data.GetFinger(Utility.FingerType.Thumb).GetJoint(Utility.FingerJointType.CMC).stretch;
				float t_Spread = p_Data.GetFinger(Utility.FingerType.Thumb).GetJoint(Utility.FingerJointType.CMC).spread;

				if (handModelType == Utility.HandType.RightHand)
				{
					t_Spread *= -1.0f;
				}

				float t_ModStretch = fingers[0].bones[0].extentStretch.x + (t_Stretch * (fingers[0].bones[0].extentStretch.y - fingers[0].bones[0].extentStretch.x));
				float t_ModSpread = fingers[0].bones[0].extentSpread.x + (t_Spread * (fingers[0].bones[0].extentSpread.y - fingers[0].bones[0].extentSpread.x));

				t_ModStretch *= t_Modifier; //mirror rotation axis

				Quaternion t_QuatStretch = Quaternion.AngleAxis(t_ModStretch, fingers[0].bones[0].right);
				Quaternion t_QuatSpread = Quaternion.AngleAxis(t_ModSpread, fingers[0].bones[0].up);

				//look into this...
				if (t_Modifier < 0)
				{
					t_QuatSpread = Quaternion.AngleAxis(t_ModSpread, Vector3.Cross(fingers[0].bones[0].right, fingers[0].bones[0].forward));
				}

				fingers[0].bones[0].transform.localRotation = fingers[0].bones[0].restRotation * t_QuatSpread * t_QuatStretch;
			}

			{
				int t_FlexJointOffset = 0;
				int t_JointIdx = 1;
				for (int f = 0; f < 5; f++)
				{
					for (; t_JointIdx < 3; t_JointIdx++)
					{
						float t_ModStretch = fingers[f].bones[t_JointIdx].extentStretch.x + (p_Data.GetFinger(f).GetJoint(t_JointIdx + t_FlexJointOffset).stretch *
							(fingers[f].bones[t_JointIdx].extentStretch.y - fingers[f].bones[t_JointIdx].extentStretch.x));

						//Change -1 to 1 to 0-1, since editor will display the full range
						float t_Spr = ((p_Data.GetFinger(f).GetJoint(t_JointIdx + t_FlexJointOffset).spread + 1.0f) * 0.5f);
						float t_ModSpread = fingers[f].bones[t_JointIdx].extentSpread.x + (t_Spr *
							(fingers[f].bones[t_JointIdx].extentSpread.y - fingers[f].bones[t_JointIdx].extentSpread.x));

						Quaternion t_QuatStretch = Quaternion.AngleAxis(t_ModStretch * t_Modifier, fingers[f].bones[t_JointIdx].right);
						Quaternion t_QuatSpread = Quaternion.AngleAxis(t_ModSpread * t_HandednessModifier * t_Modifier, fingers[f].bones[t_JointIdx].up);

						fingers[f].bones[t_JointIdx].transform.localRotation = fingers[f].bones[t_JointIdx].restRotation * (t_QuatSpread * t_QuatStretch);
					}
					t_FlexJointOffset = 1;
					t_JointIdx = 0;
				}
			}
		}

#if UNITY_EDITOR
		[ContextMenu("FindBones")]
#endif
		public void FindFingers()
		{
			fingers.Clear();

			if (wrist == null) wrist = transform;

			foreach (Transform t_Trans in wrist)
			{
				if (t_Trans.childCount != 0)
				{
					var t_FB = new FingerBones(t_Trans);
					fingers.Add(t_FB);
				}
			}
			for (int i = 0; i < fingers.Count; i++)
			{
				FindFingerBones(fingers[i]);
			}
		}

#if UNITY_EDITOR
		[ContextMenu("FindBonesThroughNames")]
#endif
		public void FindFingersViaNames()
		{
			fingers.Clear();

			if (wrist == null) wrist = transform;

			for (Utility.FingerType f = 0; f < Utility.FingerType.Invalid; f++)
			{
				foreach (Transform t_Trans in wrist)
				{
					if (t_Trans.name.ToLower().Contains(f.ToString().ToLower()))
					{
						var t_FB = new FingerBones(t_Trans);
						fingers.Add(t_FB);
					}
				}
			}
			for (int i = 0; i < fingers.Count; i++)
			{
				FindFingerBones(fingers[i]);
			}
#if UNITY_EDITOR
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
		}

#if UNITY_EDITOR
		[ContextMenu("ManualBones")]
#endif
		public void SetupManualBones()
		{
			fingers.Clear();

			if (wrist == null) wrist = transform;

			for (Utility.FingerType f = 0; f < Utility.FingerType.Invalid; f++)
			{
				fingers.Add(new FingerBones());
			}
#if UNITY_EDITOR
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
		}

		int DeepestChild(Transform p_Trans, out Transform p_DeepestChild, bool p_Furthest = true)
		{
			p_DeepestChild = p_Trans;
			if (p_Trans.childCount == 0) return 0;
			int t_Deepest = -1;
			foreach (Transform t_Child in p_Trans)
			{
				Transform t_DChild;
				int t_Res = DeepestChild(t_Child, out t_DChild);
				if (t_Deepest <= t_Res)
				{
					if (t_Deepest < t_Res)
					{
						t_Deepest = t_Res;
						p_DeepestChild = t_DChild;
					}
					else
					{
						if (p_Furthest && Vector3.Distance(p_Trans.position, p_DeepestChild.position) < Vector3.Distance(p_Trans.position, t_DChild.position))
						{
							t_Deepest = t_Res;
							p_DeepestChild = t_DChild;
						}
					}
				}
			}
			return t_Deepest + 1;
		}

		void FindFingerBones(FingerBones p_Finger)
		{
			Transform t_EndBone;
			DeepestChild(p_Finger.bones[0].transform, out t_EndBone);

			List<Transform> t_Bones = new List<Transform>();
			Transform t_Bone = t_EndBone;
			while (t_Bone != null)
			{
				t_Bones.Add(t_Bone);
				t_Bone = t_Bone.parent;
				if (t_Bone == p_Finger.bones[0].transform) break;
			}
			if (Vector3.Distance(p_Finger.bones[0].transform.position, wrist.position) < 0.001f)
			{
				p_Finger.bones.Clear();
			}
			for (int i = t_Bones.Count - 1; i > -1; i--)
			{
				p_Finger.AddBone(t_Bones[i]);
			}
		}


#if UNITY_EDITOR
		[ContextMenu("Calculate Local Axes")]
#endif
		public void CalculateAxes()
		{
			for (int f = 0; f < fingers.Count; f++)
			{
				for (int i = 0; i < fingers[f].bones.Count; i++)
				{
					fingers[f].bones[i].restRotation = fingers[f].bones[i].transform.localRotation;
				}
			}

			float t_HandnessMod = 1.0f;
			if (handModelType == Utility.HandType.RightHand) t_HandnessMod = -1.0f;

			for (int f = 1; f < 5; f++)
			{
				FingerBones t_FingerA = fingers[f];
				FingerBones t_FingerB = fingers[f == 4 ? f - 1 : f + 1];
				int t_LastBoneIdx = fingers[f].bones.Count - 1;

				float t_FMod = 1.0f;
				float t_UMod = f == 4 ? -1.0f : 1.0f;

				for (int i = 0; i < fingers[f].bones.Count; i++)
				{
					var t_BoneA = t_FingerA.bones[i];
					var t_BoneB = t_FingerA.bones[i == t_LastBoneIdx ? i - 1 : i + 1];
					var t_BoneC = t_FingerB.bones[i];
					if (i == t_LastBoneIdx)
					{
						t_FMod *= -1.0f;
						t_UMod *= -1.0f;
					}

					Vector3 t_Fwd = t_BoneB.transform.position - t_BoneA.transform.position;
					t_Fwd.Normalize();
					Vector3 t_R = t_BoneC.transform.position - t_BoneA.transform.position;
					Vector3 t_Up = Vector3.Cross(t_R, t_Fwd).normalized;

					t_Fwd = t_BoneA.transform.InverseTransformDirection(t_Fwd);
					t_Up = t_BoneA.transform.InverseTransformDirection(t_Up);

					t_Fwd *= t_FMod;
					t_Up *= t_UMod * t_HandnessMod;

					t_BoneA.forward = t_Fwd;
					t_BoneA.up = t_Up;
					t_BoneA.right = Vector3.Cross(t_Up, t_Fwd);
				}
			}

			{
				FingerBones t_FingerA = fingers[0];
				int t_LastBoneIdx = fingers[0].bones.Count - 1;

				for (int i = 0; i < fingers[0].bones.Count; i++)
				{
					var t_BoneA = t_FingerA.bones[i];
					var t_BoneB = t_FingerA.bones[i == t_LastBoneIdx ? i - 1 : i + 1];
					var t_BoneC = t_FingerA.bones[i == 0 ? i + 2 : 0];

					Vector3 t_Fwd = t_BoneB.transform.position - t_BoneA.transform.position;
					t_Fwd.Normalize();
					Vector3 t_Up = t_BoneC.transform.position - t_BoneA.transform.position;
					t_Up.Normalize();
					Vector3 t_R = Vector3.Cross(t_Fwd, t_Up).normalized;

					if (t_R.sqrMagnitude < 0.01f)
					{
						Debug.LogWarning("Thumb is too straight to calculate correct axes! Bend it inward slightly for better results!");
					}

					t_Fwd = t_BoneA.transform.InverseTransformDirection(t_Fwd);
					t_R = t_BoneA.transform.InverseTransformDirection(t_R);

					if (i == t_LastBoneIdx)
					{
						t_R *= -1.0f;
						t_Fwd *= -1.0f;
					}
					t_BoneA.forward = t_Fwd;
					t_BoneA.right = t_R;
					t_BoneA.up = Vector3.Cross(t_Fwd, t_R);
				}
			}
#if UNITY_EDITOR
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
		}


#if UNITY_EDITOR
		[ContextMenu("Set Default Limits")]
#endif
		public void SetDefaultLimits()
		{
			for (int f = 1; f < 5; f++)
			{
				for (int i = 0; i < fingers[f].bones.Count; i++)
				{
					if (i < s_DefaultMinMaxStretchValuesFingers.Length) fingers[f].bones[i].extentStretch = s_DefaultMinMaxStretchValuesFingers[i];
					if (i < s_DefaultMinMaxSpreadValuesFingers.Length) fingers[f].bones[i].extentSpread = s_DefaultMinMaxSpreadValuesFingers[i];
				}
			}

			{
				for (int i = 0; i < fingers[0].bones.Count; i++)
				{
					if (i < s_DefaultMinMaxStretchValuesThumb.Length) fingers[0].bones[i].extentStretch = s_DefaultMinMaxStretchValuesThumb[i];
					if (i < s_DefaultMinMaxSpreadValuesThumb.Length) fingers[0].bones[i].extentSpread = s_DefaultMinMaxSpreadValuesThumb[i];
				}
			}
#if UNITY_EDITOR
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
		}

		private void OnDrawGizmosSelected()
		{
			if (wrist == null) return;
			Gizmos.color = Color.yellow;
			DrawBone(wrist);


			Gizmos.color = Color.green;
			for (int f = 0; f < fingers.Count; f++)
			{
				for (int i = 0; i < fingers[f].bones.Count; i++)
				{
					if (fingers[f].bones[i].transform == null) continue;
					var t_Bone = fingers[f].bones[i];
					Vector3 t_Pos = t_Bone.transform.position;
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(t_Pos, t_Pos + t_Bone.transform.TransformDirection(t_Bone.forward) * gizmoLength);

					Gizmos.color = Color.green;
					Gizmos.DrawLine(t_Pos, t_Pos + t_Bone.transform.TransformDirection(t_Bone.up) * gizmoLength);

					Gizmos.color = Color.red;
					Gizmos.DrawLine(t_Pos, t_Pos + t_Bone.transform.TransformDirection(t_Bone.right) * gizmoLength);
				}
			}
		}

		void DrawBone(Transform p_Trans)
		{
			Gizmos.DrawWireSphere(p_Trans.position, 0.25f * gizmoLength);
			for (int i = 0; i < p_Trans.childCount; i++)
			{
				Gizmos.DrawLine(p_Trans.position, p_Trans.GetChild(i).position);
				DrawBone(p_Trans.GetChild(i));
			}
		}
	}
}

#if UNITY_EDITOR
namespace Manus.Editor.Core.Hand
{
	using UnityEditor;
	using HAnimator = Manus.Hand.HandAnimator;
	using HAFingerBones = Manus.Hand.HandAnimator.FingerBones;
	using HAFingerBone = Manus.Hand.HandAnimator.FingerBones.FingerBone;

	[CustomEditor(typeof(HAnimator))]
	[CanEditMultipleObjects]
	public class HandAnimatorEditor : Editor
	{
		UnityEditor.IMGUI.Controls.JointAngularLimitHandle m_Handle = new UnityEditor.IMGUI.Controls.JointAngularLimitHandle();
		bool m_Setup = false;

		Hermes.Glove.Data m_AnimationData = null;
		Hermes.Glove.Data m_AnimationDataRest;
		Hermes.Glove.Data m_AnimationDataFist;

		private void OnEnable()
		{
			Debug.Log("Enabled");
			HAnimator t_Target = (target as HAnimator);
			m_AnimationDataFist = new Hermes.Glove.Data(null, 0,
				t_Target.handModelType == Utility.HandType.LeftHand ?
				global::Hermes.Protocol.HandType.Left : global::Hermes.Protocol.HandType.Right);

			int t_JointMax = 3;
			int t_JointOffset = 0;
			for (int f = 0; f < 5; f++)
			{
				for (int j = t_JointOffset; j < t_JointMax; j++)
				{
					m_AnimationDataFist.GetFinger(f).GetJoint(j).ApplyStretchData(1.0f);
					m_AnimationDataFist.GetFinger(f).GetJoint(j).ApplySpreadData(0.0f);
				}
				t_JointMax = 4;
				t_JointOffset = 1;
			}

			m_AnimationDataRest = new Hermes.Glove.Data(null, 0,
				t_Target.handModelType == Utility.HandType.LeftHand ?
				global::Hermes.Protocol.HandType.Left : global::Hermes.Protocol.HandType.Right);

			t_JointMax = 3;
			t_JointOffset = 0;
			for (int f = 0; f < 5; f++)
			{
				for (int j = t_JointOffset; j < t_JointMax; j++)
				{
					m_AnimationDataRest.GetFinger(f).GetJoint(j).ApplyStretchData(0.0f);
					m_AnimationDataRest.GetFinger(f).GetJoint(j).ApplySpreadData(0.0f);
				}
				t_JointMax = 4;
				t_JointOffset = 1;
			}
		}

		private void OnDestroy()
		{
			Debug.Log("Destroyed");
			HAnimator t_Target = (target as HAnimator);
			if (t_Target.fingers.Count == 0) return;
			t_Target.ResetHand();
		}

		private void UpdateAnimation()
		{
			HAnimator t_Target = (target as HAnimator);

			if (t_Target.fingers.Count == 0) return;
			if (m_AnimationData == null)
			{
				t_Target.ResetHand();
				return;
			}
			t_Target.Animate(m_AnimationData);
		}

		public void OnSceneGUI()
		{
			HAnimator t_Target = (target as HAnimator);

			if (t_Target.wrist == null) return;

			Quaternion t_MirrorRot = Quaternion.Euler(0, 0, 180);
			if (t_Target.wrist.lossyScale.x * t_Target.wrist.lossyScale.y * t_Target.wrist.lossyScale.z < 0.0f)
			{
				t_MirrorRot = Quaternion.Euler(0, 0, 0);
			}

			for (int f = 0; f < t_Target.fingers.Count; f++)
			{
				for (int i = 0; i < t_Target.fingers[f].bones.Count; i++)
				{
					if (t_Target.fingers[f].bones[i].transform == null) continue;
					var t_Bone = t_Target.fingers[f].bones[i];

					EditorGUI.BeginChangeCheck();
					Quaternion t_LRot = Quaternion.LookRotation(t_Bone.forward, t_Bone.up);

					Quaternion t_PLFR = t_Bone.restRotation * t_LRot;
					Vector3 t_GFwd = t_Bone.transform.parent.TransformDirection(t_PLFR * Vector3.forward);
					Vector3 t_GUp = t_Bone.transform.parent.TransformDirection(t_PLFR * Vector3.up);

					Quaternion t_GRot = Quaternion.LookRotation(t_GFwd, t_GUp);

					//Quaternion t_NewRot = Handles.Disc(t_GRot, t_Bone.transform.position, t_GFwd, 0.9f * t_Target.gizmoLength, false, 1);
					Quaternion t_NewRot;

					float t_Size = HandleUtility.GetHandleSize(t_Bone.transform.position);

					// set the handle matrix to match the object's position/rotation with a uniform scale
					Matrix4x4 t_HMatrix = Matrix4x4.TRS(
						 t_Bone.transform.position,
						Quaternion.identity,
						(Vector3.one / t_Size) * t_Target.gizmoLength * 0.8f
					);

					using (new Handles.DrawingScope(t_HMatrix))
					{
						t_NewRot = Handles.RotationHandle(t_GRot, Vector3.zero);
					}

					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target, "Joint Axes Rotated");

						Vector3 t_PFwd = t_Bone.transform.parent.InverseTransformDirection(t_NewRot * Vector3.forward);
						Vector3 t_PUp = t_Bone.transform.parent.InverseTransformDirection(t_NewRot * Vector3.up);
						Quaternion t_PRot = Quaternion.LookRotation(t_PFwd, t_PUp);

						Quaternion t_R = Quaternion.Inverse(t_PLFR) * t_PRot;

						t_Bone.forward = (t_LRot * t_R * Vector3.forward);
						t_Bone.right = (t_LRot * t_R * Vector3.right);
						t_Bone.up = (t_LRot * t_R * Vector3.up);

						UpdateAnimation();
					}

					EditorGUI.BeginChangeCheck();

					// copy the target object's data to the handle
					m_Handle.xHandleColor = new Color(1.0f, 1.0f, 0.5f, 0.3f);
					m_Handle.yHandleColor = i == 0 ? new Color(1.0f, 0.5f, 1.0f, 0.3f) : Color.clear;
					m_Handle.zHandleColor = Color.clear;
					m_Handle.xMin = t_Bone.extentStretch.x;
					m_Handle.xMax = t_Bone.extentStretch.y;

					// CharacterJoint and ConfigurableJoint implement y- and z-axes symmetrically
					m_Handle.yMin = i == 0 ? -t_Bone.extentSpread.x : 0;
					m_Handle.yMax = i == 0 ? -t_Bone.extentSpread.y : 0;

					m_Handle.zMin = 0;
					m_Handle.zMax = 0;

					// set the handle matrix to match the object's position/rotation with a uniform scale
					t_HMatrix = Matrix4x4.TRS(
						t_Bone.transform.position,
						t_GRot * t_MirrorRot,
						//						t_RestRot * t_LRot * Quaternion.Euler(0, 0, 180),
						Vector3.one * t_Target.gizmoLength * 1.5f
					);

					using (new Handles.DrawingScope(t_HMatrix))
					{
						m_Handle.DrawHandle();
					}

					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(target, "Extents Modified");

						t_Bone.extentStretch.x = m_Handle.xMin;
						t_Bone.extentStretch.y = m_Handle.xMax;

						t_Bone.extentSpread.x = -m_Handle.yMin;
						t_Bone.extentSpread.y = -m_Handle.yMax;

						UpdateAnimation();
					}

				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("handModelType"));

			EditorGUILayout.PropertyField(serializedObject.FindProperty("wrist"));

			GUIStyle t_DDStyle = new GUIStyle(EditorStyles.foldout);
			t_DDStyle.fontSize = 12;
			t_DDStyle.fontStyle = FontStyle.Bold;
			var t_IT = serializedObject.FindProperty("fingers");
			if (t_IT.arraySize == 0) m_Setup = true;
			m_Setup = EditorGUILayout.Foldout(m_Setup, "Setup", t_DDStyle);
			if (m_Setup)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Find Bones:");
				if (GUILayout.Button("Index"))
				{
					foreach (HAnimator t_Ani in targets)
					{
						Undo.RecordObject(t_Ani, "Find Bones Index-wise");
						t_Ani.FindFingers();
					}
				}
				if (GUILayout.Button("Names"))
				{
					foreach (HAnimator t_Ani in targets)
					{
						Undo.RecordObject(t_Ani, "Find Bones Name-wise");
						t_Ani.FindFingersViaNames();
					}
				}
				if (GUILayout.Button("Manual"))
				{
					foreach (HAnimator t_Ani in targets)
					{
						Undo.RecordObject(t_Ani, "Setup Bones Manually");
						t_Ani.SetupManualBones();
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Calculate Axes"))
				{
					foreach (HAnimator t_Ani in targets)
					{
						Undo.RecordObject(t_Ani, "Calculate Finger Bones Axes");
						t_Ani.CalculateAxes();
					}
				}
				if (GUILayout.Button("Set Default Limits"))
				{
					foreach (HAnimator t_Ani in targets)
					{
						Undo.RecordObject(t_Ani, "Set Default Limits");
						t_Ani.SetDefaultLimits();
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Reset Animation"))
				{
					m_AnimationData = null;
					UpdateAnimation();
				}
				if (GUILayout.Button("Rest Animation"))
				{
					m_AnimationData = m_AnimationDataRest;
					UpdateAnimation();
				}
				if (GUILayout.Button("Fist Animation"))
				{
					m_AnimationData = m_AnimationDataFist;
					UpdateAnimation();
				}
				EditorGUILayout.EndHorizontal();
			}

			for (int i = 0; i < t_IT.arraySize; i++)
			{
				EditorGUILayout.LabelField(((Manus.Utility.FingerType)i).ToString(), EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(t_IT.GetArrayElementAtIndex(i), new GUIContent(i.ToString()), true);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

	[CustomPropertyDrawer(typeof(HAFingerBones))]
	public class FingerBonesDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect p_Position, SerializedProperty p_Property, GUIContent p_Label)
		{
			EditorGUI.BeginProperty(p_Position, p_Label, p_Property);

			int t_JointLabelOffset = 0;
			if (p_Label != null && p_Label.text[0] != '0')
			{
				t_JointLabelOffset = 1;
			}

			var t_IT = p_Property.FindPropertyRelative("bones");
			Rect t_Rect = p_Position;
			t_Rect.x += 40.0f;
			t_Rect.width -= 40.0f;
			Rect t_LabelRect = p_Position;
			t_LabelRect.x += 7.0f;
			t_LabelRect.width = 30;
			for (int i = 0; i < t_IT.arraySize; i++)
			{
				var t_Label = new GUIContent(p_Label.text + i);
				var t_Props = t_IT.GetArrayElementAtIndex(i);
				float t_H = EditorGUI.GetPropertyHeight(t_Props, t_Label);
				t_Rect.height = t_H;
				t_LabelRect.height = t_H;

				EditorGUI.DrawRect(t_Rect, new Color(0, 0, 0, 0.1f));

				if (i < 3) EditorGUI.LabelField(t_LabelRect, ((Manus.Utility.FingerJointType)(i + t_JointLabelOffset)).ToString());
				EditorGUI.PropertyField(t_Rect, t_Props, t_Label);
				t_Rect.y += t_H + 4.0f;
				t_LabelRect.y += t_H + 4.0f;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty p_Property, GUIContent p_Label)
		{
			float t_H = 0.0f;
			var t_IT = p_Property.FindPropertyRelative("bones");
			for (int i = 0; i < t_IT.arraySize; i++)
			{
				var t_Props = t_IT.GetArrayElementAtIndex(i);
				t_H += EditorGUI.GetPropertyHeight(t_Props);
			}
			return base.GetPropertyHeight(p_Property, p_Label) + t_H;
		}
	}

	[CustomPropertyDrawer(typeof(HAFingerBone))]
	public class FingerBoneDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect p_Position, SerializedProperty p_Property, GUIContent p_Label)
		{
			EditorGUI.BeginProperty(p_Position, p_Label, p_Property);

			bool t_IsJoint = false;
			if (p_Label != null && (p_Label.text[1] == '0' || p_Label.text[1] == '1' || p_Label.text[1] == '2'))
			{
				t_IsJoint = true;
			}

			bool t_UseSpread = false;
			if (p_Label != null && p_Label.text[1] == '0')
			{
				t_UseSpread = true;
			}

			p_Position.x += 1.0f;
			p_Position.y += 1.0f;
			p_Position.width -= 2.0f;

			Rect t_CurrentRect = p_Position;

			t_CurrentRect.height = 18;
			EditorGUI.ObjectField(t_CurrentRect, p_Property.FindPropertyRelative("transform"));

			t_CurrentRect.y += t_CurrentRect.height;

			t_CurrentRect.y += 2.0f;

			if (t_IsJoint == false)
			{
				p_Position.y = t_CurrentRect.y;
				EditorGUI.EndProperty();
				return;
			}

			t_CurrentRect.width = 100;

			var t_SLimit = p_Property.FindPropertyRelative("extentStretch");
			Vector2 t_Vec = t_SLimit.vector2Value;

			EditorGUI.LabelField(t_CurrentRect, "Stretch Extents");

			EditorGUI.BeginChangeCheck();

			t_CurrentRect.x = p_Position.x + 100.0f;
			t_CurrentRect.width = 30.0f;
			t_Vec.x = EditorGUI.FloatField(t_CurrentRect, t_Vec.x);

			t_CurrentRect.x = p_Position.x + 140.0f;
			t_CurrentRect.width = p_Position.width - 40.0f - 140.0f;
			EditorGUI.MinMaxSlider(t_CurrentRect, ref t_Vec.x, ref t_Vec.y, -180.0f, 180.0f);

			t_CurrentRect.x = p_Position.x + p_Position.width - 30.0f;
			t_CurrentRect.width = 30.0f;
			t_Vec.y = EditorGUI.FloatField(t_CurrentRect, t_Vec.y);

			if (EditorGUI.EndChangeCheck())
			{
				t_SLimit.vector2Value = t_Vec;
			}

			t_CurrentRect.y += t_CurrentRect.height;
			p_Position.y = t_CurrentRect.y;

			if (t_UseSpread)
			{
				t_CurrentRect = p_Position;
				t_CurrentRect.height = 18;

				t_SLimit = p_Property.FindPropertyRelative("extentSpread");
				t_Vec = t_SLimit.vector2Value;

				EditorGUI.LabelField(t_CurrentRect, "Spread Extents");

				EditorGUI.BeginChangeCheck();

				t_CurrentRect.x = p_Position.x + 100.0f;
				t_CurrentRect.width = 30.0f;
				t_Vec.x = EditorGUI.FloatField(t_CurrentRect, t_Vec.x);

				t_CurrentRect.x = p_Position.x + 140.0f;
				t_CurrentRect.width = p_Position.width - 40.0f - 140.0f;
				EditorGUI.MinMaxSlider(t_CurrentRect, ref t_Vec.x, ref t_Vec.y, -180.0f, 180.0f);

				t_CurrentRect.x = p_Position.x + p_Position.width - 30.0f;
				t_CurrentRect.width = 30.0f;
				t_Vec.y = EditorGUI.FloatField(t_CurrentRect, t_Vec.y);

				if (EditorGUI.EndChangeCheck())
				{
					t_SLimit.vector2Value = t_Vec;
				}
				p_Position.y += p_Position.height;
			}

			p_Position.y += 1.0f;


			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty p_Property, GUIContent p_Label)
		{
			float t_Height = 40.0f;
			if (p_Label != null && p_Label.text[1] == '0')
			{
				t_Height += 18.0f;
			}

			return t_Height;
		}
	}
}
#endif
