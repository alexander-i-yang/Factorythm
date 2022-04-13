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
    
    void Start() {
        BeatBar = FindObjectOfType<BeatBar>();
        Gauge = FindObjectOfType<Gauge>();
    }
    
    public void Tick() {
        BeatBar.Tick();
    }

    public static void Quit() {
        Application.Quit();
    }
}