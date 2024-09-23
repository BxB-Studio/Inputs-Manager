#region Namespaces

using UnityEngine;
using UnityEditor;

#endregion

namespace Utilities.Editor.Test
{
	public class LayersManagementAPITest : EditorWindow
	{
		#region Enumerators

		private enum InputType { Integer, String }
		private InputType currentInputType = InputType.Integer;

		#endregion

		#region Variables

		private int layerIndex;
		private string layerName = string.Empty;
		private string renameLayerTarget = string.Empty;   // For Rename: target layer (name or index)
		private string renameLayerNewName = string.Empty;  // For Rename: new name

		#endregion

		#region Utilities

		[MenuItem("Tools/Utilities/Layers Management API Test")]
		public static void ShowWindow()
		{
			GetWindow<LayersManagementAPITest>("Layers Management API Test");
		}

		private void TryAddLayer()
		{
			try
			{
				LayersManager.AddLayer(layerName);
				Debug.Log($"Layer '{layerName}' added successfully.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error adding layer: {ex.Message}");
			}
		}
		private void TryRemoveLayerByName()
		{
			try
			{
				LayersManager.RemoveLayer(layerName);
				Debug.Log($"Layer '{layerName}' removed successfully.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error removing layer by name: {ex.Message}");
			}
		}
		private void TryRemoveLayerByIndex()
		{
			try
			{
				LayersManager.RemoveLayer(layerIndex);
				Debug.Log($"Layer at index '{layerIndex}' removed successfully.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error removing layer by index: {ex.Message}");
			}
		}
		private void TryRenameLayer()
		{
			try
			{
				if (currentInputType == InputType.String)
				{
					LayersManager.RenameLayer(renameLayerTarget, renameLayerNewName);
					Debug.Log($"Layer '{renameLayerTarget}' renamed to '{renameLayerNewName}' successfully.");
				}
				else
				{
					// Convert renameLayerTarget to an int safely
					if (int.TryParse(renameLayerTarget, out int targetLayerIndex))
					{
						LayersManager.RenameLayer(targetLayerIndex, renameLayerNewName);
						Debug.Log($"Layer at index '{targetLayerIndex}' renamed to '{renameLayerNewName}' successfully.");
					}
					else
					{
						Debug.LogError("Invalid index entered for renaming. Please enter a valid integer.");
					}
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error renaming layer: {ex.Message}");
			}
		}
		private void TryCheckIfLayerIsEmpty()
		{
			try
			{
				bool isEmpty = LayersManager.IsLayerEmpty(layerIndex);
				Debug.Log($"Layer at index '{layerIndex}' is {(isEmpty ? "empty" : "not empty")}.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Error checking if layer is empty: {ex.Message}");
			}
		}

		#endregion

		#region Methods

		private void OnGUI()
		{
			GUILayout.Label("Layers Management API Test", EditorStyles.boldLabel);

			// Popup to switch between Integer and String inputs
			currentInputType = (InputType)EditorGUILayout.EnumPopup("Input Type:", currentInputType);

			// Based on the selection, show either integer or string input for the main operations
			if (currentInputType == InputType.Integer)
			{
				layerIndex = EditorGUILayout.IntField("Layer Index:", layerIndex);
			}
			else
			{
				layerName = EditorGUILayout.TextField("Layer Name:", layerName);
			}

			GUILayout.Space(10);

			// Buttons for various API functions
			if (GUILayout.Button("Add Layer"))
			{
				TryAddLayer();
			}

			if (currentInputType == InputType.String)
			{
				if (GUILayout.Button("Remove Layer by Name"))
				{
					TryRemoveLayerByName();
				}
			}
			else
			{
				if (GUILayout.Button("Remove Layer by Index"))
				{
					TryRemoveLayerByIndex();
				}
			}

			GUILayout.Space(20);

			// Section for renaming layers: provide target layer (name or index) and the new name
			GUILayout.Label("Rename Layer", EditorStyles.boldLabel);
			if (currentInputType == InputType.String)
			{
				renameLayerTarget = EditorGUILayout.TextField("Target Layer Name:", renameLayerTarget);
			}
			else
			{
				renameLayerTarget = EditorGUILayout.TextField("Target Layer Index:", renameLayerTarget);
			}
			renameLayerNewName = EditorGUILayout.TextField("New Name:", renameLayerNewName);

			if (GUILayout.Button("Rename Layer"))
			{
				TryRenameLayer();
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Check if Layer is Empty"))
			{
				TryCheckIfLayerIsEmpty();
			}
		}

		#endregion
	}
}
