using System;
using UnityEngine;

public class HQ : Room {
    public override void OnPlayerEnter(PlayerController pc) {
        Conductor.Instance.DisableCombo();
    }
    
    public override void OnPlayerExit(PlayerController pc) {
        Conductor.Instance.EnableCombo();
    }
}