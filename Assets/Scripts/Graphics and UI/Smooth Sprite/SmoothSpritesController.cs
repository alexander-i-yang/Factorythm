using System;
using System.Collections.Generic;
using UnityEngine;

public class SmoothSpritesContoller : MonoBehaviour {
    public SmoothSpriteLoop[] smoothSprites;

    void Start() {
        smoothSprites = GetComponentsInChildren<SmoothSpriteLoop>();
    }

    void Move() {
        foreach (var s in smoothSprites) {
            s.Move();
        }
    }
}