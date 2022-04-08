using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls all traditional UI for the game.
/// </summary>
public class UIManager : MonoBehaviour {
    // public TextMeshProUGUI CurLabel;
    public BeatBar BeatBar;
    // public TextMeshProUGUI MaxLabel;
    public Gauge Gauge;
    [SerializeField] private bool _visible;
    public bool Visible
    {
        set
        {
            _visible = value;
        }
    }
    
    void Start() {
        // CurLabel = transform.Find("Cur Combo").GetComponent<TextMeshProUGUI>();
        BeatBar = FindObjectOfType<BeatBar>();
        // MaxLabel = transform.Find("Max Combo").GetComponent<TextMeshProUGUI>();
        Gauge = FindObjectOfType<Gauge>();
    }

    public void Tick() {
        BeatBar.Tick();
    }
}