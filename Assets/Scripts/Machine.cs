using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Port = UnityEditor.Experimental.GraphView.Port;

public class Machine : MonoBehaviour {
    [SerializeField] public Recipe recipe;
    private int _maxInputPorts;
    private int _minInputPorts;
    private int _maxOutputPorts;
    private int _minOutputPorts;
    public int Perimeter;
    public int MaxStorage = 1;

    [NonSerialized] public List<OutputPort> OutputPorts;
    [NonSerialized] public List<InputPort> InputPorts;
    private int _ticksSinceProduced;
    private bool _pokedThisTick;
    private static Conductor _conductor;
    public List<Resource> OutputBuffer { get; private set; }
    public List<Resource> storage { get; private set; }

    protected void Awake() {
        if (recipe.InResources.Length == 0) {
            _maxInputPorts = 0;
            _minInputPorts = 0;
        }

        if (recipe.OutResources.Length == 0) {
            _maxOutputPorts = 0;
            _minOutputPorts = 0;
        }

        OutputBuffer = new List<Resource>();
        storage = new List<Resource>();
        recipe.Initiate();
        OutputPorts = new List<OutputPort>();
        InputPorts = new List<InputPort>();
    }

    void Start() {
        if(_conductor == null) _conductor = FindObjectOfType<Conductor>();
        print(FindObjectOfType<Conductor>());
        print(name + " " + _conductor);
    }
    
    private static void foreachMachine(List<MachinePort> PortList, Action<Machine> func) {
        foreach (MachinePort i in PortList) {
            var inputMachine = i.ConnectedMachine;
            if (inputMachine) {
                func(inputMachine);
            }
        }
    }

    private bool _checkEnoughInput() {
        var actualInputs = new List<Resource>();
        foreachMachine(new List<MachinePort>(InputPorts), m => actualInputs.AddRange(m.OutputBuffer));
        return recipe.CheckInputs(actualInputs);
    }

    public void DestroyOutput() {
        foreach (Resource m in OutputBuffer) {
            Destroy(m.gameObject);
        }

        OutputBuffer.Clear();
    }

    private void _produce() {
        var position = transform.position;
        if (recipe.CreatesNew) {
            foreachMachine(new List<MachinePort>(InputPorts), m => m.DestroyOutput());
            var newResources = recipe.outToList();
            foreach (Resource r in newResources) {
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
                OutputBuffer.Add(newObj.GetComponent<Resource>());
            }
        }
        else {
            foreachMachine(new List<MachinePort>(InputPorts), m => {
                OutputBuffer.AddRange(m.OutputBuffer);
                m.OutputBuffer.Clear();
            });

            foreach (Resource r in OutputBuffer) {
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                r.MySmoothSprite.Move(instantiatePos, false);
                OutputBuffer.Append(r);
            }
        }
    }

    public void PrepareTick() {
        _pokedThisTick = false;
    }

    public void Tick() {
        if (!_pokedThisTick) {
            _pokedThisTick = true;
            bool enoughInput = _checkEnoughInput();

            if (enoughInput && _ticksSinceProduced >= recipe.ticks) {
                _produce();
                _ticksSinceProduced = 0;
            }
            else {
                _ticksSinceProduced++;
            }
            foreachMachine(new List<MachinePort>(InputPorts), m => m.Tick());
        }
    }

    private void OnDrawGizmos() {
        if (storage != null && OutputBuffer != null) {
            Handles.Label(
                transform.position,
                "" + OutputBuffer.Count
            );
        }

        Handles.Label(
            transform.position + new Vector3(0, -0.2f, 0),
            "" + _ticksSinceProduced
        );
        Vector3 curPos = transform.position;
        foreachMachine(new List<MachinePort>(OutputPorts), m => {
            Vector3 direction = m.transform.position - curPos;
            MyMath.DrawArrow(curPos, direction, Color.green);
        });
    }

    public int GetNumOutputMachines() {
        int ret = 0;
        foreach (OutputPort p in OutputPorts) {
            if (p.ConnectedMachine != null) ret++;
        }

        return ret;
    }

    public void AddOutputMachine(Machine m) {
        OutputPort newPort = _conductor.InstantiateOutputPort(Vector3.zero, transform);
        newPort.ConnectedMachine = m;
        OutputPorts.Add(newPort);
    }
    
    public void AddInputMachine(Machine m) {
        print(_conductor);
        InputPort newPort = _conductor.InstantiateInputPort(Vector3.zero, transform);
        newPort.ConnectedMachine = m;
        InputPorts.Add(newPort);
    }

    /*public MachinePort InstantiateOutputPort(Vector3 newPos) {
        Machine newConveyor = Instantiate(ConveyorBelt, newPos, transform.rotation).GetComponent<Machine>();
        _allMachines.Add(newConveyor);
        return newConveyor;
    }*/
}