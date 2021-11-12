using System;
using UnityEngine;

public abstract class Room : MonoBehaviour {
    private BoxCollider2D _myCollider;

    void Start() {
        _myCollider = GetComponent<BoxCollider2D>();
    }

    public abstract void OnPlayerEnter(PlayerController pc);
    public abstract void OnPlayerExit(PlayerController pc);
}