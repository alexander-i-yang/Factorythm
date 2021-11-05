using System;
using System.Collections.Generic;
using System.Linq;using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

//The following code is heavily influenced from a state machine used in Side By Side (Producer: Yoon Lee)
//Note: Controller is the component that contains this component.
public abstract class StateMachine<Controller, MyState, StateInput> : MonoBehaviour
    where MyState : State<Controller, MyState, StateInput>
    where Controller : MonoBehaviour
{
    public Dictionary<Type, MyState> StateMap { get; private set; }
    public MyState CurState { get; protected set; }
    public Controller Con { get; private set; }

    public void SetCurState(Type nextStateType) {
        if (StateMap.ContainsKey(nextStateType)) {
            CurState = StateMap[nextStateType];
        } else {
            Debug.LogError("Error: state machine doesn't include type " + nextStateType);
            Debug.Break();
        }
    }

    // Start is called before the first frame update
    void Start() {
        Con = GetComponent<Controller>();
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
        CurState.Update();
    }

    public void Transition<NextStateType>() where NextStateType : MyState {
        StateInput nextInput = CurState.Exit();
        SetCurState(typeof(NextStateType));
        CurState.Enter(nextInput);
    }

    public bool OnState<CheckStateType>() where CheckStateType : MyState{
        return CurState.GetType() == typeof(CheckStateType);
    }

    protected virtual void Init() { }
    protected abstract void SetInitialState();
}

public abstract class State <Controller, MyState, StateInput> 
    where MyState : State<Controller, MyState, StateInput>
    where Controller : MonoBehaviour {
    public StateMachine<Controller, MyState, StateInput> MyStateMachine;

    public abstract void Enter(StateInput i);
    public abstract StateInput Exit();

    public abstract void Update();

    public void Transition() { }
}