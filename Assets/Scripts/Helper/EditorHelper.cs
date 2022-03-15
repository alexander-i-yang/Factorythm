#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorHelper : MonoBehaviour {
    [MenuItem("EditorHelper/SliceSprites")]
    static void SliceSprites() {
        // Change the below for the with and height dimensions of each sprite within the spritesheets
        int sliceWidth = 32;
        int sliceHeight = 32;

        // Change the below for the path to the folder containing the sprite sheets (warning: not tested on folders containing anything other than just spritesheets!)
        // Ensure the folder is within 'Assets/Resources/' (the below example folder's full path within the project is 'Assets/Resources/ToSlice')
        string folderPath = "Conveyors";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        Debug.Log("spriteSheets.Length: " + spriteSheets.Length);

        for (int z = 0; z < spriteSheets.Length; z++) {
            Debug.Log("z: " + z + " spriteSheets[z]: " + spriteSheets[z]);

            string path = AssetDatabase.GetAssetPath(spriteSheets[z]);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            Texture2D spriteSheet = spriteSheets[z] as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += sliceWidth) {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight) {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (spriteSheet.height - j) / sliceHeight + ", " + i / sliceWidth;
                    smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);

                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        Debug.Log("Done Slicing!");
    }
}

public class AutoAnimationCreator : EditorWindow {
    public int frameHeight;
    public int frameWidth;
    public Texture2D pasteTo; //Sprite atlas where to paste settings
    public Texture2D[] SpriteSheets;
    [MenuItem("Window/Sprite Animator")]
    static void Init() {
        // Window Set-Up
        AutoAnimationCreator window =
            EditorWindow.GetWindow(typeof(AutoAnimationCreator), false, "AnimationGenerator", true) as
                AutoAnimationCreator;
        window.minSize = new Vector2(260, 170);
        window.maxSize = new Vector2(260, 170);
        window.Show();
    }

    //Show UI
    void OnGUI() {
        // "target" can be any class derived from ScriptableObject 
        // (could be EditorWindow, MonoBehaviour, etc)
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("SpriteSheets");

        EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties
        // _spriteSheets = (Texture2D[]) EditorGUILayout.ObjectField("SpriteSheets", _spriteSheets, typeof(Texture2D[]), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Animation")) {
            foreach (var spriteSheet in SpriteSheets) {
                if (spriteSheet != null) {
                    Sprite[] sprites = cutSprites(spriteSheet);
                    makeAnimation(spriteSheet.name, sprites);
                }
            }
        }
        else {
            Debug.LogWarning("Forgot to set the textures?");
        }

        Repaint();
    }

    private Sprite[] cutSprites(Texture2D spriteSheet) {
        if (!IsAtlas(spriteSheet)) {
            Debug.LogWarning("Unable to proceed, the source texture is not a sprite atlas.");
            return null;
        }

        Sprite[] sprites = null;
        
        //Proceed to read all sprites from CopyFrom texture and reassign to a TextureImporter for the end result
        UnityEngine.Object[] _objects =
            AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(spriteSheet));

        if (_objects != null && _objects.Length > 0)
            sprites = new Sprite[_objects.Length];

        for (int i = 0; i < _objects.Length; i++) {
            sprites[i] = _objects[i] as Sprite;
        }

        return sprites;
    }

    private void makeAnimation(String name, Sprite[] sprites) {
        //http://forum.unity3d.com/threads/lack-of-scripting-functionality-for-creating-2d-animation-clips-by-code.212615/
        AnimationClip animClip = new AnimationClip();
        // First you need to create e Editor Curve Binding
        EditorCurveBinding curveBinding = new EditorCurveBinding();

        // I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
        curveBinding.type = typeof(SpriteRenderer);
        // Regular path to the gameobject that will be changed (empty string means root)
        curveBinding.path = "";
        // This is the property name to change the sprite of a sprite renderer
        curveBinding.propertyName = "m_Sprite";

        // An array to hold the object keyframes
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[10];
        for (int i = 0; i < 10; i++) {
            keyFrames[i] = new ObjectReferenceKeyframe();
            // set the time
            keyFrames[i].time = i;
            // set reference for the sprite you want
            keyFrames[i].value = sprites[i];
            Debug.LogWarning(sprites[i]);
        }

        AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames);
        AssetDatabase.CreateAsset(animClip, "Assets/Animation/Conveyors/" + name + ".anim");
    }

    //Check that the texture is an actual atlas and not a normal texture
    private bool IsAtlas(Texture2D tex) {
        string _path = AssetDatabase.GetAssetPath(tex);
        TextureImporter _importer = AssetImporter.GetAtPath(_path) as TextureImporter;

        return _importer.textureType == TextureImporterType.Sprite &&
               _importer.spriteImportMode == SpriteImportMode.Multiple;
    }
}


#endif