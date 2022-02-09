using System;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "BeatClip", menuName = "ScriptableObjects/BeatClip")]
public class BeatClipSO : ScriptableObject {
    public int BPM;
    [NonSerialized] public float SecPerBeat;
    public float BeatOffset;
    public AudioClip MusicClip;

    public EventReference fmodSongReference;
}
