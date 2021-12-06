using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pooler : MonoBehaviour {
    public GameObject ConveyorBelt;
    public GameObject OutputPort;
    public GameObject InputPort;
    
    public MachineBluePrint ConveyorBluePrint;
    
    public List<Machine> AllMachines { get; private set; }

    void Start() {
        AllMachines = new List<Machine>(FindObjectsOfType<Machine>());
    }
    
    public Machine InstantiateConveyor(Vector2 newPos, Quaternion direction) {
        Machine newConveyor = Instantiate(ConveyorBelt, new Vector3(newPos.x, newPos.y, ConveyorBelt.transform.position.z), direction).GetComponent<Machine>();
        AllMachines.Add(newConveyor);
        return newConveyor;
    }

    public OutputPort InstantiateOutputPort(Vector3 newPos, Transform parent) {
        return Instantiate(OutputPort, newPos, transform.rotation, parent).GetComponent<OutputPort>();
    }
    
    public InputPort InstantiateInputPort(Vector3 newPos, Transform parent) {
        return Instantiate(InputPort, newPos, transform.rotation, parent).GetComponent<InputPort>();
    }

    public MachineBluePrint CreateConveyorBluePrint(Vector3 pos, Quaternion rot) {
        return Instantiate(ConveyorBluePrint, pos, rot);
    }
}