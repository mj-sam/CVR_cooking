using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.Hand.Gesture
{
	/// <summary>
	/// This is a simple implementation of a gesture.
	/// The GestureSimple evaluates finger joints flex ranges and uses these to
	/// determine if a certain gesture is being made.
	/// </summary>
	[CreateAssetMenu(fileName = "Gesture", menuName = "Manus/Gesture", order = 1)]
	public class GestureSimple : GestureBase
	{
		/// <summary>
		/// A class containing information on flex values.
		/// </summary>
		[System.Serializable]
		public class FingerFlex : PropertyAttribute
		{
			public const float minFlex = -0.01f;
			public const float maxFlex = 1.01f;
			public Vector2[] joints = new Vector2[(int)Utility.FingerJointType.Invalid];

			public FingerFlex()
			{
				for (int i = 0; i < joints.Length; i++)
				{
					joints[i] = new Vector2(minFlex, maxFlex);
				}
			}
		}

		[System.Serializable]
		public class FingerSpread : PropertyAttribute
		{
			public const float minSpread = -1.01f;
			public const float maxSpread = 1.01f;

			public Vector2 spreadRange = new Vector2();

			public FingerSpread()
			{
				spreadRange = new Vector2(minSpread, maxSpread);
			}
		}

		/// <summary>
		/// The flex value ranges required from the fingers.
		/// </summary>
		public FingerFlex[] flexValues = new FingerFlex[(int)Utility.FingerType.Invalid];

		public FingerSpread[] spreadValues = new FingerSpread[(int)Utility.FingerType.Invalid];

		/// <summary>
		/// This function evaluates the gesture and returns True if the gesture is being made.
		/// </summary>
		/// <param name="p_Hand">The Hand to evaluate.</param>
		/// <returns>True if the gesture is made.</returns>
		public override bool Evaluate(Hand p_Hand)
		{
			if (p_Hand.data == null) return false;
			int i = (int)Utility.FingerJointType.CMC;
			for (int f = 0; f < flexValues.Length; f++)
			{
				for (; i < (int)Utility.FingerJointType.DIP; i++)
				{
					float t_Flex = Mathf.Clamp01(p_Hand.data.GetFinger(f).GetJoint(i).stretch);
					if (flexValues[f].joints[i].x > t_Flex || flexValues[f].joints[i].y < t_Flex)
					{
						return false;
					}
				}
				i = (int)Utility.FingerJointType.MCP;
			}

			Utility.FingerJointType t_CurrentJointType = Utility.FingerJointType.CMC;

			for (int j = 0; j < spreadValues.Length; j++)
			{
				float t_Spread = p_Hand.data.GetFinger(j).GetJoint(t_CurrentJointType).spread;

				if (spreadValues[j].spreadRange.x > t_Spread || spreadValues[j].spreadRange.y < t_Spread)
				{
					return false;
				}

				t_CurrentJointType = Utility.FingerJointType.MCP;
			}

			return true;
		}
	}
}

#if UNITY_EDITOR
namespace Manus.Editor.Core.Hand.Gesture
{
	using UnityEditor;

	using GData = Manus.Hermes.Glove.Data;
	using HAnimator = Manus.Hand.HandAnimator;

	using GestureSimple = Manus.Hand.Gesture.GestureSimple;
	using FFlex = Manus.Hand.Gesture.GestureSimple.FingerFlex;
	using FSpread = Manus.Hand.Gesture.GestureSimple.FingerSpread;

	[CustomEditor(typeof(GestureSimple))]
	public class GestureSimpleEditor : Editor
	{
		internal class PreviewSettings
		{
			public Vector3 orthoPosition = new Vector3(0.0f, 0.0f, 0.0f);
			public Vector2 previewDir = new Vector2(0, 0);
			public float zoomFactor = 1.0f;
			public int checkerTextureMultiplier = 10;
		}

		PreviewSettings m_Settings;
		PreviewRenderUtility m_PreviewUtility;

		GameObject m_PreviewPrefab;
		GameObject m_PreviewInstance;
		GameObject m_PreviewInstanceMin;
		GameObject m_PreviewInstanceMax;
		GameObject m_PreviewLight;
		SkinnedMeshRenderer m_SkinMeshRender;

		GData m_FakeData;
		GData m_FakeDataMin;
		GData m_FakeDataMax;

		HAnimator m_HandAnimator;
		HAnimator m_HandAnimatorMin;
		HAnimator m_HandAnimatorMax;

		Material m_HandMaterial;
		Material m_HandMaterialMin;
		Material m_HandMaterialMax;

		static float m_DisplayPose = 0.5f;

		UnityEngine.SceneManagement.Scene m_Scene;

		public override bool HasPreviewGUI() { return true; }

		private void Init()
		{
			if (m_PreviewUtility == null)
			{
				Debug.Log("New Preview");
				m_PreviewUtility = new PreviewRenderUtility();
				m_PreviewUtility.camera.fieldOfView = 30.0f;
				m_PreviewUtility.camera.transform.position = new Vector3(5, 5, 0);
			}

			if (m_Settings == null)
			{
				Debug.Log("New Settings");
				m_Settings = new PreviewSettings();

				m_Settings.orthoPosition = new Vector3(0.5f, 0.5f, -1);
				m_Settings.previewDir = new Vector2(-110, 0);
				m_Settings.zoomFactor = 1.0f;
			}

			if (m_FakeData == null)
			{
				m_FakeData = new GData(null, 0, HProt.HandType.UnknownChirality);
			}
			if (m_FakeDataMin == null)
			{
				m_FakeDataMin = new GData(null, 0, HProt.HandType.UnknownChirality);
			}
			if (m_FakeDataMax == null)
			{
				m_FakeDataMax = new GData(null, 0, HProt.HandType.UnknownChirality);
			}

			if (!m_Scene.IsValid())
			{
				Debug.Log("New Scene");
				m_Scene = UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();

				string[] t_Str = AssetDatabase.FindAssets("ManusGestureHand t:prefab");
				if (t_Str.Length == 0) return;
				string t_Path = AssetDatabase.GUIDToAssetPath(t_Str[0]);

				m_PreviewPrefab = AssetDatabase.LoadAssetAtPath<Object>(t_Path) as GameObject;

				m_PreviewInstance = GameObject.Instantiate(m_PreviewPrefab, m_PreviewPrefab.transform.position, m_PreviewPrefab.transform.rotation);
				m_PreviewInstance.name = "Poser";
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(m_PreviewInstance, m_Scene);

				m_PreviewInstanceMin = GameObject.Instantiate(m_PreviewPrefab, m_PreviewPrefab.transform.position, m_PreviewPrefab.transform.rotation);
				m_PreviewInstanceMin.name = "Min";
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(m_PreviewInstanceMin, m_Scene);

				m_PreviewInstanceMax = GameObject.Instantiate(m_PreviewPrefab, m_PreviewPrefab.transform.position, m_PreviewPrefab.transform.rotation);
				m_PreviewInstanceMax.name = "Max";
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(m_PreviewInstanceMax, m_Scene);

				m_HandAnimator = m_PreviewInstance.GetComponent<HAnimator>();
				m_HandAnimatorMin = m_PreviewInstanceMin.GetComponent<HAnimator>();
				m_HandAnimatorMax = m_PreviewInstanceMax.GetComponent<HAnimator>();

				m_PreviewLight = new GameObject();
				Light t_Light = m_PreviewLight.AddComponent<Light>();

				t_Light.color = Color.white;
				t_Light.type = LightType.Directional;
				t_Light.intensity = 1;
				t_Light.bounceIntensity = 1;
				t_Light.lightmapBakeType = LightmapBakeType.Realtime;

				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(m_PreviewLight, m_Scene);

				m_HandMaterial = new Material(Shader.Find("Standard"));
				m_HandMaterialMin = new Material(Shader.Find("Standard"));
				m_HandMaterialMax = new Material(Shader.Find("Standard"));

				m_HandMaterial.color = Color.yellow;

				m_HandMaterialMin.color = new Color(1.0f, 0.0f, 0.0f, 0.25f);
				m_HandMaterialMin.hideFlags = HideFlags.HideAndDontSave;
				m_HandMaterialMin.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				m_HandMaterialMin.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				m_HandMaterialMin.SetInt("_ZWrite", 0);
				m_HandMaterialMin.DisableKeyword("_ALPHATEST_ON");
				m_HandMaterialMin.EnableKeyword("_ALPHABLEND_ON");
				m_HandMaterialMin.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				m_HandMaterialMin.renderQueue = 3000;

				m_HandMaterialMax.color = new Color(0.0f, 1.0f, 0.0f, 0.25f);
				m_HandMaterialMax.hideFlags = HideFlags.HideAndDontSave;
				m_HandMaterialMax.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				m_HandMaterialMax.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				m_HandMaterialMin.SetInt("_ZWrite", 0);
				m_HandMaterialMax.DisableKeyword("_ALPHATEST_ON");
				m_HandMaterialMax.EnableKeyword("_ALPHABLEND_ON");
				m_HandMaterialMax.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				m_HandMaterialMin.renderQueue = 3000;

				var t_SRenders = m_PreviewInstance.GetComponentsInChildren<SkinnedMeshRenderer>();

				m_SkinMeshRender = t_SRenders[0];
				foreach (var t_SMR in t_SRenders)
				{
					t_SMR.material = m_HandMaterial;
				}

				t_SRenders = m_PreviewInstanceMin.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (var t_SMR in t_SRenders)
				{
					t_SMR.material = m_HandMaterialMin;
				}

				t_SRenders = m_PreviewInstanceMax.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (var t_SMR in t_SRenders)
				{
					t_SMR.material = m_HandMaterialMax;
				}
			}
		}

		void ResetView()
		{
			m_Settings.zoomFactor = 1.0f;
			m_Settings.orthoPosition = new Vector3(0.5f, 0.5f, -1);
		}

		public override void OnPreviewGUI(Rect p_Rect, GUIStyle p_Style)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(p_Rect.x, p_Rect.y, p_Rect.width, 40f), "Mesh preview requires\nrender texture support");
				}
				return;
			}

			Init();

			Drag2D(p_Rect);

			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			m_PreviewUtility.BeginPreview(p_Rect, p_Style);
			DoRenderPreview();
			m_PreviewUtility.EndAndDrawPreview(p_Rect);
		}

		private void DoRenderPreview()
		{
			RenderMeshPreview(m_PreviewUtility, m_Settings, -1);
		}

		void RenderMeshPreview(PreviewRenderUtility p_PreviewUtility, PreviewSettings p_Settings, int p_MeshSubset)
		{
			if (p_PreviewUtility == null)
			{
				return;
			}

			Bounds t_Bounds = m_SkinMeshRender.bounds;

			Transform t_CTransform = p_PreviewUtility.camera.GetComponent<Transform>();
			p_PreviewUtility.camera.nearClipPlane = 0.01f;
			p_PreviewUtility.camera.farClipPlane = 10f;

			float t_HSize = t_Bounds.extents.magnitude;
			float t_Dist = 4.0f * t_HSize * p_Settings.zoomFactor;

			p_PreviewUtility.camera.scene = m_Scene;
			p_PreviewUtility.camera.backgroundColor = Color.gray;
			p_PreviewUtility.camera.clearFlags = CameraClearFlags.Color;

			p_PreviewUtility.camera.orthographic = false;
			Quaternion t_CamRot = Quaternion.Euler(-p_Settings.previewDir.y, -p_Settings.previewDir.x, 0);
			Vector3 t_CamPos = t_CamRot * (Vector3.forward * -t_Dist);
			t_CTransform.position = t_CamPos;
			t_CTransform.rotation = t_CamRot;

			m_PreviewLight.transform.rotation = t_CamRot;

			PrepareHandPose();

			p_PreviewUtility.camera.Render();
		}

		void PrepareHandPose()
		{
			GestureSimple t_Target = target as GestureSimple;
			int i = (int)Manus.Utility.FingerJointType.CMC;
			for (int f = 0; f < t_Target.flexValues.Length; f++)
			{
				for (; i < (int)Manus.Utility.FingerJointType.DIP; i++)
				{
					m_FakeData.GetFinger(f).GetJoint(i).ApplyStretchData(Mathf.Lerp(t_Target.flexValues[f].joints[i].x, t_Target.flexValues[f].joints[i].y, m_DisplayPose));
					m_FakeDataMin.GetFinger(f).GetJoint(i).ApplyStretchData(t_Target.flexValues[f].joints[i].x);
					m_FakeDataMax.GetFinger(f).GetJoint(i).ApplyStretchData(t_Target.flexValues[f].joints[i].y);
				}
				i = (int)Manus.Utility.FingerJointType.MCP;
			}

			m_HandAnimator.Animate(m_FakeData);
			m_HandAnimatorMin.Animate(m_FakeDataMin);
			m_HandAnimatorMax.Animate(m_FakeDataMax);
		}

		public override void OnPreviewSettings()
		{
			GUILayout.Label("Pose: ");
			m_DisplayPose = EditorGUILayout.Slider(m_DisplayPose, 0.0f, 1.0f, GUILayout.Width(50f));
		}

		private void OnDisable()
		{
			if (m_PreviewUtility != null)
			{
				m_PreviewUtility.Cleanup();
				m_PreviewUtility = null;
			}
			UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(m_Scene);

			DestroyImmediate(m_HandMaterial);
			DestroyImmediate(m_HandMaterialMin);
			DestroyImmediate(m_HandMaterialMax);
		}

		public void Drag2D(Rect p_Rect)
		{
			int t_ControlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
			Event t_Current = Event.current;
			switch (t_Current.GetTypeForControl(t_ControlID))
			{
				case EventType.ScrollWheel:
					{
						m_Settings.zoomFactor += 0.1f * t_Current.delta.normalized.y;
						m_Settings.zoomFactor = Mathf.Clamp(m_Settings.zoomFactor, 0.1f, 2.0f);
						t_Current.Use();
						GUI.changed = true;
					}
					break;
				case EventType.MouseDown:
					if (p_Rect.Contains(t_Current.mousePosition) && p_Rect.width > 50f)
					{
						GUIUtility.hotControl = t_ControlID;
						t_Current.Use();
						EditorGUIUtility.SetWantsMouseJumping(1);
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == t_ControlID)
					{
						GUIUtility.hotControl = 0;
					}
					EditorGUIUtility.SetWantsMouseJumping(0);
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == t_ControlID)
					{
						m_Settings.previewDir -= t_Current.delta * (float)((!t_Current.shift) ? 1 : 3) / Mathf.Min(p_Rect.width, p_Rect.height) * 140f;
						m_Settings.previewDir.y = Mathf.Clamp(m_Settings.previewDir.y, -90f, 90f);
						t_Current.Use();
						GUI.changed = true;
					}
					break;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var t_IT = serializedObject.FindProperty("flexValues");
			var t_SpreadValues = serializedObject.FindProperty("spreadValues");

			for (int i = 0; i < t_IT.arraySize; i++)
			{
				EditorGUILayout.LabelField(((Manus.Utility.FingerType)i).ToString(), EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(t_IT.GetArrayElementAtIndex(i), new GUIContent(i.ToString()));
				EditorGUILayout.PropertyField(t_SpreadValues.GetArrayElementAtIndex(i), new GUIContent());
			}

			serializedObject.ApplyModifiedProperties();
		}

	}

	[CustomPropertyDrawer(typeof(FFlex))]
	public class GestureSimpleFingerFlexDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect p_Position, SerializedProperty p_Property, GUIContent p_Label)
		{
			EditorGUI.BeginProperty(p_Position, p_Label, p_Property);

			var t_Prop = p_Property.FindPropertyRelative("joints");

			Rect t_Label = p_Position;
			t_Label.height = 20.0f;
			EditorGUI.LabelField(t_Label, "Straight");
			t_Label.x = p_Position.width - 25;
			EditorGUI.LabelField(t_Label, "Flexed");

			p_Position.y += t_Label.height;
			p_Position.height = 20;

			t_Label = p_Position;
			t_Label.x = (p_Position.width * 0.5f) - 10;
			t_Label.y -= 12.0f;

			int t_JointLabelOffset = (int)Manus.Utility.FingerJointType.CMC;
			if (p_Label != null && p_Label.text[0] != '0')
			{
				t_JointLabelOffset = (int)Manus.Utility.FingerJointType.MCP;
			}

			for (int i = t_JointLabelOffset; i < (int)Manus.Utility.FingerJointType.DIP && i < t_Prop.arraySize; i++)
			{
				Vector2 t_Vec = t_Prop.GetArrayElementAtIndex(i).vector2Value;
				EditorGUI.LabelField(t_Label, ((Manus.Utility.FingerJointType)i).ToString());
				EditorGUI.MinMaxSlider(p_Position, ref t_Vec.x, ref t_Vec.y, FFlex.minFlex, FFlex.maxFlex);
				t_Prop.GetArrayElementAtIndex(i).vector2Value = t_Vec;

				t_Label.y += 30.0f;
				p_Position.y += 30.0f;
			}
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty p_Property, GUIContent p_Label)
		{
			return base.GetPropertyHeight(p_Property, p_Label) + 80.0f;
		}
	}

	//TODO Spread values for thumb should be out/in instead of Left/Right

	[CustomPropertyDrawer(typeof(FSpread))]
	public class GestureSimpleFingerSpreadDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect p_Position, SerializedProperty p_Property, GUIContent p_Label)
		{
			EditorGUI.BeginProperty(p_Position, p_Label, p_Property);

			var t_Prop = p_Property.FindPropertyRelative("spreadRange");

			Rect t_Label = p_Position;
			t_Label.height = 20.0f;
			EditorGUI.LabelField(t_Label, "Left");
			t_Label.x = p_Position.width - 25;
			EditorGUI.LabelField(t_Label, "Right");

			p_Position.y += t_Label.height;
			p_Position.height = 20;

			t_Label = p_Position;
			t_Label.x = (p_Position.width * 0.5f) - 10;
			t_Label.y -= 12.0f;

			Vector2 t_Vec = t_Prop.vector2Value;

			EditorGUI.LabelField(t_Label, "Spread");
			EditorGUI.MinMaxSlider(p_Position, ref t_Vec.x, ref t_Vec.y, FSpread.minSpread, FSpread.maxSpread);
			t_Prop.vector2Value = t_Vec;

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty p_Property, GUIContent p_Label)
		{
			return base.GetPropertyHeight(p_Property, p_Label) + 30.0f;
		}
	}
}
#endif
