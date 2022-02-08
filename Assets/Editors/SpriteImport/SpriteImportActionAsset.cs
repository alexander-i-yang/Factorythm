using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SpriteImportActionAsset : ScriptableSingleton<SpriteImportActionAsset>
{
    [SerializeField]
    public List<SpriteImportSettingsAsset> Settings = new List<SpriteImportSettingsAsset>();

}

[CustomEditor(typeof(SpriteImportActionAsset))]
public class SpriteImportActionAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Fetch All Settings"))
        {
            string[] guids = AssetDatabase.FindAssets("t:SpriteImportSettingsAsset");
            List<string> paths = new List<string>();
            foreach (string g in guids) paths.Add(AssetDatabase.GUIDToAssetPath(g));
            foreach (string p in paths)
            {
                SpriteImportSettingsAsset sisa = AssetDatabase.LoadAssetAtPath<SpriteImportSettingsAsset>(p);
                if (sisa != null && !SpriteImportActionAsset.instance.Settings.Contains(sisa))
                {
                    SpriteImportActionAsset.instance.Settings.Add(sisa);
                }
            }
        }
    }
}