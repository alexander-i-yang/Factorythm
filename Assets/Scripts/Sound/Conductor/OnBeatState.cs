using System;

public class OnBeatState : BeatState {
    public int MovesThisTick { get; private set; }
    public override void Enter(BeatStateInput input) {
        MovesThisTick = 0;
    }

    public override void Exit(BeatStateInput input) {
        if (MovesThisTick == 0) {
            if (Conductor.Instance.RhythmLock) Conductor.Instance.MachineTick();
            Conductor.Instance.SetCurCombo(0);
        }
    }

    public override void Update(BeatStateInput input) {
        if (!Conductor.Instance.SongIsOnBeat()) {
            MyStateMachine.Transition<OffBeatState>();
        }
    }

    public override bool AttemptMove(BeatStateInput input) {
        // Call tick here to move all resources with the player
        if (MovesThisTick == 0) {
            Conductor.Instance.MachineTick();
        }

        MovesThisTick++;
        
        Conductor.Instance.IncrCurCombo();
        return true;
    }
}