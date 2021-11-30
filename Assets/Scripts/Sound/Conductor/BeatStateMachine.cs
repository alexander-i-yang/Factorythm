using System;
using System.Transactions;
using UnityEngine;

public class BeatStateMachine : StateMachine<BeatState, BeatStateInput> {
    protected override void Init() {
        CurInput.Con = GetComponent<Conductor>();
    }

    protected override void SetInitialState() {
        SetCurState<OffBeatState>();
    }

    public bool AttemptMove() {
        return CurState.AttemptMove(CurInput);
    }
}

public abstract class BeatState : State<BeatState, BeatStateInput> {
    public abstract bool AttemptMove(BeatStateInput input);
}

public class BeatStateInput : StateInput {
    public Conductor Con;
}