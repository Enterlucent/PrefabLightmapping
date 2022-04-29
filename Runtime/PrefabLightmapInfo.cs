using System;
using UnityEngine;

/// <summary>
/// Lightmap slot data container
/// </summary>
[Serializable]
public struct PrefabLightmapInfo
{
    /// <summary>
    /// The lightmaps for this prefab
    /// </summary>
    public Texture2D[] Lightmaps;
    /// <summary>
    /// The directional lightmaps for this prefab
    /// </summary>
    public Texture2D[] DirectionalLightmaps;
    /// <summary>
    /// The shadowmasks for this prefab
    /// </summary>
    public Texture2D[] ShadowMasks;

    /// <summary>
    /// The <see cref="Light"/>s found within the children of this prefab
    /// </summary>
    public PrefabLightmapLightInfo[] LightData;
    /// <summary>
    /// The <see cref="Renderer"/>s found within the children of this prefab
    /// </summary>
    public PrefabLightmapRendererInfo[] RendererData;
}
