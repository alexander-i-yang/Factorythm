using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Conductor : MonoBehaviour {
    private static Conductor _instance;
    public float songBpm = 24;
    public float secPerBeat = 24.0f/60;
    [NonSerialized] public float SongPosition = 0;
    [NonSerialized] public float SongPositionInBeats = 0;
    [NonSerialized] public int LastSongPosWholeBeats = 0;
    [NonSerialized] public float DSPSongTime = 0;
    [NonSerialized] public AudioSource MusicSource;
    [FormerlySerializedAs("Music Clip")] public AudioClip musicClip;
    public bool justTicked = false;
    
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
            justTicked = true;
            // print(SongPositionInBeats);
        }
        else {
            justTicked = false;
        }
    }

    public bool IsInputOnBeat() {
        float timeSinceBeat = (SongPositionInBeats - LastSongPosWholeBeats)*secPerBeat;
        float timeTilNextBeat = secPerBeat - timeSinceBeat;
        print(timeSinceBeat + " " + timeTilNextBeat);
        return timeSinceBeat < 0.1 || timeTilNextBeat < 0.2;
    }
}