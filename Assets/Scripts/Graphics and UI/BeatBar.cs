using System.Collections.Generic;
using UnityEngine;

public class BeatBar : MonoBehaviour {
    public GameObject BeatLine;

    private List<BeatLine> _beatLines;
    
    void Start() {
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
        Vector3 initPos = BeatLine.transform.position;
        return Instantiate(
            BeatLine, 
            new Vector3(1, 0, initPos.z), 
            Quaternion.identity,
            transform).GetComponent<BeatLine>();
    }

    void Update() {
        
        // print(Conductor.Instance.currentClip.BPM);
    }
}