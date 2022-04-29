using System;
using UnityEngine;

/// <summary>
/// Light information for the prefab
/// </summary>
[Serializable]
public struct PrefabLightmapLightInfo
{
    /// <summary>
    /// Reference to the light used in the prefab bake
    /// </summary>
    public Light Light;
    /// <summary>
    /// The type of the lightmap bake
    /// </summary>
    public int LightmapBakeType;
    /// <summary>
    /// The lighting mode use for this bake
    /// </summary>
    public int MixedLightingMode;
}
