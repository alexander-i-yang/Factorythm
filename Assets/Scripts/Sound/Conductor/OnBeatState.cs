using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

public class OnBeatState : BeatState {
    private bool _movedThisTick;
    public override void Enter(BeatStateInput input) {
        _movedThisTick = false;
        // throw new System.NotImplementedException();
    }

    public override void Exit(BeatStateInput input) {
        Debug.Log("Exit >????");
        if (!_movedThisTick) {
            input.Con.SetCurCombo(0);
        }
    }

    public override void Update(BeatStateInput input) {
        if (!input.Con.SongIsOnBeat()) {
            MyStateMachine.Transition<OffBeatState>();
        }
        // throw new System.NotImplementedException();
    }

    public override bool AttemptMove(BeatStateInput input) {
        _movedThisTick = true;
        input.Con.IncrCurCombo();
        return true;
    }
}