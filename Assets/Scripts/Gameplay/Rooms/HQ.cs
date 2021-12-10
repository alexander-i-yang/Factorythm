using System;
using UnityEngine;

public class HQ : Room {
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