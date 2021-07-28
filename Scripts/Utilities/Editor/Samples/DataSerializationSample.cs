using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Utilities.Editor
{
	internal class DataSerializationSample : EditorWindow
	{
		#region Modules

		[Serializable]
		private class DataSample
		{
			public string firstName;
			public string lastName;
			public int age;
		}

		#endregion

		#region Variables

		private DataSerializationUtility<DataSample> serializationUtility;
		private DataSample data;
		private string DataPath => $"{Application.dataPath}/Resources/Assets/DataSample.data";

		#endregion

		#region Methods

		[MenuItem("Tools/Utilities/Debug/Data File Sample...")]
		public static void ShowWindow()
		{
			GetWindow<DataSerializationSample>(true, "Data File Sample").Show();
		}

		private void Load()
		{
			if (!serializationUtility)
				serializationUtility = new DataSerializationUtility<DataSample>(DataPath, false);

			data = serializationUtility.Load();
		}
		private void Save()
		{
			if (!serializationUtility)
				serializationUtility = new DataSerializationUtility<DataSample>(DataPath, false);

			serializationUtility.SaveOrCreate(data);

			AssetDatabase.Refresh();
		}
		private void OnGUI()
		{

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Data Serialization Sample");
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GUI.skin.box);

			if (data != null)
			{
				data.firstName = EditorGUILayout.TextField("First Name", data.firstName);
				data.lastName = EditorGUILayout.TextField("Last Name", data.lastName);
				data.age = Utility.ClampInfinity(EditorGUILayout.IntField("Age", data.age));

				if (GUILayout.Button("Serialize Data"))
				{
					Save();

					data = null;
				}
			}
			else
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.TextField("First Name", string.Empty);
				EditorGUILayout.TextField("Last Name", string.Empty);
				EditorGUILayout.IntField("Age", 0);
				EditorGUI.EndDisabledGroup();

				if (File.Exists(DataPath))
				{
					if (GUILayout.Button("Deserialize Data"))
						Load();
				}
				else
					data = new DataSample();
			}

			EditorGUILayout.EndVertical();
		}

		#endregion
	}
}