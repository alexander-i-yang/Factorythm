using System;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class InteractableStateMachine : StateMachine<InteractableState, InteractableStateInput>{
    protected override void Init() {
        CurInput.PC = GetComponent<PlayerController>();
    }

    protected override void SetInitialState() {
        SetCurState<NotInteractingState>();
    }

    public void SetZPressed(bool zPressed) {
        CurState.SetZPressed(zPressed);
    }

    public void Move(Vector3 newPos) {
        CurState.Move(newPos);
    }
}

public abstract class InteractableState : State<InteractableState, InteractableStateInput> {
    public abstract void SetZPressed(bool zPressed);
    public abstract void Move(Vector3 newPos);
}

public class InteractableStateInput : StateInput {
    public PlayerController PC;
    public Interactable CurInteractable;
}