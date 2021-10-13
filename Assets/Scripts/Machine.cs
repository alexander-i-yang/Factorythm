using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class Machine : MonoBehaviour {
    [SerializeField] public Recipe recipe;
    private int _maxInputPorts;
    private int _minInputPorts;
    private int _maxOutputPorts;
    private int _minOutputPorts;
    public int Perimeter;
    public int MaxStorage = 1;

    /*[NonSerialized]*/ public Port[] OutputPorts;
    /*[NonSerialized]*/ public Port[] InputPorts;
    private int _ticksSinceProduced;
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
        OutputPorts = GetComponentsInChildren<OutputPort>();
        InputPorts = GetComponentsInChildren<InputPort>();
    }

    private static void foreachMachine(Port[] PortList, Action<Machine> func) {
        foreach (Port i in PortList) {
            var inputMachine = i.ConnectedMachine;
            if (inputMachine) {
                func(inputMachine);
            }
        }
    }

    private bool _checkEnoughInput() {
        var actualInputs = new List<Resource>();
        foreachMachine(InputPorts, m => actualInputs.AddRange(m.OutputBuffer));
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
            foreachMachine(InputPorts, m => m.DestroyOutput());
            var newResources = recipe.outToList();
            foreach (Resource r in newResources) {
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
                OutputBuffer.Add(newObj.GetComponent<Resource>());
            }
        }
        else {
            foreachMachine(InputPorts, m => {
                OutputBuffer.AddRange(m.OutputBuffer);
                m.OutputBuffer.Clear();
            });

            foreach (Resource r in OutputBuffer) {
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                print(r);
                print(r.MySmoothSprite);
                r.MySmoothSprite.Move(instantiatePos, false);
                OutputBuffer.Append(r);
            }
        }
    }

    public void Tick() {
        bool enoughInput = _checkEnoughInput();

        if (enoughInput && _ticksSinceProduced >= recipe.ticks) {
            _produce();
            _ticksSinceProduced = 0;
        }
        else {
            _ticksSinceProduced++;
        }
        foreachMachine(InputPorts, m => m.Tick());
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
    }

    public int GetNumOutputMachines() {
        int ret = 0;
        foreach (Port p in OutputPorts) {
            if (p.ConnectedMachine != null) ret++;
        }

        return ret;
    }
    
    public int GetNumInputMachines() {
        int ret = 0;
        foreach (Port p in InputPorts) {
            if (p.ConnectedMachine != null) ret++;
        }

        return ret;
    }

    public void AddOutputMachine(Machine m) {
        print(OutputPorts);
        OutputPorts[0].ConnectedMachine = m;
    }
    
    public void AddInputMachine(Machine m) {
        print(InputPorts);
        InputPorts[0].ConnectedMachine = m;
    }
}