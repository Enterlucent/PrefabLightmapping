using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Logic used to drive the PrefabLightmapTool and it's public functions
/// </summary>
public class PrefabLightmapTool : EditorWindow
{
    /// <summary>
    /// Internal class used for managing the <see cref="PrefabLightmapData"/> components found within the scene
    /// </summary>
    protected class PrefabLightmapDataItem
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id;
        /// <summary>
        /// Found <see cref="PrefabLightmapData"/> component in the scene
        /// </summary>
        public PrefabLightmapData PrefabLightmap;
        /// <summary>
        /// If the item is selected
        /// </summary>
        public bool Selected;
    }

    /// <summary>
    /// The most recent value of the export path
    /// </summary>
    [SerializeField]
    protected string LastTextPath;
    /// <summary>
    /// The most recent value of the slot name
    /// </summary>
    [SerializeField]
    protected string LastTextName;
    /// <summary>
    /// The most recent "default value" editor checkbox value
    /// </summary>
    [SerializeField]
    protected bool LastDefault = true;

    /// <summary>
    /// Internal value for determining if the bake was started as a result of this tool
    /// </summary>
    protected bool StartedBaking;
    /// <summary>
    /// If the GUI has been loaded by CreateGUI
    /// </summary>
    protected bool UILoaded;

    /// <summary>
    /// Export path text field
    /// </summary>
    protected TextField TextFieldPath;
    /// <summary>
    /// Export path direcotry chooser button
    /// </summary>
    protected Button ButtonPath;
    /// <summary>
    /// Slot name text field
    /// </summary>
    protected TextField TextFieldName;
    /// <summary>
    /// Slot name default toggle
    /// </summary>
    protected Toggle ToggleDefault;
    /// <summary>
    /// Listview refresh button
    /// </summary>
    protected Button ButtonReload;
    /// <summary>
    /// ListView of <see cref="PrefabLightmapData"/> component in the current scene
    /// </summary>
    protected ListView ListViewBehaviours;
    /// <summary>
    /// Select/Deselect all values in the ListView toggle
    /// </summary>
    protected Toggle ToggleAll;
    /// <summary>
    /// Clear slot information button
    /// </summary>
    protected Button ButtonClear;
    /// <summary>
    /// Baked slot information button
    /// </summary>
    protected Button ButtonBake;

    /// <summary>
    /// Static function for registering with the Editor interface
    /// </summary>
    [UnityEditor.MenuItem("Window/Rendering/Prefab Lightmap Tool")]
    protected static void ShowPrefabLightmapTool()
    {
        EditorWindow wnd = GetWindow<PrefabLightmapTool>();
        wnd.titleContent = new GUIContent("Prefab Lightmap Tool");
        wnd.minSize = new Vector2(330, 300);
    }

    public void CreateGUI()
    {
        bool found = false;
        string assetPath = null;
        string[] assetGuids = AssetDatabase.FindAssets("PrefabLightmapTool");

        foreach (string assetGuid in assetGuids)
        {
            assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

            if (assetPath.Contains(".uxml"))
            {
                found = true;
                break;
            }
        }

        if (found == false)
        {
            this.rootVisualElement.Add(new Label("Unable to find the visiualisation tree for the PrefabLightmapTool."));

            return;
        }

        if (string.IsNullOrWhiteSpace(this.LastTextPath))
            this.LastTextPath = this.DefaultExportDirectory();

        if (string.IsNullOrWhiteSpace(this.LastTextName))
            this.LastTextName = PrefabLightmapData.DefaultPrefabLightmapName;

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
        visualTree.CloneTree(this.rootVisualElement);

        this.TextFieldPath = this.rootVisualElement.Q<TextField>(name = "TextPath");
        this.ButtonPath = this.rootVisualElement.Q<Button>(name = "ButtonPath");
        this.TextFieldName = this.rootVisualElement.Q<TextField>(name = "TextName");
        this.ToggleDefault = this.rootVisualElement.Q<Toggle>(name = "ToggleDefault");
        this.ButtonReload = this.rootVisualElement.Q<Button>(name = "ButtonReload");
        this.ListViewBehaviours = this.rootVisualElement.Q<ListView>(name = "ListBehaviours");
        this.ToggleAll = this.rootVisualElement.Q<Toggle>(name = "ToggleAll");
        this.ButtonClear = this.rootVisualElement.Q<Button>(name = "ButtonClear");
        this.ButtonBake = this.rootVisualElement.Q<Button>(name = "ButtonBake");

        this.TextFieldPath.SetEnabled(false);
        this.TextFieldName.SetEnabled(false);
        this.ToggleAll.SetEnabled(false);
        this.ButtonClear.SetEnabled(false);
        this.ButtonBake.SetEnabled(false);

        this.TextFieldPath.value = this.LastTextPath;
        this.ToggleDefault.value = this.LastDefault;

        this.TextFieldPath.RegisterValueChangedCallback(this.TextPathChangedEventHandler);
        this.ButtonPath.clicked += this.ButtonPathClicked;
        this.TextFieldName.RegisterValueChangedCallback(this.TextNameChangedEventHandler);
        this.ToggleDefault.RegisterValueChangedCallback(this.ToggleDefaltChangedEventHandler);
        this.ButtonReload.clicked += this.ButtonReloadClicked;
        this.ToggleAll.RegisterValueChangedCallback(this.ToggleAllChangedEventHandler);
        this.ButtonClear.clicked += this.ButtonClearClicked;
        this.ButtonBake.clicked += this.ButtonBakeClicked;

        this.UpdateListView();

        this.UILoaded = true;
    }
    public void OnFocus()
    {
        this.UpdateListView();
    }

    /// <summary>
    /// Allow caller to use the directory chooser to assign the <see cref="LastTextPath"/> value
    /// </summary>
    protected void ButtonPathClicked()
    {
        string path = EditorUtility.OpenFolderPanel("Choose Lightmap Export Location", this.TextFieldPath.value, "PrefabLightmaps");

        int assetIndex = path.LastIndexOf("Assets");

        if (!this.ExportDirectoryIsRelative(path))
        {
            Debug.LogError("Location must be relative to the project folder.", this);

            return;
        }

        this.LastTextPath = path[assetIndex..];
        this.TextFieldPath.value = this.LastTextPath;
    }
    /// <summary>
    /// Refresh the ListView items on user demand
    /// </summary>
    protected void ButtonReloadClicked()
    {
        this.UpdateListView();
    }
    /// <summary>
    /// Clear the slot informaiton from all selected items
    /// </summary>
    protected void ButtonClearClicked()
    {
        foreach (PrefabLightmapDataItem item in this.ListViewBehaviours.itemsSource)
        {
            PrefabLightmapData prefabLightmap = item.PrefabLightmap as PrefabLightmapData;

            if (prefabLightmap != null)
            {
                for (int i = 0; i < prefabLightmap.PrefabLightmapInfoSlots.Length; i++)
                {
                    if (prefabLightmap.PrefabLightmapInfoSlots[i].Name == this.TextFieldName.text)
                    {
                        PrefabLightmapTool.ClearLightmapData(prefabLightmap, i);

                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Bake slot information for all selected items
    /// </summary>
    protected void ButtonBakeClicked()
    {
        if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked your lightmaps and Auto mode is disabled.");

            return;
        }

        this.StartedBaking = true;

        Lightmapping.bakeCompleted += this.UpdatePrefabs;
        Lightmapping.BakeAsync();
    }

    /// <summary>
    /// Bound change event for the export path text field
    /// </summary>
    /// <param name="changeEvent">Unity provided change event</param>
    protected void TextPathChangedEventHandler(ChangeEvent<string> changeEvent)
    {
        if (this.ExportDirectoryIsRelative(changeEvent.newValue))
            this.LastTextPath = changeEvent.newValue;
        else
            this.LastTextPath = this.DefaultExportDirectory();

        this.TextFieldPath.value = this.LastTextPath;
    }
    /// <summary>
    /// Bound change event for the slot name text field
    /// </summary>
    /// <param name="changeEvent">Unity provided change event</param>
    protected void TextNameChangedEventHandler(ChangeEvent<string> changeEvent)
    {
        if (this.ToggleDefault.value == false)
        {
            if (string.IsNullOrWhiteSpace(changeEvent.newValue))
                this.LastTextName = PrefabLightmapData.DefaultPrefabLightmapName;
            else
                this.LastTextName = changeEvent.newValue;

            this.TextFieldName.value = this.LastTextName;
        }
    }
    /// <summary>
    /// Bound change event for the default slot name enabler/disabler
    /// </summary>
    /// <param name="changeEvent">Unity provided change event</param>
    protected void ToggleDefaltChangedEventHandler(ChangeEvent<bool> changeEvent)
    {
        this.LastDefault = changeEvent.newValue;

        if (changeEvent.newValue == true)
        {
            this.TextFieldName.value = PrefabLightmapData.DefaultPrefabLightmapName;
            this.TextFieldName.SetEnabled(false);
        }
        else
        {
            this.TextFieldName.SetEnabled(true);
            this.TextFieldName.value = this.LastTextName;
        }
    }
    /// <summary>
    /// Bound change event for the select/deselect all toggle
    /// </summary>
    /// <param name="changeEvent">Unity provided change event</param>
    protected void ToggleAllChangedEventHandler(ChangeEvent<bool> changeEvent)
    {
        if (this.ListViewBehaviours.itemsSource.Count > 0)
        {
            foreach (PrefabLightmapDataItem item in this.ListViewBehaviours.itemsSource)
            {
                if (changeEvent.newValue == true)
                {
                    item.Selected = true;

                    this.ButtonClear.SetEnabled(true);
                    this.ButtonBake.SetEnabled(true);
                }
                else
                {
                    item.Selected = false;

                    this.ButtonClear.SetEnabled(false);
                    this.ButtonBake.SetEnabled(false);
                }
            }

            this.ListViewBehaviours.Rebuild();
        }
    }

    /// <summary>
    /// Update the list view by comparing what's currently in it vs what is in the scene
    /// </summary>
    protected void UpdateListView()
    {
        if (this.UILoaded == false)
            return;

        List<PrefabLightmapDataItem> items = this.FindPrefabLightmapItems();

        if (items.Count > 0)
            this.ToggleAll.SetEnabled(true);
        else
            this.ToggleAll.SetEnabled(false);

        this.ListViewBehaviours.fixedItemHeight = 23;
        this.ListViewBehaviours.itemsSource = items;
        this.ListViewBehaviours.makeItem = () =>
        {
            VisualElement visualElement = new VisualElement();
            Toggle toggle = new Toggle() { name = "ItemSelection" };
            Label label = new Label() { name = "ItemLabel" };

            visualElement.style.flexDirection = FlexDirection.Row;

            label.style.marginLeft = 5;
            label.style.alignSelf = Align.Center;

            toggle.Add(label);

            visualElement.Add(toggle);

            return visualElement;
        };
        this.ListViewBehaviours.bindItem = (e, i) =>
        {
            PrefabLightmapDataItem data = this.ListViewBehaviours.itemsSource[i] as PrefabLightmapDataItem;
            Toggle toggle = e.Q(name = "ItemSelection") as Toggle;
            Label label = e.Q(name = "ItemLabel") as Label;

            label.text = data.PrefabLightmap.gameObject.name;
            toggle.value = data.Selected;

            toggle.RegisterValueChangedCallback(e =>
            {
                data.Selected = e.newValue;

                if (this.HasSelectedItems())
                {
                    if (this.AllItemsSelected())
                        this.ToggleAll.SetValueWithoutNotify(true);

                    this.ButtonClear.SetEnabled(true);
                    this.ButtonBake.SetEnabled(true);
                }
                else
                {
                    this.ButtonClear.SetEnabled(false);
                    this.ButtonBake.SetEnabled(false);
                    this.ToggleAll.SetValueWithoutNotify(false);
                }
            });
        };

        this.ListViewBehaviours.Rebuild();
    }
    /// <summary>
    /// UPdate the prefab with the lighting data and save the prefab to the file system
    /// </summary>
    protected void UpdatePrefabs()
    {
        if (this.StartedBaking == false)
            return;

        int counter = 0;

        foreach (PrefabLightmapDataItem item in this.ListViewBehaviours.itemsSource)
        {
            counter++;

            string itemName = item.PrefabLightmap.gameObject.name + "-" + counter;

            if (item.Selected == false) continue;

            PrefabLightmapInfo data = PrefabLightmapTool.GatherLightmapData(item.PrefabLightmap.gameObject, this.TextFieldPath.value, itemName, this.TextFieldName.value);

            PrefabLightmapData prefabLightmap = item.PrefabLightmap as PrefabLightmapData;

            if (prefabLightmap != null)
            {
                int index = -1;

                if (prefabLightmap.PrefabLightmapInfoSlots != null)
                {
                    for (int i = 0; i < prefabLightmap.PrefabLightmapInfoSlots.Length; i++)
                    {
                        if (prefabLightmap.PrefabLightmapInfoSlots[i].Name == this.TextFieldName.value)
                            index = i;

                        if (index > -1) break;
                    }
                }

                if (index < 0)
                {
                    index = prefabLightmap.PrefabLightmapInfoSlots == null ? 0 : prefabLightmap.PrefabLightmapInfoSlots.Length;

                    Array.Resize(ref prefabLightmap.PrefabLightmapInfoSlots, index + 1);
                }

                prefabLightmap.PrefabLightmapInfoSlots[index] = new PrefabLightmapInfoSlot()
                {
                    Name = this.TextFieldName.value,
                    Data = data
                };

                GameObject targetPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(item.PrefabLightmap.gameObject) as GameObject;

                if (targetPrefab != null)
                {
                    GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(item.PrefabLightmap.gameObject);

                    if (root != null)
                    {
                        GameObject rootPrefab = PrefabUtility.GetCorrespondingObjectFromSource(item.PrefabLightmap.gameObject);

                        string rootPath = AssetDatabase.GetAssetPath(rootPrefab);

                        PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(root, PrefabUnpackMode.OutermostRoot);

                        try
                        {
                            PrefabUtility.ApplyPrefabInstance(item.PrefabLightmap.gameObject, InteractionMode.AutomatedAction);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            PrefabUtility.SaveAsPrefabAssetAndConnect(root, rootPath, InteractionMode.AutomatedAction);
                        }
                    }
                    else
                    {
                        PrefabUtility.ApplyPrefabInstance(item.PrefabLightmap.gameObject, InteractionMode.AutomatedAction);
                    }
                }
            }
        }

        this.StartedBaking = false;
    }

    /// <summary>
    /// Find all <see cref="PrefabLightmapData"/> componets in the scene and return any additions/removals
    /// </summary>
    /// <returns>All currently enabled <see cref="PrefabLightmapData"/> components in the scene</returns>
    protected List<PrefabLightmapDataItem> FindPrefabLightmapItems()
    {
        List<PrefabLightmapDataItem> items = null;

        if (this.ListViewBehaviours != null && this.ListViewBehaviours.itemsSource != null)
            items = this.ListViewBehaviours.itemsSource as List<PrefabLightmapDataItem>;

        PrefabLightmapData[] prefabLightmaps = GameObject.FindObjectsOfType<PrefabLightmapData>();

        if (items != null)
        {
            for (int i = items.Count - 1; i > -1; i--)
            {
                bool found = false;

                for (int j = 0; j < prefabLightmaps.Length; j++)
                    if (items[i].Id == prefabLightmaps[j].GetInstanceID() && items[i].PrefabLightmap != null)
                    {
                        found = true;
                        break;
                    }

                if (found == false)
                    items.RemoveAt(i);
            }
        }
        else items = new List<PrefabLightmapDataItem>();

        foreach (PrefabLightmapData prefablightmap in prefabLightmaps)
        {
            bool found = false;

            if (items != null)
                for (int i = 0; i < items.Count; i++)
                    if (items[i].Id == prefablightmap.GetInstanceID())
                    {
                        found = true;
                        break;
                    }

            if (found == false)
                items.Add(new PrefabLightmapDataItem()
                {
                    Id = prefablightmap.GetInstanceID(),
                    PrefabLightmap = prefablightmap,
                    Selected = false
                });
        }

        return items;
    }
    /// <summary>
    /// Get the default export directory
    /// </summary>
    /// <returns>Default export directory for the lightmap data</returns>
    protected string DefaultExportDirectory()
    {
        return SceneManager.GetActiveScene().path[0..^6] + "/PrefabLightmaps";
    }
    /// <summary>
    /// Check that the provided export directory path is relative to the project's asset folder
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>True if the provided path is relative to the project's asset folder</returns>
    protected bool ExportDirectoryIsRelative(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        int assetIndex = path.LastIndexOf("Assets");

        if (assetIndex == -1)
            return false;

        return true;
    }
    /// <summary>
    /// Check if any <see cref="PrefabLightmapDataItem"/> has been selected
    /// </summary>
    /// <returns>True if any <see cref="PrefabLightmapDataItem"/> has the selected flag</returns>
    protected bool HasSelectedItems()
    {
        foreach (PrefabLightmapDataItem item in this.ListViewBehaviours.itemsSource)
            if (item.Selected == true)
                return true;

        return false;
    }
    /// <summary>
    /// Check if all <see cref="PrefabLightmapDataItem"/>s have been selected
    /// </summary>
    /// <returns>True if all <see cref="PrefabLightmapDataItem"/>s are selected</returns>
    protected bool AllItemsSelected()
    {
        foreach (PrefabLightmapDataItem item in this.ListViewBehaviours.itemsSource)
            if (item.Selected != true)
                return false;

        return true;
    }

    /// <summary>
    /// Scan the <see cref="Renderer"/>s and the scene's light data to gather prefab lighting information
    /// </summary>
    /// <param name="gameObject">Prefab to scan</param>
    /// <param name="exportRoot">Root folder to export lightmap data</param>
    /// <param name="objectName">Name of the object including instance index</param>
    /// <param name="lightmapName">The lightmap slot name</param>
    /// <returns><see cref="PrefabLightmapInfo"/> gathered frot his prefab</returns>
    public static PrefabLightmapInfo GatherLightmapData(GameObject gameObject, string exportRoot, string objectName, string lightmapName)
    {
        PrefabLightmapInfo data = new PrefabLightmapInfo();

        List<Texture2D> lightmapList = new List<Texture2D>();
        List<Texture2D> directionLightmapList = new List<Texture2D>();
        List<Texture2D> shadowMaskList = new List<Texture2D>();

        List<PrefabLightmapLightInfo> lightsDataList = new List<PrefabLightmapLightInfo>();
        List<PrefabLightmapRendererInfo> renderDataList = new List<PrefabLightmapRendererInfo>();

        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.lightmapIndex != -1)
            {
                PrefabLightmapRendererInfo renderData = new PrefabLightmapRendererInfo
                {
                    Renderer = renderer
                };

                if (renderer.lightmapScaleOffset != Vector4.zero)
                {
                    //1ibrium's pointed out this issue : https://docs.unity3d.com/ScriptReference/Renderer-lightmapIndex.html
                    if (renderer.lightmapIndex < 0 || renderer.lightmapIndex == 0xFFFE) continue;

                    renderData.LightmapOffsetScale = renderer.lightmapScaleOffset;

                    Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
                    Texture2D lightmapDir = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
                    Texture2D shadowMask = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;

                    renderData.LightmapIndex = lightmapList.IndexOf(lightmap);

                    if (renderData.LightmapIndex == -1)
                    {
                        renderData.LightmapIndex = lightmapList.Count;

                        lightmapList.Add(lightmap);
                        directionLightmapList.Add(lightmapDir);
                        shadowMaskList.Add(shadowMask);
                    }

                    renderDataList.Add(renderData);
                }
            }
        }

        Light[] lights = gameObject.GetComponentsInChildren<Light>(true);

        foreach (Light light in lights)
        {
            lightsDataList.Add(new PrefabLightmapLightInfo
            {
                Light = light,
                LightmapBakeType = (int)light.lightmapBakeType,
                MixedLightingMode = (int)Lightmapping.lightingSettings.mixedBakeMode
            });
        }

        data.Lightmaps = lightmapList.ToArray();
        data.DirectionalLightmaps = directionLightmapList.ToArray();
        data.ShadowMasks = shadowMaskList.ToArray();
        data.LightData = lightsDataList.ToArray();
        data.RendererData = renderDataList.ToArray();

        for (int i = 0; i < data.Lightmaps.Length; i++)
        {
            Texture2D copied = PrefabLightmapTool.CopyLightmap(
                data.Lightmaps[i],
                exportRoot,
                objectName,
                lightmapName,
                TextureImporterType.Lightmap);

            data.Lightmaps[i] = copied;
        }

        for (int i = 0; i < data.DirectionalLightmaps.Length; i++)
        {
            Texture2D copied = PrefabLightmapTool.CopyLightmap(
                data.DirectionalLightmaps[i],
                exportRoot,
                objectName,
                lightmapName,
                TextureImporterType.DirectionalLightmap);

            data.DirectionalLightmaps[i] = copied;
        }

        for (int i = 0; i < data.ShadowMasks.Length; i++)
        {
            Texture2D copied = PrefabLightmapTool.CopyLightmap(
                data.ShadowMasks[i],
                exportRoot,
                objectName,
                lightmapName,
                TextureImporterType.Shadowmask);

            data.ShadowMasks[i] = copied;
        }

        return data;
    }
    /// <summary>
    /// Remove the slot information from the prefab and clean up the file system
    /// </summary>
    /// <param name="instance">Slot information of the prefab to clear</param>
    /// <param name="index">Index of the slot to clear</param>
    public static void ClearLightmapData(PrefabLightmapData instance, int index = -1)
    {
        if (index == -1 || index >= instance.PrefabLightmapInfoSlots.Length)
        {
            Debug.LogWarning("The provided index was out of range.", instance);

            return;
        }

        PrefabLightmapInfo prefabLightmapData = instance.PrefabLightmapInfoSlots[index].Data;

        foreach (Texture2D texture in prefabLightmapData.Lightmaps) PrefabLightmapTool.DeleteLightmap(texture);
        foreach (Texture2D texture in prefabLightmapData.DirectionalLightmaps) PrefabLightmapTool.DeleteLightmap(texture);
        foreach (Texture2D texture in prefabLightmapData.ShadowMasks) PrefabLightmapTool.DeleteLightmap(texture);

        PrefabLightmapInfoSlot[] newPrefablightmapData = new PrefabLightmapInfoSlot[instance.PrefabLightmapInfoSlots.Length - 1];

        int j = 0;

        for (int i = 0; i < instance.PrefabLightmapInfoSlots.Length; i++)
            if (i != index)
                newPrefablightmapData[j++] = instance.PrefabLightmapInfoSlots[i];

        instance.PrefabLightmapInfoSlots = newPrefablightmapData;
    }

    /// <summary>
    /// Copy lightmap files from the scene folder to the export folder
    /// </summary>
    /// <param name="texture">Lightmap texture to copy</param>
    /// <param name="exportRoot">Root folder to export lightmap data</param>
    /// <param name="objectName">Name of the object including instance index</param>
    /// <param name="lightmapName">The lightmap slot name</param>
    /// <param name="textureType">Type of the lightmap texture</param>
    /// <returns>The copied <see cref="Texture2D"/></returns>
    public static Texture2D CopyLightmap(Texture2D texture, string exportRoot, string objectName, string lightmapName, TextureImporterType textureType)
    {
        if (texture == null) return null;

        string prefabPath = Path.Combine(exportRoot, objectName, lightmapName);
        string assetPath = AssetDatabase.GetAssetPath(texture);
        string newAssetPath = Path.Combine(prefabPath, new FileInfo(assetPath).Name);

        Directory.CreateDirectory(prefabPath);

        AssetDatabase.CopyAsset(assetPath, newAssetPath);

        TextureImporter importer = AssetImporter.GetAtPath(newAssetPath) as TextureImporter;

        if (importer != null)
        {
            importer.textureType = textureType;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.anisoLevel = 3;
            importer.maxTextureSize = 4096;
            importer.isReadable = true;
        }

        AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<Texture2D>(newAssetPath);
    }
    /// <summary>
    /// Remove a texture from the file system
    /// </summary>
    /// <param name="texture"><see cref="Texture2D"/> to delete from the filesystem</param>
    public static void DeleteLightmap(Texture2D texture)
    {
        if (texture == null) return;

        string assetPath = AssetDatabase.GetAssetPath(texture);
        string parentPath = assetPath[..assetPath.LastIndexOf('/')];
        string grandparentPath = parentPath[..parentPath.LastIndexOf('/')];

        FileInfo info = new FileInfo(assetPath);

        AssetDatabase.DeleteAsset(assetPath);

        string[] files = Directory.GetFiles(info.Directory.FullName);

        if (files != null && files.Length < 1)
            AssetDatabase.DeleteAsset(parentPath);

        string[] parentFiles = Directory.GetFiles(info.Directory.Parent.FullName);

        if (parentFiles != null && parentFiles.Length < 1)
            AssetDatabase.DeleteAsset(grandparentPath);
    }
}
