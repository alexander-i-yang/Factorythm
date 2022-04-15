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
        StartPos = transform.localPosition;
        endPos = transform.localPosition + endPos;
    }

    public void Move() {
        if (pos == positionState.START) {
            base.Move(endPos, destroyOnComplete:false, isLocalPos:true);
            pos = positionState.END;
        } else if (pos == positionState.END) {
            base.Move(StartPos, destroyOnComplete:false, isLocalPos:true);
            pos = positionState.START;
        }
    }

    /*public void Update() {
        if (!MainMenu.GameStarted) {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        }
    }*/
}