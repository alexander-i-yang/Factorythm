using System;
using System.Collections;
using UnityEngine;
using FMOD;
using FMODUnity;
using Debug = UnityEngine.Debug;
using Studio = FMOD.Studio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MachineSfx : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public EventReference exposed_src;

    [HideInInspector]
    public bool? loopByDefault = null;

    public Studio.EventInstance instance { get; private set; }

    public StartCondition startCondition;

    public IEnumerator InstantiateSong()
    {
        while (!RuntimeManager.IsInitialized || RuntimeManager.AnySampleDataLoading()) {
            // Debug.Log("Uhhh");
            yield return null;
        }

        instance = RuntimeManager.CreateInstance(exposed_src);
        
        if (instance.isValid()) {
            if (loopByDefault != null) {
                instance.setParameterByName("loop", loopByDefault.Value ? 1f : 0f);
            }
            // if the loop parameter is not in the timeline, loop is left as null by default

            // decide whether to start the song or not
            instance.set3DAttributes(transform.To3DAttributes());
            if (startCondition == StartCondition.ON_ENABLE)
            {
                if (instance.getParameterByName("loop", out float _) == RESULT.OK) {
                    print(this);
                    instance.start();
                    instance.release();
                }
                else {
                    instance.start();
                }
            }
        }
    }

    public void Pause()
    {
        if (instance.isValid())
        {
            instance.setPaused(true);
        }
    }

    public void UnPause() {
        if(instance.isValid())
        {
            instance.start();
            instance.setPaused(false);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(InstantiateSong());
    }

    private void OnDisable()
    {
        StopInstance(Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        StopInstance(Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopInstance(Studio.STOP_MODE mode)
    {
        if (instance.isValid()) instance.stop(mode);
    }

    public enum StartCondition
    {
        ON_ENABLE,
        ON_CLICK,
        ON_DESELECT,
        ON_DELETE,
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MachineSfx))]
public class MachineSfxEditor : Editor
{
    SerializedProperty song;

    void OnEnable(){
        var m = serializedObject.targetObject as MachineSfx;
        UpdateLoopable(m);
        song = serializedObject.FindProperty("exposed_src");
    }
    
    public override void OnInspectorGUI()
    {
    
        var m = (serializedObject.targetObject as MachineSfx);

        if (m.instance.isValid()){
            if (
                m.instance.getPlaybackState(out Studio.PLAYBACK_STATE state) == RESULT.OK
                && state == Studio.PLAYBACK_STATE.PLAYING
                ){
                EditorGUILayout.LabelField("Instance Playing");
            } else {
                EditorGUILayout.LabelField("Instance Not Playing");
            }
        } else {
            EditorGUILayout.LabelField("No Instance");
        }

        EditorGUILayout.PropertyField(song, new GUIContent("Event Reference"));
        var newR = song.GetEventReference();
        if (!newR.Equals(m.exposed_src)) {
            // Debug.Log($"changed to {newR}");
            m.exposed_src = newR;
            UpdateLoopable(m);
            // EditorCoroutineUtility.StartCoroutine(Preview(3, newR), this);
            if (m.instance.isValid()) {

                if (m.instance.getPlaybackState(out Studio.PLAYBACK_STATE s) == RESULT.OK
                    && s == Studio.PLAYBACK_STATE.PLAYING) {

                    m.instance.stop(Studio.STOP_MODE.IMMEDIATE);
                }
                m.instance.release();
                if (m.startCondition == MachineSfx.StartCondition.ON_ENABLE && Application.isPlaying) {
                    m.StartCoroutine(m.InstantiateSong());
                }
            }
        }

        if (m.loopByDefault.HasValue) {
            var e = GUI.enabled;
            GUI.enabled = !Application.isPlaying;
            m.loopByDefault = EditorGUILayout.Toggle("Loop By Default", m.loopByDefault.Value);
            GUI.enabled = e; // restore old enable state
        } else {
            EditorGUILayout.LabelField("Not loopable");
        }

        base.OnInspectorGUI();
    }

    void UpdateLoopable(MachineSfx m) {
        Action<EventReference> e = src => {
            Studio.EventInstance instance = RuntimeManager.CreateInstance(src);
            if (instance.isValid()){
                var loopable = instance.getParameterByName("loop", out float val) == RESULT.OK;
                if (loopable)
                    m.loopByDefault = true;
                else
                    m.loopByDefault = null;
                instance.release();
            } else {
                m.loopByDefault = null;
            }
        };
        e(m.exposed_src);
    }

    /// apparently FMOD doesn't play in editor mode...

    //IEnumerator Preview(float sec, EventReference er) {
    //    float t = 0f;
    //    var inst = RuntimeManager.CreateInstance(er);
    //    Debug.Log("enter preview");
    //    if (inst.isValid()) {
    //        Debug.Log("started");
    //        inst.start();
    //        while (t < sec
    //            && inst.getPlaybackState(out Studio.PLAYBACK_STATE s) == RESULT.OK
    //            && s == Studio.PLAYBACK_STATE.PLAYING
    //            ) {
    //            t += Time.deltaTime;
    //            yield return null;
    //        }
    //        inst.stop(Studio.STOP_MODE.ALLOWFADEOUT);
    //        inst.release();
    //    }
    //}
}
#endif
