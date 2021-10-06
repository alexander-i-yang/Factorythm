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
    public int Storage = 1;

    public List<Machine> InputMachines;
    public List<Machine> OutputMachines;
    private int _ticksSinceProduced;
    public List<Resource> OutputBuffer { get; private set; }
    public List<Resource> storage { get; private set; }

    void Start() {
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

    private bool _checkEnoughInput() {
        var actualInputs = new List<Resource>();
        for (int i = 0; i < InputMachines.Count; ++i) {
            var inputMachine = InputMachines[i];
            actualInputs.AddRange(inputMachine.OutputBuffer);
        }
        
        /*foreach (Resource r in actualInputs) {
            print(r.name);
        }*/
        
        var inputOccurences = occurenceDict(actualInputs);
        /*String ret = "";
        foreach (KeyValuePair<Resource, int> x in inputOccurences) {
            ret += x.Key + " " + x.Value;
        }
        print("Input occurences: "  + ret);*/
        
        bool enoughInput = true;
        foreach (ResourceNum rn in recipe.InResources) {
            if (!inputOccurences.ContainsKey(rn.resource) || rn.num > inputOccurences[rn.resource]) {
                enoughInput = false;
            }
        }

        return enoughInput;
    }

    private void _executeTick() {
        print("Executing Tick!");
        foreach (Machine m in InputMachines) {
            m.GiveToOutput();
        }
        OutputBuffer = recipe.outToList();
        print("Output buffer:" + OutputBuffer.Count());
    }

    public void Tick() {
        print(name);
        bool enoughInput = _checkEnoughInput();
        if (enoughInput && _ticksSinceProduced >= recipe.ticks) {
            _executeTick();
            _ticksSinceProduced = 0;
        } else {
            _ticksSinceProduced++;
        }

        foreach (Machine m in InputMachines) {
            m.Tick();
        }

        /*var actualInputs = new List<Resource>();
        bool enoughInput = InputMachines.Count == 0;
        int inputMachinesUsed = 0;
        OutputBuffer = storage;
        storage.Clear();
        for (int i = 0; i<InputMachines.Count; ++i) {
            Machine m = InputMachines[i];
            inputMachinesUsed = i;
            m.Tick();
            if (_ticksSinceProduced >= recipe.ticks) {
                actualInputs.Concat(m.OutputBuffer);

                var inputOccurences = occurenceDict(actualInputs);
                enoughInput = true;
                foreach (ResourceNum rn in recipe.InResources) {
                    print(rn.num);
                    String ret = "";
                    foreach (KeyValuePair<Resource, int> x in inputOccurences) {
                        ret += x.Key + " " + x.Value;
                    }
                    print(ret);
                    if (inputOccurences.ContainsKey(rn.resource) && rn.num > inputOccurences[rn.resource]) {
                        enoughInput = false;
                        //Todo: breaking the code in the loop will skip ticks for any machines.
                        break;
                    }
                }
            
                if (enoughInput) {
                    print("break!");
                    break;
                }
            }
        }
        if (enoughInput && _ticksSinceProduced >= recipe.ticks) {
            storage = recipe.outToList();
            print(storage.Count);
            
            Debug.Break();
            for (int j = 0; j < inputMachinesUsed; ++j) {
                InputMachines[j].GiveToOutput();
            }
            
            _ticksSinceProduced = 0;
            print(name + " Produce!");
            print(storage.Count);
            return;
        }

        _ticksSinceProduced++;
        print(name + " " + OutputBuffer.Count);*/
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