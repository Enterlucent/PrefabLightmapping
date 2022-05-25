using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Editor Inspector for mainpulation the PrefabLightmapData component
/// </summary>
[CustomEditor(typeof(PrefabLightmapData)), CanEditMultipleObjects]
public partial class PrefabLightmapDataEditor : Editor
{
    /// <summary>
    /// Serialized Property for the lightmap slots
    /// </summary>
    private SerializedProperty serializedPrefabLightmapInfoSlots;
    /// <summary>
    /// Serialized Property for the loaded (or will be loaded at runtime) slot name
    /// </summary>
    private SerializedProperty serializedLoadedLightmapName;

    private void OnEnable()
    {
        this.serializedPrefabLightmapInfoSlots = this.serializedObject.FindProperty("PrefabLightmapInfoSlots");
        this.serializedLoadedLightmapName = this.serializedObject.FindProperty("LoadedLightmapName");
    }

    public override void OnInspectorGUI()
    {
        if (this.targets.Length == 1)
            EditorGUILayout.PropertyField(this.serializedPrefabLightmapInfoSlots, new GUIContent("Stored Lightmaps"));

        List<string> lightmapSlotNames = null;

        foreach (PrefabLightmapData prefabLightmapData in this.targets)
            lightmapSlotNames = this.GetSlotNameIntersection(prefabLightmapData, lightmapSlotNames);

        int currentLoadedLightmapName = this.GetLightmapSlotNameIndex("Default Lightmap", lightmapSlotNames, this.serializedLoadedLightmapName);

        if (currentLoadedLightmapName > -1 && lightmapSlotNames[currentLoadedLightmapName] != "—")
            this.serializedLoadedLightmapName.stringValue = lightmapSlotNames[currentLoadedLightmapName];

        if (!this.serializedLoadedLightmapName.hasMultipleDifferentValues && lightmapSlotNames[currentLoadedLightmapName] != "—")
        {
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Switch"))
                foreach (PrefabLightmapData prefabLightmap in this.targets)
                    if (!string.IsNullOrWhiteSpace(this.serializedLoadedLightmapName.stringValue))
                        prefabLightmap.Initialize(this.serializedLoadedLightmapName.stringValue);
        }

        this.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Finds the intersections between a list of PrefabLightmapInfoSlot names and the provided list of slot names
    /// </summary>
    /// <param name="data"><see cref="PrefabLightmapData"/> to inspect for slot names</param>
    /// <param name="lightmapSlotNames">A pre-created List of slots to look for the intersection of</param>
    /// <returns></returns>
    protected virtual List<string> GetSlotNameIntersection(PrefabLightmapData data, List<string> lightmapSlotNames)
    {
        List<string> intersection = new List<string>();

        if (data.PrefabLightmapInfoSlots == null || data.PrefabLightmapInfoSlots.Length < 1)
            return intersection;

        if (lightmapSlotNames == null)
            for (int i = 0; i < data.PrefabLightmapInfoSlots.Length; i++)
                intersection.Add(data.PrefabLightmapInfoSlots[i].Name);
        else
            for (int i = 0; i < data.PrefabLightmapInfoSlots.Length; i++)
                for (int j = 0; j < lightmapSlotNames.Count; j++)
                    if (data.PrefabLightmapInfoSlots[i].Name == lightmapSlotNames[j])
                        intersection.Add(data.PrefabLightmapInfoSlots[i].Name);

        return intersection;
    }

    /// <summary>
    /// Get the user selected lightmap slot index from a list of names.
    /// </summary>
    /// <param name="label">The text labled for the popup element</param>
    /// <param name="lightmapSlotNames">The names to select from</param>
    /// <param name="property">The <see cref="SerializedProperty"/> to set the selected value to.</param>
    /// <returns>Index into the string array or -1 if not found</returns>
    protected virtual int GetLightmapSlotNameIndex(string label, List<string> lightmapSlotNames, SerializedProperty property)
    {
        int value = -1;

        if (property.hasMultipleDifferentValues || lightmapSlotNames.Count < 1)
        {
            value = lightmapSlotNames.FindIndex(s => s == "—");

            if (value == -1)
            {
                value = lightmapSlotNames.Count;

                lightmapSlotNames.Add("—");
            }
        }
        else
        {
            value = lightmapSlotNames.FindIndex(s => s == property.stringValue);
        }

        return EditorGUILayout.Popup(label, value, lightmapSlotNames.ToArray());
    }
}
