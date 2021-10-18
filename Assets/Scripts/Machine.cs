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

    [NonSerialized] public List<OutputPort> OutputPorts = new List<OutputPort>();
    [NonSerialized] public List<InputPort> InputPorts = new List<InputPort>();
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
    }

    void Start() {
        if(_conductor == null) _conductor = FindObjectOfType<Conductor>();
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
        OutputBuffer.Clear();
    }

    private void _produce() {
        var position = transform.position;
        if (recipe.CreatesNew) {
            foreachMachine(new List<MachinePort>(InputPorts), m => {
                foreach (Resource r in m.OutputBuffer) {
                    var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                    r.MySmoothSprite.Move(instantiatePos, true);
                }
            });
            foreachMachine(new List<MachinePort>(InputPorts), m => m.DestroyOutput());
            var newResources = recipe.outToList();
            foreach (Resource r in newResources) {
                var instantiatePos = new Vector3(position.x, position.y, 0);
                var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
                OutputBuffer.Add(newObj.GetComponent<Resource>());
            }
        } else {
            foreachMachine(new List<MachinePort>(InputPorts), m => {
                OutputBuffer.AddRange(m.OutputBuffer);
                m.OutputBuffer.Clear();
            });

            foreach (Resource r in OutputBuffer) {
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                // print(instantiatePos);
                r.MySmoothSprite.Move(instantiatePos, false);
                // OutputBuffer.Append(r);
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

    public void OnDrawGizmos() {
        // if (storage != null && OutputBuffer != null) {
        //     Handles.Label(
        //         transform.position,
        //         "" + OutputBuffer.Count
        //     );
        // }

        // Handles.Label(
        //     transform.position + new Vector3(0, -0.2f, 0),
        //     "" + _ticksSinceProduced
        // );
        Vector3 curPos = transform.position + new Vector3(0.1f, 0.1f, 0);
        foreachMachine(new List<MachinePort>(OutputPorts), m => {
            Vector3 direction = m.transform.position +new Vector3(0.1f, 0.1f, 0) - curPos;
            MyMath.DrawArrow(curPos, direction, Color.green);
        });
        // foreachMachine(new List<MachinePort>(InputPorts), m => {
        //     Vector3 direction = curPos-m.transform.position - new Vector3(0.1f, 0.1f, 0);
        //     MyMath.DrawArrow(m.transform.position, direction, Color.blue);
        // });
    }

    public int GetNumOutputMachines() {
        int ret = 0;
        foreach (OutputPort p in OutputPorts) {
            if (p.ConnectedMachine != null) ret++;
        }

        return ret;
    }

    public void AddOutputMachine(Machine m, Vector3 pos) {
        OutputPort newPort = _conductor.InstantiateOutputPort(pos, transform);
        newPort.ConnectedMachine = m;
        OutputPorts = new List<OutputPort>();
        OutputPorts.Add(newPort);
    }
    
    public void AddInputMachine(Machine m, Vector3 pos) {
        InputPort newPort = _conductor.InstantiateInputPort(pos, transform);
        newPort.ConnectedMachine = m;
        InputPorts = new List<InputPort>();
        InputPorts.Add(newPort);
    }
}