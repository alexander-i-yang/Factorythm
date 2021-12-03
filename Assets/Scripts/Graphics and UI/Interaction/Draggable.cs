using UnityEngine;

public abstract class Draggable : Interactable {
    // Called when the player drags this object around
    public abstract void OnDrag(PlayerController p, Vector3 newPos);
}