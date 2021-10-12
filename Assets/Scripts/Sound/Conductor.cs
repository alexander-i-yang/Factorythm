using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class Conductor : MonoBehaviour {
    private static Conductor _instance;
    public BeatClip currentClip;
    
    [NonSerialized] private Machine[] _allMachines;
    
    [NonSerialized] public AudioSource MusicSource;
    
    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        currentClip.Init();
        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = currentClip.MusicClip;
    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.Play();
        _allMachines = FindObjectsOfType<Machine>();
    }

    // Update is called once per frame
    void Update() {
        UpdateSongPos();
    }

    void UpdateSongPos() {
        bool isNewBeat = currentClip.UpdateSongPos();
        if (isNewBeat) {
            Tick();
        }
    }

    public bool IsInputOnBeat() {
        return (currentClip.TimeSinceBeat() < 0.1 || currentClip.TimeTilNextBeat() < 0.1);
    }

    private void OnDrawGizmos() {
        Handles.Label(new Vector3(3, 3, -0.5f), (currentClip.TimeSinceBeat() < 0.15)+"");
        Handles.Label(new Vector3(3.5f, 3, -0.5f), (currentClip.TimeSinceBeat())+"");
        Gizmos.DrawWireCube(new Vector3(3, 3, -0.5f), new Vector3(currentClip.TimeSinceBeat()/currentClip.SecPerBeat, 1, -0.5f));
        
        Handles.Label(new Vector3(3, 4, -0.5f), (currentClip.TimeTilNextBeat() <0.3)+"");
        Handles.Label(new Vector3(3.5f, 4, -0.5f), (currentClip.TimeTilNextBeat())+"");
        Gizmos.DrawWireCube(new Vector3(3, 4, -0.5f), new Vector3(currentClip.TimeTilNextBeat()/currentClip.SecPerBeat, 1, -0.5f));
    }

    public void Tick() {
        foreach (Machine machine in _allMachines) {
            if (machine.OutputMachines.Count == 0) {
                machine.Tick();
            }
        }
    }
}