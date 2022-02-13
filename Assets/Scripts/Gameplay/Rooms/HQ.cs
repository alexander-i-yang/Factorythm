using System;
using UnityEngine;

/// <summary>
/// The player's headquarters/base. Combo functionality is disabled while inside.
/// </summary>
public class HQ : Room {
    public override void CanPlayerEnter(PlayerController pc) {
        throw new NotImplementedException();
    }

    public override void OnPlayerEnter(PlayerController pc) {
        Conductor.Instance.DisableCombo();
        pc.RhythmLocked = false;

    }
    
    public override void OnPlayerExit(PlayerController pc) {
        Conductor.Instance.EnableCombo();
        pc.RhythmLocked = true;
    }

    public override bool CanPlaceHere(Machine m) {
        return false;
    }
}