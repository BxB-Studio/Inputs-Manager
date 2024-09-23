#region Namespaces

using UnityEngine;
using UnityEditor;

#endregion

namespace Utilities.Editor
{
	public class WheelRadiusCalculatorEditor : EditorWindow
	{
		#region Variables

		private static int width = 225;
		private static int aspect = 55;
		private static int diameter = 17;

		#endregion

		#region Methods

		#region Static Methods

		[MenuItem("Tools/Utilities/Wheel Radius Calculator")]
		public static void ShowWindow()
		{
			WheelRadiusCalculatorEditor window = GetWindow<WheelRadiusCalculatorEditor>(true, "Wheel Radius Calculator", true);

			window.minSize = new Vector2(512f, 128f);
			window.maxSize = window.minSize;
		}

		#endregion

		#region Global Methods

		private void OnGUI()
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Wheel Radius Calculator", EditorStyles.boldLabel);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(25f);
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();

			float orgLabelWidth = EditorGUIUtility.labelWidth;

			EditorGUIUtility.labelWidth = 15f;
			width = Mathf.RoundToInt(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(Utility.NumberToValueWithUnit(width, "mm", true), new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter })));

			EditorGUILayout.PrefixLabel("/", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });

			aspect = Mathf.Clamp(Mathf.RoundToInt(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(Utility.NumberToValueWithUnit(aspect, "%", true), new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter }))), 0, 100);

			EditorGUILayout.PrefixLabel("R", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });

			diameter = Mathf.RoundToInt(Utility.ValueWithUnitToNumber(EditorGUILayout.TextField(Utility.NumberToValueWithUnit(diameter, "in", true), new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter })));
			EditorGUIUtility.labelWidth = orgLabelWidth;

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUILayout.Space(25f);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(100f);
			EditorGUILayout.BeginVertical();

			float wheelDiameter = Utility.ValueWithUnitToNumber(EditorGUILayout.TextField("Diameter", Utility.NumberToValueWithUnit((diameter * 25.4f + 2f * (aspect * width / 100f)) * .001f, "m", 6), new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter }));

			EditorGUILayout.TextField("Radius", Utility.NumberToValueWithUnit(wheelDiameter * .5f, "m", 6), new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter });
			EditorGUILayout.EndVertical();
			GUILayout.Space(100f);
			EditorGUILayout.EndHorizontal();
		}

		#endregion

		#endregion
	}
}
