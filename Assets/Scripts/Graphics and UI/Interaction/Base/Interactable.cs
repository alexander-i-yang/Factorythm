using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    private MachineSfx _clickSfx;
    private MachineSfx _deselectSfx;
    
    protected virtual void Awake() {
        var machineSfxs = GetComponents<MachineSfx>();
        foreach (var sfx in machineSfxs) {
            if (sfx.startCondition == MachineSfx.StartCondition.ON_CLICK) {
                _clickSfx = sfx;
            } else if (sfx.startCondition == MachineSfx.StartCondition.ON_DESELECT) {
                _deselectSfx = sfx;
            }
        }
    }

    /// <summary>
    /// Called when player presses [interact] over this object
    /// </summary>
    public virtual void OnInteract(PlayerController p) {
        if (_clickSfx) {
            _clickSfx.UnPause();
        }
    }

    /// <summary>
    /// Called after the player is no longer holding [interact] over this object
    /// </summary>
    public virtual void OnDeInteract(PlayerController p) {
        if (_deselectSfx) {
            _deselectSfx.UnPause();
        }
    }
}