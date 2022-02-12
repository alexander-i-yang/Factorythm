public class InteractingState : InteractableState {
    public override void SetZPressed(bool zPressed) {
        if (!zPressed) {
            MyStateMachine.Transition<NotInteractingState>();
        }
    }

    public override void Move() {
        if (MyStateMachine.CurInput.CurInteractable is Draggable) {
            MyStateMachine.Transition<DraggingState>();
        } else {
            MyStateMachine.Transition<NotInteractingState>();
        }
    }

    public override void SetXPressed(bool xPressed)
    {
    }

    public override void Enter(InteractableStateInput i) {
        MyStateMachine.CurInput.CurInteractable.OnInteract(MyStateMachine.CurInput.PC);
    }

    public override void Exit(InteractableStateInput i) {
        
    }

    public override void Update(InteractableStateInput i) {
        // throw new System.NotImplementedException();
    }
}