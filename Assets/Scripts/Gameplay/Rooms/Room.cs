using UnityEngine;

/// <summary>
/// A room that the player can enter.
/// </summary>
public abstract class Room : MonoBehaviour {
    private BoxCollider2D _myCollider;

    void Start() {
        _myCollider = GetComponent<BoxCollider2D>();
    }

    public abstract bool CanPlayerEnter(PlayerController pc);
    public virtual void OnPlayerEnter(PlayerController pc) { }
    public virtual void OnPlayerExit(PlayerController pc) { }
    public abstract bool CanPlaceHere(Machine m);
}