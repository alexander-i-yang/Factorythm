using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls all traditional UI for the game.
/// </summary>
public class UIManager : MonoBehaviour {
    [NonSerialized] public TextMeshProUGUI CurLabel;
    [NonSerialized] public BeatBar BeatBar;
    [NonSerialized] public TextMeshProUGUI MaxLabel;
    [SerializeField] private bool _visible;
    public bool Visible
    {
        set
        {
            _visible = value;
            CurLabel.gameObject.SetActive(_visible);
            MaxLabel.gameObject.SetActive(_visible);
        }
    }
    void Start() {
        CurLabel = transform.Find("Cur Combo").GetComponent<TextMeshProUGUI>();
        BeatBar = FindObjectOfType<BeatBar>();
        MaxLabel = transform.Find("Max Combo").GetComponent<TextMeshProUGUI>();
    }

    public void Tick() {
        BeatBar.Tick();
    }
}