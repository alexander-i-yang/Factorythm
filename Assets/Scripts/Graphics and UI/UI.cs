using System;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour {
    [NonSerialized] public TextMeshProUGUI Label;

    void Start() {
        Label = GetComponentInChildren<TextMeshProUGUI>();
    }
}