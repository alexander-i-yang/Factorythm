using System;
using System.Collections.Generic;
using System.Linq;using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

//The following code is heavily influenced from a state machine used in Side By Side (Producer: Yoon Lee)
//Note: Controller is the component that contains this component.
public abstract class StateMachine<MyState, MyStateInput> : MonoBehaviour
    where MyState : State<MyState, MyStateInput>
    where MyStateInput : StateInput
{
    public Dictionary<Type, MyState> StateMap { get; private set; }
    public MyState CurState { get; protected set; }
    public MyStateInput CurInput { get; private set; }

    protected void SetCurState<T>() where T : MyState {
        if (StateMap.ContainsKey(typeof(T))) {
            CurState = StateMap[typeof(T)];
        } else {
            Debug.LogError("Error: state machine doesn't include type " + typeof(T));
            Debug.Break();
        }
    }

    void InitStateInput() { CurInput = (MyStateInput) Activator.CreateInstance(typeof(MyStateInput)); }

    // Start is called before the first frame update
    void Start() {
        InitStateInput();
        
        //The below code was provided by Side By Side (Producer: Yoon Lee), who got it from Brandon Shockley
        //Gets all inherited classes of S and instantiates them using voodoo magic code I got from Brandon Shockley lol
        StateMap = new Dictionary<Type, MyState>();
        // loadedStates = new GenericDictionary<string, string>();
        Init();
        foreach (Type type in Assembly.GetAssembly(typeof(MyState)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(MyState)))) {
            MyState newState = (MyState) Activator.CreateInstance(type);
            newState.MyStateMachine = this;
            // newState.character = this;
            // newState.Init(stateInput);
            StateMap.Add(type, newState);
            // loadedStates.Add(type.FullName, RuntimeHelpers.GetHashCode(newState).ToString());
        }
        
        SetInitialState();
    }

    protected void Update() {
        CurState.Update(CurInput);
    }

    public void Transition<NextStateType>() where NextStateType : MyState {
        print("transition exiting from " + CurState + " to " + typeof(NextStateType));
        CurState.Exit(CurInput);
        SetCurState<NextStateType>();
        CurState.Enter(CurInput);
    }

    public bool IsOnState<CheckStateType>() where CheckStateType : MyState{
        return CurState.GetType() == typeof(CheckStateType);
    }

    protected virtual void Init() { }
    protected abstract void SetInitialState();
}

public abstract class State <MyState, MyStateInput> 
    where MyState : State<MyState, MyStateInput> 
    where MyStateInput : StateInput
{
    public StateMachine<MyState, MyStateInput> MyStateMachine;

    public abstract void Enter(MyStateInput i);
    public abstract void Exit(MyStateInput i);

    public abstract void Update(MyStateInput i);

    public void Transition() { }
}

public class StateInput {
}