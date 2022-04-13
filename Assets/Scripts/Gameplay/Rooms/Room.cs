using UnityEngine;

/// <summary>
/// A room that the player can enter.
/// </summary>
public abstract class Room : MonoBehaviour {
    private BoxCollider2D _myCollider;

    private MachineSfx _bumpSFX;
    
    void Start() {
        _myCollider = GetComponent<BoxCollider2D>();
        
        foreach(MachineSfx s in GetComponents<MachineSfx>()) {
            if (s.startCondition == MachineSfx.StartCondition.ON_BUMP) {
                _bumpSFX = s;
            }
        }
    }

    public void PlayBumpSFX() {
        _bumpSFX.UnPause();
    }

    public abstract bool CanPlayerEnter(PlayerController pc);
    public virtual void OnPlayerEnter(PlayerController pc) { }
    public virtual void OnPlayerExit(PlayerController pc) { }
    public abstract bool CanPlaceHere(Machine m);
}