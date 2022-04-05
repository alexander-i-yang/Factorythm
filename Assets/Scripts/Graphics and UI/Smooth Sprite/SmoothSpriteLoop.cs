using System;
using UnityEngine;

public enum positionState {
    START,
    END,
}

/// <summary>
/// <b>Legacy.</b> Alternates smoothly between two positions.
/// </summary>
public class SmoothSpriteLoop : SmoothSprite {
    public Vector3 endPos;
    [NonSerialized] public Vector3 StartPos;

    [NonSerialized] public positionState pos = positionState.START;

    void Awake() {
        StartPos = transform.position;
        endPos = transform.TransformPoint(endPos);
    }

    public void Move() {
        if (pos == positionState.START) {
            base.Move(endPos, destroyOnComplete:false, isLocalPos:false);
            pos = positionState.END;
        } else if (pos == positionState.END) {
            base.Move(StartPos, destroyOnComplete:false, isLocalPos:false);
            pos = positionState.START;
        }
    }
}