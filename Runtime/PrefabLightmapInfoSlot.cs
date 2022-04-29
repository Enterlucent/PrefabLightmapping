using System;

/// <summary>
/// Name identifiable lightmap data for a specific prefab
/// </summary>
[Serializable]
public struct PrefabLightmapInfoSlot
{
    /// <summary>
    /// Name of the lightmap slot
    /// </summary>
    public string Name;
    /// <summary>
    /// Lightmap data for a particular slot
    /// </summary>
    public PrefabLightmapInfo Data;
}
