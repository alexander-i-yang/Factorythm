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

    public List<Machine> InputMachines;
    public List<Machine> OutputMachines;
    public OutputPort[] OutputPorts;
    private int _ticksSinceProduced;
    public List<Resource> OutputBuffer { get; private set; }
    public List<Resource> storage { get; private set; }

    protected void Start() {
        if (recipe.InResources.Length == 0) {
            _maxInputPorts = 0;
            _minInputPorts = 0;
        }

        if (recipe.OutResources.Length == 0) {
            _maxOutputPorts = 0;
            _minOutputPorts = 0;
        }

        InputMachines = InputMachines == null ? new List<Machine>() : InputMachines;
        OutputMachines = OutputMachines == null ? new List<Machine>() : OutputMachines;
        OutputBuffer = new List<Resource>();
        storage = new List<Resource>();
        recipe.Initiate();

        print(name);
        OutputPorts = GetComponentsInChildren<OutputPort>();
        foreach (OutputPort o in OutputPorts) {
            print(o.GetRelativeRotation());
        }
    }

    private bool _checkEnoughInput() {
        var actualInputs = new List<Resource>();
        for (int i = 0; i < InputMachines.Count; ++i) {
            var inputMachine = InputMachines[i];
            actualInputs.AddRange(inputMachine.OutputBuffer);
        }

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
        print(name);
        print(recipe.CreatesNew);
        if (recipe.CreatesNew) {
            foreach (Machine im in InputMachines) {
                im.DestroyOutput();
            }
            var newResources = recipe.outToList();
            print(newResources.Count);
            foreach (Resource r in newResources) {
                print(r);
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
                OutputBuffer.Add(newObj.GetComponent<Resource>());
                print(newObj);
            }
        } else {
            foreach (Machine im in InputMachines) {
                OutputBuffer.AddRange(im.OutputBuffer);
                im.OutputBuffer.Clear();
            }

            foreach (Resource r in OutputBuffer) {
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
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
        } else {
            _ticksSinceProduced++;
        }

        foreach (Machine m in InputMachines) {
            m.Tick();
        }
    }

    private void OnDrawGizmos() {
        if (storage != null && OutputBuffer != null) {
            Handles.Label(
                transform.position, 
                ""+OutputBuffer.Count
            );
        }

        Handles.Label(
            transform.position+new Vector3(0, -0.2f, 0), 
            "" + _ticksSinceProduced
        );
    }
}