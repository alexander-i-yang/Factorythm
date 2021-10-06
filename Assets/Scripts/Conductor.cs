using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Conductor : MonoBehaviour {
    private static Conductor _instance;
    public float secPerBeat = 24.0f/60;
    [NonSerialized] public float SongPosition = 0;
    [NonSerialized] public float SongPositionInBeats = 0;
    [NonSerialized] public int LastSongPosWholeBeats = 0;
    [NonSerialized] public float DSPSongTime = 0;
    [NonSerialized] public AudioSource MusicSource;
    [FormerlySerializedAs("Music Clip")] public AudioClip musicClip;
    
    [NonSerialized] private Machine[] _allMachines;
    
    [NonSerialized] public float timeSinceBeat;
    [NonSerialized] public float timeTilNextBeat;
    
    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = musicClip;
    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.Play();
        DSPSongTime = (float)AudioSettings.dspTime;
        _allMachines = FindObjectsOfType<Machine>();
    }

    // Update is called once per frame
    void Update() {
        UpdateSongPos();
    }

    void UpdateSongPos() {
        // songPosition = (float)(AudioSettings.dspTime - (timeCountDownStarted + (secPerBeat * (numCountDownBeats + 1))) - songData.startOffset - GameManager.Instance.cumulativePauseTime);
        SongPosition = (float) (AudioSettings.dspTime-DSPSongTime);
        SongPositionInBeats = SongPosition / secPerBeat;
        if (SongPositionInBeats - LastSongPosWholeBeats > 1) {
            LastSongPosWholeBeats = (int) Math.Floor(SongPositionInBeats);
            Tick();
        }
    }

    public bool IsInputOnBeat() {
        timeSinceBeat = (SongPositionInBeats - LastSongPosWholeBeats)*secPerBeat;
        timeTilNextBeat = secPerBeat - timeSinceBeat;
        return timeTilNextBeat < 0.1 || timeSinceBeat < 0.2;
    }

    private void OnDrawGizmos() {
        Handles.Label(new Vector3(3, 3, -0.5f), (timeSinceBeat < 0.15)+"");
        Handles.Label(new Vector3(3.5f, 3, -0.5f), (timeSinceBeat)+"");
        Gizmos.DrawWireCube(new Vector3(3, 3, -0.5f), new Vector3(timeSinceBeat/secPerBeat, 1, -0.5f));
        
        Handles.Label(new Vector3(3, 4, -0.5f), (timeTilNextBeat <0.3)+"");
        Handles.Label(new Vector3(3.5f, 4, -0.5f), (timeTilNextBeat)+"");
        Gizmos.DrawWireCube(new Vector3(3, 4, -0.5f), new Vector3(timeTilNextBeat/secPerBeat, 1, -0.5f));
    }

    public void Tick() {
        foreach (Machine machine in _allMachines) {
            if (machine.OutputMachines.Count == 0) {
                print("Root tick is " + machine.name);
                machine.Tick();
            }
        }
        Debug.Break();
    }
}