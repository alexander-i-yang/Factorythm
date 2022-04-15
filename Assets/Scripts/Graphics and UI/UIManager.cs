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
    public PauseMenu PauseMenu;

    public SmoothRotate[] Gears;
  
    [SerializeField] private bool _visible;
    public bool Visible
    {
        set
        {
            _visible = value;
        }
    }
    
    void Start() {
        BeatBar = FindObjectOfType<BeatBar>();
        Gauge = FindObjectOfType<Gauge>();
        PauseMenu = FindObjectOfType<PauseMenu>();

        Gears = GameObject.Find("Gears").GetComponentsInChildren<SmoothRotate>();
    }

    public void Tick() {
        BeatBar.Tick();
        foreach (var smoothRotate in Gears) {
            smoothRotate.Rotate();
        }
    }

    public static void Quit() {
        Application.Quit();
    }
}