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

    [Header("Beat Line Movement")]
    
    [Tooltip("The amount of time, in beats, that a beat line will exist on the beat bar.")]
    [SerializeField] private int beatLineDuration = 8;

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

        // The time we want between the beat line's appearance and it reaching it the end zone
        double timeElapsed = (beatLineDuration - Conductor.Instance.BeatClipHelper.BeatClip.BeatOffset) * secPerBeat;

        // Do not change this formula if you want beat lines to be on time.
        double v = (StartPos.x-EndPos.x)/(timeElapsed);
        return v;
    }

    public void Tick() {
        InitBeatClipAtStart();
    }
}