using System;
using UnityEngine;

public class OffBeatState : BeatState {
    public override void Enter(BeatStateInput i) {
        
        // throw new System.NotImplementedException();
    }

    public override void Exit(BeatStateInput i) {
    }

    public override void Update(BeatStateInput input) {
        if (input.Con.SongIsOnBeat()) {
            MyStateMachine.Transition<OnBeatState>();
        }
    }

    public override bool AttemptMove(BeatStateInput input) {
        input.Con.SetCurCombo(0);
        // return false;
        return true;
    }
}