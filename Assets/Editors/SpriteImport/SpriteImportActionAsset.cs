using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpriteImportActionAsset : ScriptableSingleton<SpriteImportActionAsset>
{
    [SerializeField]
    public List<SpriteImportSettingsAsset> Settings = new List<SpriteImportSettingsAsset>();

}