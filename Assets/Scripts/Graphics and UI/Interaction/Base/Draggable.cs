using System;
using UnityEngine;

public abstract class Draggable : Interactable {
    // Called when the player drags this object around
    public abstract void OnDrag(PlayerController p, Vector3 newPos);

    public Vector3 GetNewInitDragDirection(Vector2 dragInitDirection, Vector2 delta) {
        //If the player just started dragging, set drag dir to delta
        if (dragInitDirection == Vector2.zero) {
            return delta;
        }
        if (delta.magnitude == 0) {
            return Vector3.zero;
        }
        // If the player crosses a boundary that indicates a new drag direction, set drag dir to the direction of the boundary
        if (Math.Abs(delta.x) < 0.1 || Math.Abs(delta.y) < 0.1) {
            Vector2 ret = delta / Math.Abs(delta.magnitude);
            return ret;
        }

        // Just return the old drag init dir
        return dragInitDirection;
    }
}