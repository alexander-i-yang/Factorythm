using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour {
    private static Conductor _instance;
    public float songBpm = 24;
    public float secPerBeat = 24.0f/60;
    [NonSerialized] public float songPosition = 0;
    [NonSerialized] public float songPositionInBeats = 0;
    [NonSerialized] public float dspSongTime = 0;
    [NonSerialized] public AudioSource MusicSource;
    public AudioClip MusicClip;
    
    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = MusicClip;
    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}