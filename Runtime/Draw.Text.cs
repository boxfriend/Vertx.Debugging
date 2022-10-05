#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using static Vertx.Debugging.Shapes;
using UnityEditor;
#if !UNITY_2022_1_OR_NEWER
using System.Reflection;
#endif
#if !UNITY_2021_1_OR_NEWER
using Vertx.Debugging.Internal;
#else
using UnityEngine.Pool;
#endif

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace Vertx.Debugging
{
	[InitializeOnLoad]
	internal static class DrawText
	{
		private static Font Font
		{
			get
			{
				if (s_Font != null)
					return s_Font;
				return s_Font = AssetDatabase.LoadAssetAtPath<Font>("Packages/com.vertx.debugging/Editor/Assets/JetbrainsMono-Regular.ttf");
			}
		}

		public static GUIStyle TextStyle => textStyle ?? (textStyle = new GUIStyle(EditorStyles.label) { font = Font });

		private static Type GameViewType => s_GameViewType ?? (s_GameViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));

		private static EditorWindow GameView
		{
			get
			{
				if (s_GameView != null)
					return s_GameView;
				Object[] gameViewQuery = Resources.FindObjectsOfTypeAll(GameViewType);
				if (gameViewQuery == null || gameViewQuery.Length == 0)
					return null;
				return s_GameView = (EditorWindow)gameViewQuery[0];
			}
		}

		private static readonly GUIContent s_SharedContent = new GUIContent();
		private static Font s_Font;
		private static GUIStyle textStyle;
		private static Type s_GameViewType;
		private static EditorWindow s_GameView;

		static DrawText()
		{
			SceneView.duringSceneGui -= SceneViewGUI;
			SceneView.duringSceneGui += SceneViewGUI;
		}

		private static void SceneViewGUI(SceneView obj)
		{
			if (!obj.drawGizmos)
				return;

			if (Event.current.type != EventType.Repaint)
				return;

			Handles.BeginGUI();
			DoOnGUI(View.Scene);
			Handles.EndGUI();
		}

		public static void OnGUI()
		{
			if (Event.current.type != EventType.Repaint)
				return;

			DoOnGUI(View.Game);
		}

		private static void DoOnGUI(View view)
		{
			var commandBuilder = CommandBuilder.Instance;
			if (commandBuilder.DefaultTexts.Count == 0 && commandBuilder.GizmoTexts.Count == 0)
				return;
			
			bool uses3DIcons = Uses3DIcons;
			float size = IconSize;

			using (ListPool<TextData>.Get(out List<TextData> text3D))
			{
				Gather3DText(commandBuilder.DefaultTexts, text3D);
				Gather3DText(commandBuilder.GizmoTexts, text3D);

				// 3D text is collected and sorted by distance before being displayed.
				if (text3D.Count > 0)
				{
					text3D.Sort((a, b) => b.Distance.CompareTo(a.Distance));

					foreach (TextData textData in text3D)
					{
						//------DRAW-------
						Color backgroundColor = textData.BackgroundColor;
						Color textColor = textData.TextColor;
						backgroundColor.a *= textData.Alpha;
						textColor.a *= textData.Alpha;

						GUIContent content = GetGUIContentFromObject(textData.Value);
						Rect rect = new Rect(textData.ScreenPosition, TextStyle.CalcSize(content));
						DrawAtScreenPosition(rect, content, backgroundColor, textColor, null);
					}
				}
			}

			if ((view & View.Scene) == 0)
				DrawScreenTexts(commandBuilder, view);

			void Gather3DText(CommandBuilder.TextDataLists list, List<TextData> text3D)
			{
				for (int i = 0; i < list.Count; i++)
				{
					TextData textData = list.Elements[i];
					Camera camera = SceneView.currentDrawingSceneView?.camera ?? textData.Camera;
					if (camera == null) continue;
					if (!WorldToGUIPoint(textData.Position, out Vector2 screenPos, out float distance, camera)) return;
					float alpha;
					if (uses3DIcons)
					{
						float iconSize = size * 1000;
						alpha = 1 - Mathf.InverseLerp(iconSize * 0.75f, iconSize, distance);
						if (alpha <= 0)
							return;
					}
					else
					{
						alpha = 1;
					}

					textData.ScreenPosition = screenPos;
					textData.Distance = distance;
					textData.Alpha = alpha;
					text3D.Add(textData);
				}
			}
		}
		
		private static void DrawScreenTexts(CommandBuilder commandBuilder, View view)
		{
			Vector2 position = new Vector2(10, 10);
			DrawScreenText(commandBuilder.DefaultScreenTexts);
			DrawScreenText(commandBuilder.GizmoScreenTexts);
			
			void DrawScreenText(CommandBuilder.ScreenTextDataLists list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ScreenTextData textData = list.Elements[i];
					if((textData.ActiveViews & view) == 0) continue;
					GUIContent content = GetGUIContentFromObject(textData.Value);
					Rect rect = new Rect(position, TextStyle.CalcSize(content));
					DrawAtScreenPosition(rect, content, textData.BackgroundColor, textData.TextColor, textData.Context);
					position.y = rect.yMax + 1;
				}
			}
		}

#if !UNITY_2022_1_OR_NEWER
		private static Func<bool> s_use3dGizmos;
		private static Func<float> s_iconSize;
#endif

		private static bool Uses3DIcons
		{
			get
			{
#if UNITY_2022_1_OR_NEWER
				return GizmoUtility.use3dIcons;
#else
				if (s_use3dGizmos == null)
					s_use3dGizmos = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), Type.GetType("UnityEditor.AnnotationUtility,UnityEditor").GetProperty("use3dGizmos", BindingFlags.Static | BindingFlags.NonPublic).GetMethod);
				return s_use3dGizmos();
#endif
			}
		}

		private static float IconSize
		{
			get
			{
#if UNITY_2022_1_OR_NEWER
				return GizmoUtility.iconSize;
#else
				if (s_iconSize == null)
					s_iconSize = (Func<float>)Delegate.CreateDelegate(typeof(Func<float>), Type.GetType("UnityEditor.AnnotationUtility,UnityEditor").GetProperty("iconSize", BindingFlags.Static | BindingFlags.NonPublic).GetMethod);
				return s_iconSize();
#endif
			}
		}

		public static void DrawAtScreenPosition(Rect rect, GUIContent content, Color backgroundColor, Color textColor, Object context)
		{
			bool hasContext = context != null;
			if (hasContext && rect.Contains(Event.current.mousePosition))
			{
				if (Event.current.type == EventType.MouseDown)
					EditorGUIUtility.PingObject(context);
				textColor = Color.white;
			}
			else
			{
				DrawGUIRect();
			}

			TextStyle.normal.textColor = textColor;
			GUI.Label(rect, content, TextStyle);
			//-----------------

			void DrawGUIRect()
			{
				Color color1 = GUI.color;
				GUI.color *= backgroundColor;
				GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
				GUI.color = color1;
			}
		}

		private static string GetContentFromObject(object text)
		{
			switch (text)
			{
				case Vector3 vector3:
					return vector3.ToString("F3");
				case Vector2 vector2:
					return  vector2.ToString("F3");
				default:
					return text.ToString();
			}
		}

		public static GUIContent GetGUIContentFromObject(object text)
		{
			s_SharedContent.text = GetContentFromObject(text);
			return s_SharedContent;
		}

		/// <summary>
		/// Converts world point to a point in screen space if valid.
		/// </summary>
		/// <param name="query">The world point query</param>
		/// <param name="point">The GUI point. Zero if behind the camera.</param>
		/// <param name="distance">The distance to the query from the camera.</param>
		/// <param name="camera">The camera that owns the GUI space.</param>
		/// <returns>True if a valid position in front of the camera.</returns>
		private static bool WorldToGUIPoint(Vector3 query, out Vector2 point, out float distance, Camera camera)
		{
			Vector3 viewPos = camera.WorldToViewportPoint(query);
			distance = viewPos.z;
			if (distance < 0)
			{
				point = Vector2.zero;
				return false;
			}

			Vector2 viewScreenVector = new Vector2(viewPos.x, 1 - viewPos.y);
			viewScreenVector /= EditorGUIUtility.pixelsPerPoint;
			point = new Vector2(viewScreenVector.x * camera.pixelWidth, viewScreenVector.y * camera.pixelHeight);
			return true;
		}
	}
}
#endif