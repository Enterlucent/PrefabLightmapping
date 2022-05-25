using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Unity component for managing lightmap data for prefabs
/// </summary>
public partial class PrefabLightmapData : MonoBehaviour
{
    /// <summary>
    /// The name of the default lightmap slot to load if one is not specified 
    /// </summary>
    public const string DefaultPrefabLightmapName = "Default";

    /// <summary>
    /// The array of lightmap data assigned to this component
    /// </summary>
    public PrefabLightmapInfoSlot[] PrefabLightmapInfoSlots;

    /// <summary>
    /// The name of the lightmap to load or that is loaded
    /// </summary>
    public string LoadedLightmapName;

    /// <summary>
    /// The index of the lightmap slot that has been loaded
    /// </summary>
    private int loadedIndex;

    /// <summary>
    /// Array of reference to the <see cref="Renderer"/> found in the prefab's children
    /// </summary>
    private Renderer[] renderers;

    private void Awake()
    {
        if (String.IsNullOrWhiteSpace(this.LoadedLightmapName))
            this.LoadedLightmapName = PrefabLightmapData.DefaultPrefabLightmapName;

        this.renderers = this.gameObject.GetComponentsInChildren<Renderer>();

        this.Initialize(this.LoadedLightmapName);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    /// <summary>
    /// Get the index of a given slot name within the prefab data array
    /// </summary>
    /// <param name="name">Name of the slot to find the index for.</param>
    /// <returns>The index within the <see cref="PrefabLightmapInfoSlot">PrefabLightmapInfoSlot</see> array or -1 if not found</returns>
    public int SlotNameToIndex(string name)
    {
        for (int i = 0; i < this.PrefabLightmapInfoSlots.Length; i++)
            if (this.PrefabLightmapInfoSlots[i].Name == name)
                return i;
        return -1;
    }

    /// <summary>
    /// Initialize the lightmap data for this prefab
    /// </summary>
    /// <param name="name">Name of the lightmap data slot</param>
    public virtual void Initialize(string name)
    {
        this.loadedIndex = -1;

        if (!String.IsNullOrWhiteSpace(name) && this.PrefabLightmapInfoSlots.Length > 0)
        {
            this.loadedIndex = this.SlotNameToIndex(name);

            if (loadedIndex > -1)
            {
                this.LoadedLightmapName = name;
            }
            else
            {
                Debug.LogWarning("The provided slot name " + name + " doesn't not exist on this object.e", this);

                return;
            }
        }

        this.InitializeLoaded();
    }
    /// <summary>
    /// Initialize the lightmap data for this prefab (suitable for being called from Unity animations)
    /// </summary>
    /// <param name="name">Name of the lightmap data slot</param>
    /// <param name="resetRenderers">Reset the MaterialPropertyBlock for the <see cref="Renderer"/>s </param>
    public void ChangePrefabLightmap(string name, bool resetRenderers = true)
    {
        this.Initialize(name);

        if (resetRenderers == true)
            for (int i = 0; i < this.renderers.Length; i++)
                this.renderers[i].SetPropertyBlock(null);
    }

    /// <summary>
    /// Initalize the lightmap data when the scene is loaded
    /// </summary>
    /// <param name="scene">The scene that has been loaded</param>
    /// <param name="mode">The mode the scene was loaded with</param>
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.InitializeLoaded();
    }

    /// <summary>
    /// Initilize the lightmap from the <see cref="loadedIndex"/> value
    /// </summary>
    protected virtual void InitializeLoaded()
    {
        if (this.PrefabLightmapInfoSlots.Length > 0)
            if (this.loadedIndex > -1 && this.loadedIndex < this.PrefabLightmapInfoSlots.Length)
                this.InjectLightData(this.PrefabLightmapInfoSlots[this.loadedIndex].Data);
    }

    /// <summary>
    /// Load the lightmap data into the scene lighting infrastructure
    /// </summary>
    /// <param name="lightmapData">Lightmap data slot to inject into the scene's light data</param>
    protected virtual void InjectLightData(PrefabLightmapInfo lightmapData)
    {
        if (lightmapData.RendererData == null || lightmapData.RendererData.Length == 0)
            return;

        LightmapData[] lightmaps = LightmapSettings.lightmaps;

        int totalLightmaps = LightmapSettings.lightmaps.Length;
        int[] offsetsIndexes = new int[lightmapData.Lightmaps.Length];

        List<LightmapData> localLightmaps = new List<LightmapData>();

        for (int i = 0; i < lightmapData.Lightmaps.Length; i++)
        {
            bool exists = false;

            for (int j = 0; j < lightmaps.Length; j++)
            {
                if (lightmapData.Lightmaps[i] == lightmaps[j].lightmapColor)
                {
                    exists = true;

                    offsetsIndexes[i] = j;
                }
            }

            if (!exists)
            {
                offsetsIndexes[i] = totalLightmaps;

                localLightmaps.Add(new LightmapData
                {
                    lightmapColor = lightmapData.Lightmaps[i],
                    lightmapDir = lightmapData.DirectionalLightmaps.Length == lightmapData.Lightmaps.Length ? lightmapData.DirectionalLightmaps[i] : default(Texture2D),
                    shadowMask = lightmapData.ShadowMasks.Length == lightmapData.Lightmaps.Length ? lightmapData.ShadowMasks[i] : default(Texture2D),
                });

                totalLightmaps++;
            }
        }

        LightmapData[] combinedLightmaps = new LightmapData[totalLightmaps];

        lightmaps.CopyTo(combinedLightmaps, 0);

        localLightmaps.ToArray().CopyTo(combinedLightmaps, lightmaps.Length);

        bool directional = true;

        for (int i = 0; i < lightmapData.DirectionalLightmaps.Length; i++)
        {
            if (lightmapData.DirectionalLightmaps[i] == null)
            {
                directional = false;
                break;
            }
        }

        LightmapSettings.lightmapsMode = (lightmapData.DirectionalLightmaps.Length == lightmapData.Lightmaps.Length && directional) ? LightmapsMode.CombinedDirectional : LightmapsMode.NonDirectional;

        for (int i = 0; i < lightmapData.RendererData.Length; i++)
        {
            PrefabLightmapRendererInfo renderData = lightmapData.RendererData[i];

            renderData.Renderer.lightmapIndex = offsetsIndexes[renderData.LightmapIndex];
            renderData.Renderer.lightmapScaleOffset = renderData.LightmapOffsetScale;

            Material[] materials = renderData.Renderer.sharedMaterials;

            for (int j = 0; j < materials.Length; j++)
                if (materials[j] != null && Shader.Find(materials[j].shader.name) != null)
                    materials[j].shader = Shader.Find(materials[j].shader.name);
        }

        for (int i = 0; i < lightmapData.LightData.Length; i++)
        {
            if (lightmapData.LightData[i].Light == null)
                continue;

            lightmapData.LightData[i].Light.bakingOutput = new LightBakingOutput
            {
                isBaked = true,
                lightmapBakeType = (LightmapBakeType)lightmapData.LightData[i].LightmapBakeType,
                mixedLightingMode = (MixedLightingMode)lightmapData.LightData[i].MixedLightingMode
            };
        }

        LightmapSettings.lightmaps = combinedLightmaps;
    }
}
