using System;
using UnityEngine;

/// <summary>
/// Renderer information for the lightmap bake
/// </summary>
[Serializable]
public struct PrefabLightmapRendererInfo
{
    /// <summary>
    /// Reference to the prefab renderer
    /// </summary>
    public Renderer Renderer;
    /// <summary>
    /// The index to the lightmap data of the scene
    /// </summary>
    public int LightmapIndex;
    /// <summary>
    /// Offset and scale of the UV assigned to the lightmap data
    /// </summary>
    public Vector4 LightmapOffsetScale;
}
