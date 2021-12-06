using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Port = UnityEditor.Experimental.GraphView.Port;

public class Machine : Draggable {
    [SerializeField] public Recipe recipe;
    // private int _maxInputPorts;
    // private int _minInputPorts;
    // private int _maxOutputPorts;
    // private int _minOutputPorts;
    // public int Perimeter;
    // public int MaxStorage = 1;

    [NonSerialized] public List<OutputPort> OutputPorts = new List<OutputPort>();
    [NonSerialized] public List<InputPort> InputPorts = new List<InputPort>();
    private int _ticksSinceProduced;
    private bool _pokedThisTick;

    public List<Resource> OutputBuffer { get; private set; }
    public List<Resource> storage { get; private set; }

    private Vector2 _dragDirection;
    private List<MachineBluePrint> _dragBluePrints;
    
    public bool ShouldPrint;
    public bool ShouldBreak;
    
    protected void Awake() {
        if (recipe.InResources.Length == 0) {
            // _maxInputPorts = 0;
            // _minInputPorts = 0;
        }

        if (recipe.OutResources.Length == 0) {
            // _maxOutputPorts = 0;
            // _minOutputPorts = 0;
        }

        OutputBuffer = new List<Resource>();
        storage = new List<Resource>();
        recipe.Initiate();
    }
    
    void Start() {
        _dragBluePrints = new List<MachineBluePrint>();
    }

    bool _shouldPrint() {
        return ShouldPrint;
        // return transform.name == Helper.Consts.NAME;
    }

    bool _shouldBreak() {
        return ShouldBreak;
    }

    private static void foreachMachine(List<MachinePort> portList, Action<Machine> func) {
        foreach (MachinePort i in portList) {
            var inputMachine = i.ConnectedMachine;
            if (inputMachine) {
                func(inputMachine);
            }
        }
    }

    private bool _checkEnoughInput() {
        var actualInputs = new List<Resource>();
        foreachMachine(new List<MachinePort>(InputPorts), m => actualInputs.AddRange(m.OutputBuffer));
        bool ret = recipe.CheckInputs(actualInputs);
        if (_shouldPrint()) {
            print("Input resources: ");
            foreach (Resource i in actualInputs) { print(i);}
            print("Enough input: "  +ret);
        }

        return ret;
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
            MoveHere(r, _shouldDestroyInputs());
        }
    }

    protected virtual void CreateOutput() {
        var position = transform.position;
        var resourcesToCreate = recipe.outToList();
        foreach (Resource r in resourcesToCreate) {
            var instantiatePos = new Vector3(position.x, position.y, r.transform.position.z);
            var newObj = Instantiate(r.transform, instantiatePos, transform.rotation);
            if (_shouldPrint()) {
                print("Adding new resource: " + r);
            }
            OutputBuffer.Add(newObj.GetComponent<Resource>());
        }
    }
    
    // Returns true if the machine destroys its input resources after moving them in.
    protected virtual bool _shouldDestroyInputs() {
        return recipe.CreatesNewOutput;
    }

    public void MoveAndDestroy() {
        //Foreach resource in each port's input buffer, move to this machine
        foreachMachine(new List<MachinePort>(InputPorts), m => {
            foreach (Resource resource in m.OutputBuffer) {
                if (_shouldPrint()) {
                    print("moving resource: " + m + resource);
                }
                MoveHere(resource, _shouldDestroyInputs());
            }
        });
        //Empty the output list of the input machines
        if (_shouldPrint()) {
            print("Emptying input ports' output");
        }
        foreachMachine(new List<MachinePort>(InputPorts), m => m.ClearOutput());
        //Create new resources based on the old ones
        if (_shouldPrint()) {
            print("Creating output");
        }
        CreateOutput();
    }

    protected virtual void _produce() {
        if (recipe.CreatesNewOutput) {
            if (_shouldPrint()) {
                print("moving and destroying");
            }
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
            if (_shouldPrint()) {
                print("Enough ticks: " + (_ticksSinceProduced >= recipe.ticks));
            }

            if (enoughInput && _ticksSinceProduced >= recipe.ticks) {
                _produce();
                _ticksSinceProduced = 0;
            }
            else {
                _ticksSinceProduced++;
            }
            foreachMachine(new List<MachinePort>(InputPorts), m => m.Tick());
        }

        if (_shouldBreak()) {
            Debug.Break();
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

    public void AddOutputMachine(Machine m) {
        Vector3 portPos = (m.transform.position + transform.position) / 2;
        OutputPort newPort = Conductor.GetPooler().InstantiateOutputPort(portPos, transform);
        newPort.ConnectedMachine = m;
        OutputPorts = new List<OutputPort>();
        OutputPorts.Add(newPort);
    }
    
    public void AddInputMachine(Machine m, Vector3 pos) {
        InputPort newPort = Conductor.GetPooler().InstantiateInputPort(pos, transform);
        newPort.ConnectedMachine = m;
        InputPorts = new List<InputPort>();
        InputPorts.Add(newPort);
    }

    public override void OnInteract(PlayerController p) {
        // throw new NotImplementedException();
    }

    public override void OnDeInteract(PlayerController p) {
        _dragDirection = Vector2.zero;
        Interactable onInteractable = p.OnInteractable();
        Machine onMachine = null;
        if (onInteractable != null) {
            onMachine = onInteractable.gameObject.GetComponent<Machine>();
        }
        List<Machine> conveyors = InstantiateFromBluePrints(_dragBluePrints, onMachine);
        ClearDragBluePrints();

        for (int i = 0; i < conveyors.Count; ++i) {
            Machine curMachine = conveyors[i];
            // If this is the last conveyor in the line and the player is on a machine,
            // Set the output of the new conveyor to the new machine
            if (i > conveyors.Count - 1) {
                if (onMachine) {
                    curMachine.AddOutputMachine(onMachine);
                }
            }
        }
        /*Vector3 newPos = p.transform.position;
        print(p.OnInteractable(newPos));
        Interactable nextInteractable = p.OnInteractable(newPos);
        Machine outMachine;
        if (nextInteractable == null) {
            Vector3 direction = newPos-transform.position;
            float angleRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angleRot);
            Vector2 conveyorPos = new Vector2(newPos.x, newPos.y);
            outMachine = Conductor.GetPooler().InstantiateConveyor(conveyorPos, rotation);
        } else {
            outMachine = nextInteractable.GetComponent<Machine>();
        }
        Vector3 portPos = (transform.position + newPos) / 2;
        outMachine.AddInputMachine(this, portPos);
        AddOutputMachine(outMachine, portPos);*/
    }
    
    
    /** <summary>
     *      For each blueprint in the blueprint list, instantiate a new machine
     *      Assumes the blueprint list is ordered by distance from this machine
     * </summary>
     **/
    public List<Machine> InstantiateFromBluePrints(List<MachineBluePrint> dragBluePrints, Machine onMachine) {
        List<Machine> ret = new List<Machine>();
        for (int i = 0; i < dragBluePrints.Count; i++) {
            MachineBluePrint bluePrint = dragBluePrints[i];
            Transform bluePrintTransform = bluePrint.transform;

            // If this is the last conveyor and the player is on a machine, break
            if (i == dragBluePrints.Count - 1 && onMachine) {
                break;
            }

            Machine machine = Instantiate(bluePrint.MachineCopy);
            Transform machineTransform = machine.transform;
            
            machineTransform.rotation = bluePrintTransform.rotation;
            machineTransform.position = bluePrintTransform.position;
            ret.Add(machine);
        }
        return ret;
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        ClearDragBluePrints();
        
        Vector2 delta = newPos - transform.position;
        _dragDirection = GetNewInitDragDirection(_dragDirection, delta);
        
        // Get the component of delta in the direction of dir
        int n1 = (int)Math.Abs(Vector2.Dot(delta, _dragDirection));
        _dragBluePrints.AddRange(RenderConveyorBluePrintLine(n1, transform.position, _dragDirection));
        
        // Get the component of delta orthogonal to the direction of dir
        Vector3 startPos2 = transform.position + (Vector3)_dragDirection * n1;
        Vector2 orthoDir = delta - n1*_dragDirection;
        int n2 = (int) Math.Abs(orthoDir.x + orthoDir.y);
        if (n2 != 0) {
            orthoDir = orthoDir / n2;
            _dragBluePrints.AddRange(RenderConveyorBluePrintLine(n2, startPos2, orthoDir));
        }
    }

    public void ClearDragBluePrints() {
        foreach (MachineBluePrint m in _dragBluePrints) {
            Destroy(m.gameObject);
        }
        _dragBluePrints.Clear();
    }

    public Vector3 GetNewInitDragDirection(Vector2 dragInitDirection, Vector2 delta) {
        //If the player just started dragging, set drag dir to delta
        if (dragInitDirection == Vector2.zero) {
            return delta;
        } else {
            // If the player crosses a boundary that indicates a new drag direction, set drag dir to the direction of the boundary
            float deltaAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            float dragDirAngle = Mathf.Atan2(dragInitDirection.y, dragInitDirection.x) * Mathf.Rad2Deg;
            if (Math.Abs(deltaAngle - dragDirAngle) > 0.00001 && Math.Abs(deltaAngle - dragDirAngle) % 90 < 0.00001) {
                return delta / delta.magnitude;
            }
        }
        // Just return the old drag init dir
        return dragInitDirection;
    }

    //Creates n conveyors starting at startPos, going in direction dir
    public List<MachineBluePrint> RenderConveyorBluePrintLine(int n, Vector3 startPos, Vector2 dir) {
        List<MachineBluePrint> ret = new List<MachineBluePrint>();
        float angleRot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // Account for dir.x being 0 which causes a div by 0 error

        Quaternion rotation = Quaternion.Euler(0, 0, angleRot);
        for (int i = 1; i < n+1; ++i) {
            ret.Add(Conductor.GetPooler().CreateConveyorBluePrint(startPos + (Vector3)(dir*i), rotation));
        }
        return ret;
    }
}