using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BeatBar : MonoBehaviour {
    public GameObject BeatLine;

    private List<BeatLine> _beatLines;
    
    void Start() {
        _beatLines = new List<BeatLine>();
        for (int i = 0; i < 10; ++i) {
            BeatLine b = Instantiate(BeatLine).GetComponent<BeatLine>();
            b.gameObject.SetActive(false);
            _beatLines.Add(b);
        }
    }

    void Update() {
        // print(Conductor.Instance.currentClip.BPM);
    }
}