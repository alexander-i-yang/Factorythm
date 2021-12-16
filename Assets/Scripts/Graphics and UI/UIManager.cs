using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [NonSerialized] public TextMeshProUGUI Label;
    [NonSerialized] public BeatBar BeatBar;

    void Start() {
        Label = GetComponentInChildren<TextMeshProUGUI>();
        BeatBar = FindObjectOfType<BeatBar>();
    }

    public void Tick() {
        BeatBar.Tick();
    }
}