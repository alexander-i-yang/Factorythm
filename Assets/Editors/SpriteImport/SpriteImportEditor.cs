using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteImportEditor : EditorWindow
{
    public Texture2D sprite;
    public int setting_index = 0;
    public string path;

    private void OnGUI()
    {
        
        GUI.DrawTexture(new Rect(75, 25, 150, 150), sprite, ScaleMode.ScaleToFit, true);

        EditorGUILayout.Space(200);
        setting_index = EditorGUILayout.Popup(setting_index, SpriteImportActionAsset.instance.Settings.ConvertAll(sisa => sisa.name).ToArray());

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Import"))
        {
            SpriteImportSettingsAsset sisa = SpriteImportActionAsset.instance.Settings[setting_index];

            string preferred_save = EditorPrefs.GetString("SprImp_Pref_Save");

            string save_path;
            if (preferred_save.Length != 0)
                save_path = EditorUtility.SaveFilePanelInProject("Save", Path.GetFileNameWithoutExtension(path), "png", "", preferred_save);
            else
            {
                save_path = EditorUtility.SaveFilePanelInProject("Save", Path.GetFileNameWithoutExtension(path), "png", "");
            }

            if (save_path.Length != 0)
            {
                EditorPrefs.SetString("SprImp_Pref_Save", Path.GetDirectoryName(save_path));
                Directory.CreateDirectory(Path.GetDirectoryName(save_path));
                File.WriteAllBytes(
                    save_path,
                    sprite.EncodeToPNG());

                AssetDatabase.Refresh();

                var ti = AssetImporter.GetAtPath(save_path) as TextureImporter;
                ti.spritePixelsPerUnit = sisa.pixel_per_unit;
                ti.filterMode = sisa.filter_mode;
                ti.wrapMode = sisa.wrap_mode;
                ti.textureType = sisa.texture_importer_type;
                ti.mipmapEnabled = false;
                EditorUtility.SetDirty(ti);
                ti.SaveAndReimport();
                this.Close();
            }
            else
            {

            }
        }
    }

    [MenuItem("SprImp/Initialize")]
    static void InstantiateSpriteImportActionSingleton()
    {
        var path = EditorUtility.SaveFilePanelInProject("Save your SprImp Asset", "sprimp.asset", "asset", "");

        if (path.Length == 0) return;

        AssetDatabase.CreateAsset(SpriteImportActionAsset.instance, path);

    }

    [MenuItem("SprImp/Add Sprite Category")]
    static void AddSpriteCategory()
    {
        var path = EditorUtility.SaveFilePanelInProject("Save your SprImp Asset", "sprite category.asset", "asset", "");
        if (path.Length != 0)
        {
            var sisa = CreateInstance<SpriteImportSettingsAsset>();
            AssetDatabase.CreateAsset(sisa, path);
            SpriteImportActionAsset.instance.Settings.Add(sisa);
            AssetDatabase.OpenAsset(sisa);
        }
    }

    [MenuItem("SprImp/Import Sprite")]
    static void ImportSprite()
    {
        if (SpriteImportActionAsset.instance == null)
        {
            Debug.LogError("SprImp is not initialized yet");
            InstantiateSpriteImportActionSingleton();
            return;
        }
        else
        {
            // prune unused
            Debug.Log("SprImp found the following availble categories:");
            for (int i = 0; i < SpriteImportActionAsset.instance.Settings.Count; i++)
            {
                SpriteImportSettingsAsset sisa = SpriteImportActionAsset.instance.Settings[i];
                try
                {
                    Debug.Log("... " + sisa.name);
                }
                catch
                {
                    SpriteImportActionAsset.instance.Settings.Remove(sisa);
                    i--;
                }
            }

            if (SpriteImportActionAsset.instance.Settings.Count == 0)
            {
                Debug.LogError("You do not have a sprite format yet!");
                AddSpriteCategory();
                return;
            }
            else
            {
                // extensions from https://docs.unity3d.com/Manual/ImportingTextures.html

                string read_pref = EditorPrefs.GetString("SprImp_Pref_Read");
                string path;
                if (read_pref.Length != 0)
                {
                    path = EditorUtility.OpenFilePanel("Select Sprite", read_pref, "bmp,exr,gif,hdr,iff,jpg,jpeg,pict,png,psd,tga,tiff");
                }
                else
                {
                    path = EditorUtility.OpenFilePanel("Select Sprite", "", "bmp,exr,gif,hdr,iff,jpg,jpeg,pict,png,psd,tga,tiff");
                }
                
                if (path.Length != 0)
                {
                    EditorPrefs.SetString("SprImp_Pref_Read", Path.GetDirectoryName(path));

                    var w = GetWindow<SpriteImportEditor>();
                    w.sprite = new Texture2D(1, 1);
                    w.path = path;
                    ImageConversion.LoadImage(w.sprite, File.ReadAllBytes(path));
                    float d = Mathf.Min(300, Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height));
                    w.position = new Rect(Screen.currentResolution.width / 2 - d/2, Screen.currentResolution.height / 2 - d/2, d, d);
                    return;
                }
                else
                {
                    Debug.LogError("Invalid selection");
                }
            }
        }
    }
}
