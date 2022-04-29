using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Editor Inspector for mainpulation the PrefabLightmapData component
/// </summary>
[CustomEditor(typeof(PrefabLightmapData))]
public class PrefabLightmapDataEditor : Editor
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
        PrefabLightmapData prefabLightmap = (PrefabLightmapData)this.target;

        EditorGUILayout.PropertyField(this.serializedPrefabLightmapInfoSlots, new GUIContent("Stored Lightmaps"));

        if (prefabLightmap.PrefabLightmapInfoSlots != null && prefabLightmap.PrefabLightmapInfoSlots.Length > 0)
        {
            string[] loadedLIghtmapNameChoices = new string[prefabLightmap.PrefabLightmapInfoSlots.Length];
            int currentSelection = 0;

            for (int i = 0; i < prefabLightmap.PrefabLightmapInfoSlots.Length; i++)
            {
                loadedLIghtmapNameChoices[i] = prefabLightmap.PrefabLightmapInfoSlots[i].Name;

                if (prefabLightmap.PrefabLightmapInfoSlots[i].Name == this.serializedLoadedLightmapName.stringValue)
                    currentSelection = i;
            }

            this.serializedLoadedLightmapName.stringValue = prefabLightmap.PrefabLightmapInfoSlots[EditorGUILayout.Popup("Default Lightmap", currentSelection, loadedLIghtmapNameChoices)].Name;

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Switch"))
                prefabLightmap.Initialize(this.serializedLoadedLightmapName.stringValue);
        }

        this.serializedObject.ApplyModifiedProperties();
    }
}
