#region Namespaces

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

#endregion

namespace Utilities.Editor
{
	public static class EditorUtilities
	{
		#region Modules

		public static class Styles
		{
			public static GUIStyle Button => new("Button")
			{
#if UNITY_2019_3_OR_NEWER
				normal = new GUIStyleState()
				{
					textColor = Utility.Color.lightGray
				}
#endif
			};
			public static GUIStyle ButtonActive
			{
				get
				{
					GUIStyle style = new("Button");

#if UNITY_2019_3_OR_NEWER
					style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
#else
					style.normal = style.active;
#endif

					return style;
				}
			}
			public static GUIStyle MiniButton => new("MiniButton")
			{
#if UNITY_2019_3_OR_NEWER
				normal = new GUIStyleState()
				{
					textColor = Utility.Color.lightGray
				}
#endif
			};
			public static GUIStyle MiniButtonActive
			{
				get
				{
					GUIStyle style = new("MiniButton");

#if UNITY_2019_3_OR_NEWER
					style.normal = new()
					{
						background = style.active.background,
						scaledBackgrounds = style.active.scaledBackgrounds,
						textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black
					};
#else
					style.normal = style.active;
#endif

					return style;
				}
			}
			public static GUIStyle MiniButtonMiddle => new("MiniButtonMid")
			{
#if UNITY_2019_3_OR_NEWER
				normal = new GUIStyleState()
				{
					textColor = Utility.Color.lightGray
				}
#endif
			};
			public static GUIStyle MiniButtonMiddleActive
			{
				get
				{
					GUIStyle style = new("MiniButtonMid");

#if UNITY_2019_3_OR_NEWER
					style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
#else
					style.normal = style.active;
#endif

					return style;
				}
			}
			public static GUIStyle MiniButtonLeft => new("MiniButtonLeft")
			{
#if UNITY_2019_3_OR_NEWER
				normal = new GUIStyleState()
				{
					textColor = Utility.Color.lightGray
				}
#endif
			};
			public static GUIStyle MiniButtonLeftActive
			{
				get
				{
					GUIStyle style = new("MiniButtonLeft");

#if UNITY_2019_3_OR_NEWER
					style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
#else
					style.normal = style.active;
#endif

					return style;
				}
			}
			public static GUIStyle MiniButtonRight => new("MiniButtonRight")
			{
#if UNITY_2019_3_OR_NEWER
				normal = new GUIStyleState()
				{
					textColor = Utility.Color.lightGray
				}
#endif
			};
			public static GUIStyle MiniButtonRightActive
			{
				get
				{
					GUIStyle style = new("MiniButtonRight");

#if UNITY_2019_3_OR_NEWER
					style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
#else
					style.normal = style.active;
#endif

					return style;
				}
			}
		}
		public static class Icons
		{
			public static Texture2D Add => Resources.Load($"{IconsPath}/{IconsThemeFolder}/plus") as Texture2D;
			public static Texture2D CaretUp => Resources.Load($"{IconsPath}/{IconsThemeFolder}/caret-up") as Texture2D;
			public static Texture2D CaretDown => Resources.Load($"{IconsPath}/{IconsThemeFolder}/caret-down") as Texture2D;
			public static Texture2D CaretLeft => Resources.Load($"{IconsPath}/{IconsThemeFolder}/caret-left") as Texture2D;
			public static Texture2D CaretRight => Resources.Load($"{IconsPath}/{IconsThemeFolder}/caret-right") as Texture2D;
			public static Texture2D Chart => Resources.Load($"{IconsPath}/{IconsThemeFolder}/chart") as Texture2D;
			public static Texture2D ChevronUp => Resources.Load($"{IconsPath}/{IconsThemeFolder}/chevron-up") as Texture2D;
			public static Texture2D ChevronDown => Resources.Load($"{IconsPath}/{IconsThemeFolder}/chevron-down") as Texture2D;
			public static Texture2D ChevronLeft => Resources.Load($"{IconsPath}/{IconsThemeFolder}/chevron-left") as Texture2D;
			public static Texture2D ChevronRight => Resources.Load($"{IconsPath}/{IconsThemeFolder}/chevron-right") as Texture2D;
			public static Texture2D Check => Resources.Load($"{IconsPath}/{IconsThemeFolder}/check") as Texture2D;
			public static Texture2D CheckCircle => Resources.Load($"{IconsPath}/{IconsThemeFolder}/check-circle") as Texture2D;
			public static Texture2D CheckCircleColored => Resources.Load($"{IconsPath}/check-circle") as Texture2D;
			public static Texture2D CheckColored => Resources.Load($"{IconsPath}/check") as Texture2D;
			public static Texture2D Clone => Resources.Load($"{IconsPath}/{IconsThemeFolder}/clone") as Texture2D;
			public static Texture2D Box => Resources.Load($"{IconsPath}/{IconsThemeFolder}/box") as Texture2D;
			public static Texture2D Cross => Resources.Load($"{IconsPath}/{IconsThemeFolder}/cross") as Texture2D;
			public static Texture2D Error => Resources.Load($"{IconsPath}/exclamation-circle") as Texture2D;
			public static Texture2D ExclamationCircle => Resources.Load($"{IconsPath}/{IconsThemeFolder}/exclamation-circle") as Texture2D;
			public static Texture2D ExclamationTriangle => Resources.Load($"{IconsPath}/{IconsThemeFolder}/exclamation-triangle") as Texture2D;
			public static Texture2D ExclamationSquare => Resources.Load($"{IconsPath}/{IconsThemeFolder}/exclamation-square") as Texture2D;
			public static Texture2D Eye => Resources.Load($"{IconsPath}/{IconsThemeFolder}/eye") as Texture2D;
			public static Texture2D Info => Resources.Load($"{IconsPath}/exclamation-square") as Texture2D;
			public static Texture2D Pencil => Resources.Load($"{IconsPath}/{IconsThemeFolder}/pencil") as Texture2D;
			public static Texture2D Reload => Resources.Load($"{IconsPath}/{IconsThemeFolder}/reload") as Texture2D;
			public static Texture2D Settings => Resources.Load($"{IconsPath}/{IconsThemeFolder}/cog") as Texture2D;
			public static Texture2D Save => Resources.Load($"{IconsPath}/{IconsThemeFolder}/save") as Texture2D;
			public static Texture2D Sort => Resources.Load($"{IconsPath}/{IconsThemeFolder}/sort") as Texture2D;
			public static Texture2D Trash => Resources.Load($"{IconsPath}/{IconsThemeFolder}/trash") as Texture2D;
			public static Texture2D Warning => Resources.Load($"{IconsPath}/exclamation-triangle") as Texture2D;
		}

		#endregion

		#region Variables

		private static readonly string IconsPath = "Editor/Icons";
		private static readonly string IconsThemeFolder = EditorGUIUtility.isProSkin ? "Pro" : "Personal";

		#endregion

		#region Methods

		#region Utilities

		public static void AddScriptingDefineSymbol(string symbol)
		{
			string[] scriptingDefineSymbols = GetScriptingDefineSymbols();
			bool emptySymbols = scriptingDefineSymbols.Length < 1;

			if (emptySymbols || !ScriptingDefineSymbolExists(scriptingDefineSymbols, symbol))
				PlayerSettings.SetScriptingDefineSymbols(GetCurrentNamedBuildTarget(), $"{(!emptySymbols ? $"{string.Join(';', scriptingDefineSymbols)};" : "")}{symbol}");
		}
		public static void RemoveScriptingDefineSymbol(string symbol)
		{
			string[] scriptingDefineSymbols = GetScriptingDefineSymbols();

			if (scriptingDefineSymbols.Length > 0 && ScriptingDefineSymbolExists(scriptingDefineSymbols, symbol, out int symbolIndex))
			{
				ArrayUtility.RemoveAt(ref scriptingDefineSymbols, symbolIndex);
				PlayerSettings.SetScriptingDefineSymbols(GetCurrentNamedBuildTarget(), scriptingDefineSymbols.Length > 0 ? string.Join(';', scriptingDefineSymbols) : "");
			}
		}
		public static void RemoveScriptingDefineSymbol(int symbolIndex)
		{
			string[] scriptingDefineSymbols = GetScriptingDefineSymbols();

			if (symbolIndex > -1 && symbolIndex < scriptingDefineSymbols.Length)
			{
				ArrayUtility.RemoveAt(ref scriptingDefineSymbols, symbolIndex);
				PlayerSettings.SetScriptingDefineSymbols(GetCurrentNamedBuildTarget(), scriptingDefineSymbols.Length > 0 ? string.Join(';', scriptingDefineSymbols) : "");
			}
		}
		public static bool ScriptingDefineSymbolExists(string symbol)
		{
			string[] scriptingDefineSymbols = GetScriptingDefineSymbols();

			return ScriptingDefineSymbolExists(scriptingDefineSymbols, symbol);
		}
		public static bool ScriptingDefineSymbolExists(string symbol, out int symbolIndex)
		{
			string[] scriptingDefineSymbols = GetScriptingDefineSymbols();

			return ScriptingDefineSymbolExists(scriptingDefineSymbols, symbol, out symbolIndex);
		}
		public static string[] GetScriptingDefineSymbols()
		{
			GetScriptingDefineSymbols(GetCurrentNamedBuildTarget(), out string[] defines);

			return defines;
		}
		public static void GetScriptingDefineSymbols(NamedBuildTarget buildTargetGroup, out string[] defines)
		{
			PlayerSettings.GetScriptingDefineSymbols(buildTargetGroup, out defines);
		}
		public static NamedBuildTarget GetCurrentNamedBuildTarget()
		{
			return NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
		}
		public static BuildTargetGroup GetCurrentBuildTargetGroup()
		{
			return EditorUserBuildSettings.selectedBuildTargetGroup;
		}
		public static BuildTarget GetCurrentBuildTarget()
		{
			return EditorUserBuildSettings.activeBuildTarget;
		}

		private static bool ScriptingDefineSymbolExists(string[] symbols, string symbol)
		{
			return Array.IndexOf(symbols, symbol) > -1;
		}
		private static bool ScriptingDefineSymbolExists(string[] symbols, string symbol, out int index)
		{
			index = Array.IndexOf(symbols, symbol);

			return index > -1;
		}

		#endregion

		#region Menu Items

		[MenuItem("Tools/Utilities/Debug/GameObject Bounds", true)]
		public static bool DebugBoundsCheck()
		{
			if (Selection.activeGameObject && Selection.gameObjects.Length == 1)
				return true;

			return false;
		}
		[MenuItem("Tools/Utilities/Debug/GameObject Bounds", false, 0)]
		public static void DebugBounds()
		{
			if (!DebugBoundsCheck())
				return;

			Bounds bounds = Utility.GetObjectBounds(Selection.activeGameObject);

			Debug.Log($"{Selection.activeGameObject.name} Dimensions (Click to see more...)\r\n\r\nSize in meters\r\nX: {bounds.size.x}\r\nY: {bounds.size.y}\r\nZ: {bounds.size.z}\r\n\r\nCenter in Unity coordinates\r\nX: {bounds.center.x}\r\nY: {bounds.center.y}\r\nZ: {bounds.center.z}\r\n\r\n", Selection.activeGameObject);
		}
		[MenuItem("Tools/Utilities/Debug/GameObject Meshes", true)]
		public static bool DebugMeshCheck()
		{
			if (Selection.activeGameObject && Selection.gameObjects.Length == 1 && Selection.activeGameObject.GetComponentInChildren<MeshFilter>())
				return true;

			return false;
		}
		[MenuItem("Tools/Utilities/Debug/GameObject Meshes", false, 0)]
		public static void DebugMesh()
		{
			if (!DebugMeshCheck())
				return;

			MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
			int vertices = 0;
			int triangles = 0;

			for (int i = 0; i < meshFilters.Length; i++)
				if (meshFilters[i].sharedMesh)
				{
					vertices += meshFilters[i].sharedMesh.vertexCount;
					triangles += meshFilters[i].sharedMesh.triangles.Length;

#if UNITY_2019_3_OR_NEWER
					for (int j = 0; j < meshFilters[i].sharedMesh.subMeshCount; j++)
						vertices += meshFilters[i].sharedMesh.GetSubMesh(j).vertexCount;
#endif
				}

			Debug.Log($"{Selection.activeGameObject.name} Mesh Details (Click to see more...)\r\n\r\nVertices: {vertices}\r\nTriangles: {triangles}", Selection.activeGameObject);
		}
		[MenuItem("Tools/Utilities/Place the selected object on top of the Zero surface", true)]
		public static bool PlaceObjectOnSurfaceCheck()
		{
			if (Selection.activeGameObject && Selection.gameObjects.Length == 1)
				return true;

			return false;
		}
		[MenuItem("Tools/Utilities/Place the selected object on top of the Zero surface", false, 100)]
		public static void PlaceObjectOnSurface()
		{
			if (!Selection.activeGameObject)
				return;

			Bounds bounds = Utility.GetObjectBounds(Selection.activeGameObject, true);

			Selection.activeGameObject.transform.position = new Vector3();
			Selection.activeGameObject.transform.position = Vector3.up * (bounds.center.y * -1f + bounds.extents.y);

			Debug.Log(Selection.activeGameObject.name + " placed on the surface successfully!", Selection.activeGameObject);
		}
		[MenuItem("Tools/Utilities/Export textures from Texture Array", true)]
		public static bool Texture2DArrayExportCheck()
		{
			if (Selection.activeObject && Selection.objects.Length == 1)
				return Selection.activeObject is Texture2DArray;

			return false;
		}
		[MenuItem("Tools/Utilities/Export textures from Texture Array")]
		public static void Texture2DArrayExport()
		{
			if (!Texture2DArrayExportCheck())
				return;

			Texture2DArray array = Selection.activeObject as Texture2DArray;
			Texture2D[] textures = Utility.GetTextureArrayItems(array);
			string path = EditorUtility.SaveFolderPanel("Choose a save destination...", Path.GetDirectoryName(AssetDatabase.GetAssetPath(array)), array.name);

			if (path.IsNullOrEmpty())
				return;

			for (int i = 0; i < textures.Length; i++)
			{
				EditorUtility.DisplayProgressBar("Exporting...", $"{array.name}_{i}", (float)i / textures.Length);
				Utility.SaveTexture2D(textures[i], Utility.TextureEncodingType.PNG, $"{path}/{array.name}_{i}");
			}

			EditorUtility.DisplayProgressBar("Exporting...", "Finishing...", 1f);
			AssetDatabase.Refresh();
			EditorUtility.ClearProgressBar();
		}

		#endregion

		#endregion
	}
}
