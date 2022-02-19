using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    /// <summary>
    /// Called when player presses [interact] over this object
    /// </summary>
    public abstract void OnInteract(PlayerController p);

    /// <summary>
    /// Called after the player is no longer holding [interact] over this object
    /// </summary>
    public abstract void OnDeInteract(PlayerController p);
}