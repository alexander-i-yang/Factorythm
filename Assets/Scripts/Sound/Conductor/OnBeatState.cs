using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

public class OnBeatState : BeatState {
    public int MovesThisTick { get; private set; }
    public override void Enter(BeatStateInput input) {
        MovesThisTick = 0;
        // throw new System.NotImplementedException();
    }

    public override void Exit(BeatStateInput input) {
        if (MovesThisTick == 0) {
            input.Con.Tick();
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
        if (MovesThisTick == 0) {
            input.Con.Tick();
        }

        MovesThisTick++;
        input.Con.IncrCurCombo();
        return true;
    }
}