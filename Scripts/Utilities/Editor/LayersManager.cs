#region Namespaces

using System;
using System.Data;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

#endregion

namespace Utilities.Editor
{
	public static class LayersManager
	{
		#region Constants

		public const int MaxLayersCount = 32; // Total number of layers in Unity

		#endregion

		#region Variables

		// Built-in layers have specific names and indices
		private static readonly string[] BuiltInLayerNames = { "Default", "TransparentFX", "Ignore Raycast", "Water", "UI" };
		private static readonly int[] BuiltInLayerIndices = { 0, 1, 2, 4, 5 };

		#endregion

		#region Methods

		public static void AddLayer(string name)
		{
			// Get current layers
			string[] layers = GetLayersFromTagManager();
			
			// Check if layer name already exists
			foreach (var layer in layers)
			{
				if (layer.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					throw new DuplicateNameException($"Layer '{name}' already exists.");
				}
			}

			// Find first empty slot in the layer array (starting from index 8 to avoid built-in layers)
			int emptySlotIndex = FindEmptyLayerSlot(layers);
			if (emptySlotIndex == -1)
			{
				throw new Exception("Error adding layer: No empty layer slots available.");
			}

			// Add the layer at the first empty slot
			layers[emptySlotIndex] = name;

			// Save the updated layers to TagManager
			SetLayers(layers);
		}
		public static void RemoveLayer(int layerIndex)
		{
			if (Array.Exists(BuiltInLayerIndices, index => index == layerIndex))
			{
				throw new ArgumentException($"Cannot remove built-in layer at index {layerIndex}.");
			}

			string[] layers = GetLayersFromTagManager();
			if (layerIndex < 0 || layerIndex >= layers.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(layerIndex), $"Layer index {layerIndex} is out of range.");
			}

			layers[layerIndex] = string.Empty; // Mark the layer as empty
			SetLayers(layers); // Save the updated layers
		}
		public static void RemoveLayer(string name)
		{
			// Check if the name is one of the built-in layers
			if (Array.Exists(BuiltInLayerNames, builtInName => builtInName.Equals(name, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ArgumentException($"Cannot remove built-in layer with name '{name}'.");
			}

			int layerIndex = LayerMask.NameToLayer(name);
			if (layerIndex == -1)
			{
				throw new Exception($"Layer '{name}' not found.");
			}

			RemoveLayer(layerIndex); // Reuse RemoveLayer by index
		}
		public static void RenameLayer(int layerIndex, string name)
		{
			// Validate index
			if (layerIndex < 0 || layerIndex >= MaxLayersCount)
			{
				throw new ArgumentOutOfRangeException(nameof(layerIndex), $"Layer index {layerIndex} is out of range.");
			}

			// Check if it's a built-in layer
			if (Array.Exists(BuiltInLayerIndices, index => index == layerIndex))
			{
				throw new ArgumentException($"Cannot rename built-in layer at index {layerIndex}.");
			}

			// Check if new name already exists
			string[] layers = GetLayersFromTagManager();
			foreach (var layer in layers)
			{
				if (layer.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					throw new DuplicateNameException($"Layer '{name}' already exists.");
				}
			}

			// Set the new name
			layers[layerIndex] = name;
			SetLayers(layers);
		}
		public static void RenameLayer(string currentName, string newName)
		{
			int layerIndex = LayerMask.NameToLayer(currentName);
			if (layerIndex == -1)
			{
				throw new Exception($"Layer '{currentName}' not found.");
			}

			RenameLayer(layerIndex, newName);
		}
		public static bool IsLayerEmpty(int layerIndex)
		{
			if (layerIndex < 0 || layerIndex >= MaxLayersCount)
			{
				throw new ArgumentOutOfRangeException(nameof(layerIndex), $"Layer index {layerIndex} is out of range.");
			}

			string[] layers = GetLayersFromTagManager();
			return string.IsNullOrEmpty(layers[layerIndex]);
		}
		public static bool LayerExists(string name)
		{
			return LayerMask.NameToLayer(name) > -1;
		}
		public static string[] GetLayers()
		{
			// Returning the currently used layers without empty slots
			return InternalEditorUtility.layers;
		}

		private static string[] GetLayersFromTagManager()
		{
			// Get the TagManager.asset file, which contains all the layers
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty layersProp = tagManager.FindProperty("layers");

			string[] layers = new string[MaxLayersCount];
			for (int i = 0; i < MaxLayersCount; i++)
			{
				layers[i] = layersProp.GetArrayElementAtIndex(i).stringValue;
			}

			return layers;
		}
		private static void SetLayers(string[] layers)
		{
			// Save the layers back to TagManager.asset
			SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty layersProp = tagManager.FindProperty("layers");

			for (int i = 0; i < MaxLayersCount; i++)
			{
				layersProp.GetArrayElementAtIndex(i).stringValue = layers[i];
			}

			tagManager.ApplyModifiedProperties();
		}
		private static int FindEmptyLayerSlot(string[] layers)
		{
			// Start searching from index 8 to avoid built-in layers (0-7 are reserved)
			for (int i = 0; i < MaxLayersCount; i++)
			{
				if (string.IsNullOrEmpty(layers[i]) || layers[i] == " ") // Allow space as valid empty layer
				{
					return i;
				}
			}

			return -1; // No empty slot found
		}

		#endregion
	}
}
