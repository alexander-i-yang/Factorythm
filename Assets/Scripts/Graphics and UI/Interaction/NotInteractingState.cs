using UnityEngine;

public class NotInteractingState : InteractableState {
    private bool _zWasPressed;

    public Interactable PCOnInteractable() {
        return MyStateMachine.CurInput.PC.OnInteractable();
    }

    public override void SetZPressed(bool zPressed) {
        if ((!_zWasPressed && zPressed) && PCOnInteractable()) {
            MyStateMachine.Transition<InteractingState>();
        }

        _zWasPressed = zPressed;
    }

    public override void Move(Vector3 newPos) {
        
    }

    public override void Enter(InteractableStateInput i) {
        Interactable ci = MyStateMachine.CurInput.CurInteractable;
        ci.OnDeInteract(MyStateMachine.CurInput.PC);
        MyStateMachine.CurInput.CurInteractable = null;
    }

    public override void Exit(InteractableStateInput i) {
        //Might cause problems depending on execution order, PC could move before exit is called???
        MyStateMachine.CurInput.CurInteractable = PCOnInteractable();
    }

    public override void Update(InteractableStateInput i) {
    }
}