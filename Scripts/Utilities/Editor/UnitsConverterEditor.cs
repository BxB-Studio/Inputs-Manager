#region Namespaces

using UnityEngine;
using UnityEditor;

#endregion

namespace Utilities.Editor
{
	public class UnitsConverterEditor : EditorWindow
	{
		#region Variables

		private static Utility.Units unit = Utility.Units.Distance;
		private static float metricValue = 1f;
		private static float imperialValue = 3.28084f;

		#endregion

		#region Methods

		#region Static Methods

		[MenuItem("Tools/Utilities/Units Converter")]
		public static void ShowWindow()
		{
			UnitsConverterEditor window = GetWindow<UnitsConverterEditor>(true, "Units Converter", true);

			window.minSize = new Vector2(512f, 128f);
			window.maxSize = window.minSize;
		}

		#endregion

		#region Global Methods

		private void OnGUI()
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Units Converter", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			Utility.Units newUnit = (Utility.Units)EditorGUILayout.EnumPopup("Unit", unit);
			string unitPrefix = newUnit == Utility.Units.Speed || newUnit == Utility.Units.Torque ? string.Empty : "s";

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Metric ({Utility.FullUnit(newUnit, Utility.UnitType.Metric)}{unitPrefix})", EditorStyles.miniBoldLabel);
			EditorGUILayout.LabelField($"Imperial ({Utility.FullUnit(newUnit, Utility.UnitType.Imperial)}{unitPrefix})", EditorStyles.miniBoldLabel);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();

			metricValue = EditorGUILayout.FloatField(imperialValue / Utility.UnitMultiplier(unit, Utility.UnitType.Imperial));

			if (unit != newUnit)
				unit = newUnit;

			imperialValue = EditorGUILayout.FloatField(metricValue * Utility.UnitMultiplier(unit, Utility.UnitType.Imperial));

			EditorGUILayout.EndHorizontal();
		}

		#endregion

		#endregion
	}
}
