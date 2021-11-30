using UnityEngine;

public interface Draggable {
    // Called when the player drags this object around
    public abstract void OnDrag(PlayerController p, Vector3 newPos);
}