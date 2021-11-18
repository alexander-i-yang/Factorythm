using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    // Called when the player presses z while hovering over this object
    public abstract void OnInteract(PlayerController p);
    
    // Called when the player drags this object around
    public abstract void OnDrag(PlayerController p, Vector3 newPos);
    
    //Called after the player is no longer holding z over this object
    //TODO: add OnDeInteract implementation in the PC
    public abstract void OnDeInteract(PlayerController p);
}