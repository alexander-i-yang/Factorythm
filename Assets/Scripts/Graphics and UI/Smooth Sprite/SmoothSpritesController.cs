using System;
using System.Collections.Generic;
using UnityEngine;

public class SmoothSpritesController : MonoBehaviour {
    private SmoothSpriteLoop[] smoothSprites;

    void Start() {
        smoothSprites = GetComponentsInChildren<SmoothSpriteLoop>();
    }

    public void Move() {
        foreach (var s in smoothSprites) {
            //print("s");
            s.Move();
        }
    }
}