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
    //Not rigorously tested 
    private double _endZoneWidth; //In units
    private double _beatLineWidth; //In units
    
    [Tooltip("The amount of time a player can press before the beat line touches the end zone. Multiplied by seconds/beat")]
    [SerializeField] private float _graceTime = 0.1f;

    //The amount of time it takes for a beat line to dissapate after stopping. Multiplied by seconds/beat
    public float DissolveTime { get; } = 0.1f;

    void Start() {
        StartPos = transform.Find("StartPos").localPosition;
        StartPos = new Vector3(StartPos.x, StartPos.y, BeatLineInstance.transform.position.z);
        EndPos = transform.Find("End zone").localPosition;

        _endZoneWidth = transform.Find("End zone").GetComponent<SpriteRenderer>().bounds.size.x;
        _beatLineWidth = BeatLineInstance.GetComponent<SpriteRenderer>().bounds.size.x;
        
        _beatLines = new List<BeatLine>();

        //Find an inactive beatline
        // BeatLine initLine = _beatLines.Find(e => !e.gameObject.activeSelf);
    }

    BeatLine InitBeatClipAtStart() {
        GameObject g = Instantiate(BeatLineInstance, transform);
        g.transform.localPosition = StartPos;
        BeatLine beatLine = g.GetComponent<BeatLine>();
        beatLine.beatBar = this;
        _beatLines.Add(beatLine);
        return beatLine;
    }

    public double GetVelocity() {
        double secPerBeat = Conductor.Instance.BeatClipHelper.BeatClip.SecPerBeat;
        double validMult = Conductor.Instance.BeatClipHelper.ValidTime;

        // Represents the total time between the moment a beatline crosses the valid threshold and the moment it reaches
        // The center of the endzone
        double timeElapsed = secPerBeat*validMult - DissolveTime + _graceTime*secPerBeat;

        double v = (_endZoneWidth + _beatLineWidth) / (2 * timeElapsed);
        return v;
    }

    public void Tick() {
        double dTravel = Math.Abs(EndPos.x - StartPos.x);
        InitBeatClipAtStart();
    }
}