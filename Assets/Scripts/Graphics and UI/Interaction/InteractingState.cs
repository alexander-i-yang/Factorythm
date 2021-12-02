using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InteractingState : InteractableState {
    public override void SetZPressed(bool zPressed) {
        if (!zPressed) {
            Debug.Log("Exit");
            MyStateMachine.Transition<NotInteractingState>();
        }
    }

    public override void Move(Vector3 newPos) {
        if (MyStateMachine.CurInput.CurInteractable is Draggable) {
            Debug.Log("Dragging");
            MyStateMachine.Transition<DraggingState>();
        } else {
            MyStateMachine.Transition<NotInteractingState>();
        }
    }

    public override void Enter(InteractableStateInput i) {
        MyStateMachine.CurInput.CurInteractable.OnInteract(MyStateMachine.CurInput.PC);
    }

    public override void Exit(InteractableStateInput i) {
        Debug.Log("On De interact");
        MyStateMachine.CurInput.CurInteractable.OnDeInteract(MyStateMachine.CurInput.PC);
    }

    public override void Update(InteractableStateInput i) {
        // throw new System.NotImplementedException();
    }
}