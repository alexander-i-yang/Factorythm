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
    }

    String _getNameFromClone(String cloneName) {
        int cutoffIndex = cloneName.LastIndexOf("Clone");
        if (cutoffIndex == -1) return cloneName;
        return cloneName.Substring(0, cutoffIndex);
    }

    private List<Resource> _checkEnoughInput() {
        var actualInputs = new List<Resource>();
        for (int i = 0; i < InputMachines.Count; ++i) {
            var inputMachine = InputMachines[i];
            actualInputs.AddRange(inputMachine.OutputBuffer);
        }

        var inputOccurences = occurenceDict(actualInputs);
        
        print("Inputs:");
        foreach (var rn in inputOccurences) {
            print(rn.Key + " " + rn.Value);
        }

        bool enoughInput = true;
        foreach (ResourceNum rn in recipe.InResources) {
            bool resourceInInput = false;
            foreach (var inputOccurence in inputOccurences) {
                if (inputOccurence.Value >= rn.num && inputOccurence.Key.ResourceName == rn.resource.ResourceName) {
                    resourceInInput = true;
                    break;
                }
            }
            if (!resourceInInput) {
                enoughInput = false;
                break;
            }
        }
        
        return enoughInput ? actualInputs : null;
    }

    private void _produce(List<Resource> inputs) {
        OutputBuffer.Clear();
        print(inputs.Count());
        foreach (Resource r in inputs) {
            print(r);
        }

        if (inputs.Count() == 0) {
            var newResources = recipe.outToList();
            print(newResources.Count);
            foreach (Resource r in newResources) {
                var position = transform.position;
                print(r);
                print(r.gameObject);
                var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
                var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
                print(newObj);
                OutputBuffer.Add(newObj.GetComponent<Resource>());
            }
            print(OutputBuffer.Count);
        } else {
            foreach (Resource r in inputs) {
                var position = transform.position;
                var instantiatePos = new Vector3(position.x, position.y, r.transform.position.z);
                r.gameObject.GetComponent<SmoothSprite>().Move(instantiatePos);
                // r.transform.position = instantiatePos;
                OutputBuffer.Append(r);
            }
        }
    }

    private void _executeTick() {
        foreach (Machine m in InputMachines) {
            m.GiveToOutput();
        }
    }

    public void Tick() {
        print(name);
        List<Resource> enoughInput = _checkEnoughInput();
        
        if (enoughInput != null && _ticksSinceProduced >= recipe.ticks) {
            print("Produce!");
            _executeTick();
            _produce(enoughInput);
            foreach (var r in OutputBuffer) {
                print(r);
            }
            _ticksSinceProduced = 0;
        } else {
            print(enoughInput);
            _ticksSinceProduced++;
        }

        foreach (Machine m in InputMachines) {
            m.Tick();
        }
    }

    public void GiveToOutput() {
        OutputBuffer.Clear();
    }

    //https://stackoverflow.com/questions/15862191/counting-the-number-of-times-a-value-appears-in-an-array
    public static Dictionary<Resource, int> occurenceDict(List<Resource> resources) {
        var ret = new Dictionary<Resource, int>();
        var g = resources.GroupBy( i => i );
        foreach (var grp in g) {
            ret[grp.Key] = grp.Count();
        }
        return ret;
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