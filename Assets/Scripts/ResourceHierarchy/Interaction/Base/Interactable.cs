using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    // Called when the player presses z while hovering over this object
    public abstract void OnInteract(PlayerController p);

    //Called after the player is no longer holding z over this object
    public abstract void OnDeInteract(PlayerController p);
}