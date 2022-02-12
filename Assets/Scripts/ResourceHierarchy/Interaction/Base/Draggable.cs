using System;
using UnityEngine;

public abstract class Draggable : Interactable {
    // Called when the player drags this object around
    public abstract void OnDrag(PlayerController p, Vector3 newPos);

    public Vector3 GetNewInitDragDirection(Vector2 dragInitDirection, Vector2 delta)
    {
        //If the player just started dragging, set drag dir to delta
        if (dragInitDirection == Vector2.zero)
        {
            return delta;
        }
        else
        {
            // If the player crosses a boundary that indicates a new drag direction, set drag dir to the direction of the boundary
            float deltaAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            float dragDirAngle = Mathf.Atan2(dragInitDirection.y, dragInitDirection.x) * Mathf.Rad2Deg;
            if (Math.Abs(deltaAngle - dragDirAngle) > 0.00001 && Math.Abs(deltaAngle - dragDirAngle) % 90 < 0.00001)
            {
                return delta / delta.magnitude;
            }
        }
        // Just return the old drag init dir
        return dragInitDirection;
    }
}