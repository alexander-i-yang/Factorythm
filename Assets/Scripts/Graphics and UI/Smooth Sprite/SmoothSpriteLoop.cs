using System;
using UnityEngine;

public enum positionState {
    START,
    END,
}

public class SmoothSpriteLoop : SmoothSprite {
    public Vector3 endPos;
    public Vector3 startPos;

    [NonSerialized] public positionState pos = positionState.START;

    void Start() {
        startPos = transform.position;
    }

    public void Move() {
        if (pos == positionState.START) {
            base.Move(endPos);
            pos = positionState.END;
        } else if (pos == positionState.END) {
            base.Move(startPos);
            pos = positionState.START;
        }
    }
}