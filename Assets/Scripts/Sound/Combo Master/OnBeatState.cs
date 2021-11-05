using UnityEngine;

public class OnBeatState : BeatState {
    public override void Enter(BeatStateInput i) {
        // throw new System.NotImplementedException();
    }

    public override BeatStateInput Exit() {
        return new BeatStateInput();
    }

    public override void Update() {
        if (MyStateMachine.Con.SongIsOnBeat()) {
            MyStateMachine.Transition<OffBeatState>();
        }
        // throw new System.NotImplementedException();
    }
}