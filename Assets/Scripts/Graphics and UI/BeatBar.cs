using System.Collections.Generic;
using UnityEngine;

public class BeatBar : MonoBehaviour {
    public GameObject BeatLine;

    private List<BeatLine> _beatLines;
    private Vector3 _startPos;
    private Vector3 _endPos;

    private float _travelLength;


    void Start() {
        _startPos = transform.Find("StartPos").localPosition;
        _endPos = transform.Find("End zone").localPosition;
        _startPos.z = BeatLine.transform.position.z;

        _travelLength = _startPos.x - _endPos.x;
        
        _beatLines = new List<BeatLine>();
        for (int i = 0; i < 10; ++i) {
            BeatLine b = InitBeatClipAtEnd();
            b.gameObject.SetActive(false);
            _beatLines.Add(b);
        }
        
        //Find an inactive beatline
        // BeatLine initLine = _beatLines.Find(e => !e.gameObject.activeSelf);
    }

    BeatLine InitBeatClipAtEnd() {
        GameObject beatLine = Instantiate(BeatLine, transform);
        beatLine.transform.localPosition = _startPos;
        return beatLine.GetComponent<BeatLine>();
    }

    void Update() {
        
        // print(Conductor.Instance.currentClip.BPM);
    }
}