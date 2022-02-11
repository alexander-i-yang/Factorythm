#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteImportSettingsAsset : ScriptableObject
{
    public int pixel_per_unit;
    public TextureWrapMode wrap_mode;
    public FilterMode filter_mode;
    public TextureImporterType texture_importer_type;
}
#endif