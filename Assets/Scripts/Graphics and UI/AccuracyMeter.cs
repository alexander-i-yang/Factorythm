using UnityEngine;
using UnityEditor;

public class AccuracyMeter : MonoBehaviour {
    private Vector3 _startPos;

    void Start() {
        _startPos = transform.position;
    }

    void Update() {
        // Handles.Label(_startPos, "hello");
    }
}