using UnityEngine;
public class NotInteractingState : InteractableState {
    private bool _zWasPressed;

    public Interactable PCOnInteractable() {
        return MyStateMachine.CurInput.PC.OnInteractable();
    }

    public override void SetZPressed(bool zPressed) {
        if ((!_zWasPressed && zPressed) && PCOnInteractable()) {
            Debug.Log("Transferring to Interactable State");
            MyStateMachine.CurInput.CurInteractable = PCOnInteractable();
            MyStateMachine.Transition<InteractingState>();
        }

        _zWasPressed = zPressed;
    }

    public override void Move() {
    }

    public override void SetXPressed(bool xPressed) {
        if (MyStateMachine.CurInput.PC.OnDestructable() && xPressed)
        {
            MyStateMachine.CurInput.PC.OnDestructable().OnDestruct();
        }
    }

    public override void Enter(InteractableStateInput i) {
        if (MyStateMachine.CurInput.CurInteractable)
        {
            Interactable ci = MyStateMachine.CurInput.CurInteractable;
            ci.OnDeInteract(MyStateMachine.CurInput.PC);
        }
        MyStateMachine.CurInput.CurInteractable = null;
    }

    public override void Exit(InteractableStateInput i) {
        
    }

    public override void Update(InteractableStateInput i) {
    }
}