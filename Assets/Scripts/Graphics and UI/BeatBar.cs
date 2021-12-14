using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BeatBar : MonoBehaviour {
    public GameObject BeatLineInstance;

    private List<BeatLine> _beatLines;
    public Vector3 StartPos { get; private set; }
    public Vector3 EndPos { get; private set; }
    
    private float _endZoneWidth; //In units

    [Tooltip("The amount of time a player can press before the beat line touches the end zone. Multiplied by seconds/beat")]
    [SerializeField] private float _graceTime = 0.1f;
    
    [Tooltip("The amount of time it takes for a beat line to dissapate after stopping. Multiplied by seconds/beat")]
    [SerializeField] private float _dissolveTime = 0f;

    void Start() {
        StartPos = transform.Find("StartPos").localPosition;
        StartPos = new Vector3(StartPos.x, StartPos.y, BeatLineInstance.transform.position.z);
        EndPos = transform.Find("End zone").localPosition;

        _endZoneWidth = transform.Find("End zone").GetComponent<SpriteRenderer>().bounds.size.x;
        
        _beatLines = new List<BeatLine>();
        InitBeatClipAtStart();

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

    void Update() {
        // _beatLines[0].transform.position.x
        // print(Conductor.Instance.currentClip.BPM);
    }
}