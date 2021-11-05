using System;
using System.Transactions;
using UnityEngine;

public class BeatStateMachine : StateMachine<Conductor, BeatState, BeatStateInput> {
    protected override void Init() {
        
    }

    protected override void SetInitialState() {
        SetCurState(typeof(OffBeatState));
    }

    void Update() {
        base.Update();
    }
}

public abstract class BeatState : State<Conductor, BeatState, BeatStateInput> {
    
}

public struct BeatStateInput {
    
}