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
        CurInput.NewPos = newPos;
        CurState.Move();
    }

    public void SetXPressed(bool xPressed)
    {
        CurState.SetXPressed(xPressed);
    }
}

public abstract class InteractableState : State<InteractableState, InteractableStateInput> {
    public abstract void SetZPressed(bool zPressed);

    public abstract void Move();

    public abstract void SetXPressed(bool xPressed);
}

public class InteractableStateInput : StateInput {
    public PlayerController PC;
    public Vector3 NewPos;
    public Interactable CurInteractable;
    public Draggable CurDraggable;
    public Destructable CurDestructable;
}