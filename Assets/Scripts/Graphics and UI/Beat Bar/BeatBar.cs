using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// Not rigorously tested. Proceed with caution.
public class BeatBar : MonoBehaviour {
    public GameObject BeatLineInstance;

    private List<BeatLine> _beatLines;
    public Vector3 StartPos { get; private set; }
    public Vector3 EndPos { get; private set; }

    [Header("Math for beatline movement calculations")]
    
    [Tooltip("The amount of time a player can press before the beat line touches the end zone. Multiplied by seconds/beat")]
    [SerializeField] private float _graceTime = 0.1f;

    //The amount of time it takes for a beat line to dissipate after stopping. Multiplied by seconds/beat
    public float DissolveTime { get; } = 0.1f;

    void Start() {
        StartPos = transform.Find("StartPos").localPosition;
        EndPos = transform.Find("End zone").localPosition;

        _beatLines = new List<BeatLine>();

        //Find an inactive beatline
        // BeatLine initLine = _beatLines.Find(e => !e.gameObject.activeSelf);
    }

    void InitBeatClipAtStart() {
        GameObject g = Instantiate(BeatLineInstance, transform);
        g.transform.localPosition = StartPos;
        BeatLine beatLine = g.GetComponent<BeatLine>();
        beatLine.beatBar = this;
        _beatLines.Add(beatLine);
    }

    public double GetVelocity() {
        double secPerBeat = Conductor.Instance.BeatClipHelper.BeatClip.SecPerBeat;

        // Represents the total time between the moment a beatline starts to exist and the moment it reaches
        // The center of the endzone
        double timeElapsed = 3 * secPerBeat + Conductor.Instance.BeatClipHelper.BeatClip.BeatOffset;

        double v = (StartPos.x-EndPos.x)/(timeElapsed);
        v /= 2;
        return v;
    }

    public void Tick() {
        InitBeatClipAtStart();
    }
}