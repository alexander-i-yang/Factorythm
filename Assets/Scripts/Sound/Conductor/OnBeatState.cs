using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

public class OnBeatState : BeatState {
    public int MovesThisTick { get; private set; }
    public override void Enter(BeatStateInput input) {
        MovesThisTick = 0;
    }

    public override void Exit(BeatStateInput input) {
        if (MovesThisTick == 0) {
            Conductor.Instance.Tick();
            Conductor.Instance.SetCurCombo(0);
        }
    }

    public override void Update(BeatStateInput input) {
        if (!input.Con.SongIsOnBeat()) {
            MyStateMachine.Transition<OffBeatState>();
        }
    }

    public override bool AttemptMove(BeatStateInput input) {
        // Call tick here to move all resources with the player
        if (MovesThisTick == 0) {
            Conductor.Instance.Tick();
        }

        MovesThisTick++;
        
        input.Con.IncrCurCombo();
        return true;
    }
}