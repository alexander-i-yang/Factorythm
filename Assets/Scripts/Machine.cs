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
    protected static Conductor OurConductor;
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
        if(OurConductor == null) OurConductor = FindObjectOfType<Conductor>();
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

    public void ClearOutput() {
        OutputBuffer.Clear();
    }

    public void MoveHere(Resource r, bool destroyOnComplete) {
        var position = transform.position;
        var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
        r.MySmoothSprite.Move(instantiatePos, destroyOnComplete);
    }

    protected void MoveResourcesIn() {
        foreachMachine(new List<MachinePort>(InputPorts), m => {
            OutputBuffer.AddRange(m.OutputBuffer);
            m.OutputBuffer.Clear();
        });

        foreach (Resource r in OutputBuffer) {
            MoveHere(r, false);
        }
    }

    protected virtual void CreateOutput() {
        var position = transform.position;
        var resourcesToCreate = recipe.outToList();
        foreach (Resource r in resourcesToCreate) {
            var instantiatePos = new Vector3(position.x, position.y, r.transform.position.z);
            var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
            OutputBuffer.Add(newObj.GetComponent<Resource>());
        }
    }

    public void MoveAndDestroy() {
        //Foreach resource in each port's input buffer, move to this machine
        foreachMachine(new List<MachinePort>(InputPorts), m => {
            foreach (Resource resource in m.OutputBuffer) {
                MoveHere(resource, true);
            }
        });
        //Empty the output list of the input machines
        foreachMachine(new List<MachinePort>(InputPorts), m => m.ClearOutput());
        //Create new resources based on the old ones
        CreateOutput();
    }

    protected virtual void _produce() {
        if (recipe.CreatesNewOutput) {
            MoveAndDestroy();
        } else {
            MoveResourcesIn();
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
            Helper.DrawArrow(curPos, direction, Color.green);
        });
        // foreachMachine(new List<MachinePort>(InputPorts), m => {
        //     Vector3 direction = curPos-m.transform.position - new Vector3(0.1f, 0.1f, 0);
        //     Helper.DrawArrow(m.transform.position, direction, Color.blue);
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
        OutputPort newPort = OurConductor.InstantiateOutputPort(pos, transform);
        newPort.ConnectedMachine = m;
        OutputPorts = new List<OutputPort>();
        OutputPorts.Add(newPort);
    }
    
    public void AddInputMachine(Machine m, Vector3 pos) {
        InputPort newPort = OurConductor.InstantiateInputPort(pos, transform);
        newPort.ConnectedMachine = m;
        InputPorts = new List<InputPort>();
        InputPorts.Add(newPort);
    }
}