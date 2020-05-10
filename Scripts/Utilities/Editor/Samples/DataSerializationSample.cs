using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Utilities
{
	internal class DataSerializationSample : EditorWindow
	{
		[Serializable]
		private class DataSample
		{
			public string firstName;
			public string lastName;
			public int age;
		}

		private DataSerializationUtility<DataSample> serializationUtility;
		private DataSample data;
		private string DataPath => $"{Application.dataPath}/Resources/Assets/DataSample.data";

		[MenuItem("Tools/Utilities/Debug/Data File Sample...")]
		public static void ShowWindow()
		{
			GetWindow<DataSerializationSample>("Data File Sample").Show();
		}

		private void Load()
		{
			string directory = Path.GetDirectoryName(DataPath);
			string fileName = Path.GetFileName(DataPath);

			if (!serializationUtility)
				serializationUtility = new DataSerializationUtility<DataSample>(directory, fileName);

			data = serializationUtility.Load();
		}
		private void Save()
		{
			string directory = Path.GetDirectoryName(DataPath);
			string fileName = Path.GetFileName(DataPath);

			if (!serializationUtility)
				serializationUtility = new DataSerializationUtility<DataSample>(directory, fileName);

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
	}
}