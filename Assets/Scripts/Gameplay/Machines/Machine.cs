using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// All machine logic is contained within here.
/// </summary>
public class Machine : Draggable {
    [SerializeField] public Recipe[] recipes;

    public List<OutputPort> OutputPorts = new List<OutputPort>();
    public List<InputPort> InputPorts = new List<InputPort>();
    protected int _ticksSinceProduced;
    protected bool _pokedThisTick;
    public bool _isActive = true;

    /// <summary>
    /// A list of resources that this machine just produced this tick.
    /// Currently, it just includes all resources stored in this machine.
    /// </summary>
    public Queue<Resource> OutputBuffer { get; } = new Queue<Resource>();

    public ResourceDictQueue InputBuffer { get; } = new ResourceDictQueue();
    [FormerlySerializedAs("Max Storage")] public ResourceNum[] EditorMaxStorage;
    private Dictionary<Resource, int> _maxStorage = new Dictionary<Resource, int>();

    private Vector2 _dragDirection;
    private List<ConveyorBlueprint> _dragBluePrints;

    [SerializeField] protected bool _shouldPrint;
    [SerializeField] protected bool _shouldBreak;

    protected virtual void Awake() {
        base.Awake();
        foreach (var rn in EditorMaxStorage) {
            _maxStorage.Add(rn.resource, rn.num);
        }
    }

    protected virtual void Start() {
        _dragBluePrints = new List<ConveyorBlueprint>();
    }

    /// <summary>
    /// Performs <paramref name="func"/> on all machines in <typeparamref name="portlist"/>.
    /// </summary>
    /// <param name="portList">The list of ports to iterate over</param>
    /// <param name="func">The function to perform on each machine in <paramref name="portList"/></param>
    protected static void foreachMachine(List<MachinePort> portList, Action<Machine> func) {
        foreach (MachinePort i in portList) {
            var inputMachine = i.ConnectedMachine;
            if (inputMachine) {
                func(inputMachine);
            }
        }
    }

    /// <summary>
    /// Moves <paramref name="r"/> to this machine's position.
    /// </summary>
    /// <param name="r">The resource to move</param>
    /// <param name="destroyOnComplete">Whether <paramref name="r"/> should self-destruct after moving here</param>
    protected virtual void MoveHere(Resource r, bool destroyOnComplete) {
        var position = transform.position;
        var instantiatePos = new Vector3(position.x, position.y, r.gameObject.transform.position.z);
        r.MySmoothSprite.Move(instantiatePos, destroyOnComplete);
        r.transform.position = instantiatePos;
    }

    protected virtual void MoveResourcesIn() {
        foreachMachine(new List<MachinePort>(InputPorts), m => {
            if (_shouldPrint) {
                print("Checking: " + m.name + " ");
            }

            if (m.OutputBuffer.Count > 0) {
                Resource next = m.OutputBuffer.Peek();
                if (_shouldPrint) {
                    print("Storage full: " + StorageFull(next));
                }

                if (!StorageFull(next)) {
                    next = m.OutputBuffer.Dequeue();
                    InputBuffer.Add(next);
                    MoveHere(next, false);
                }
            }
        });
    }

    protected virtual ResourceNum[] GetInCriteria(Recipe recipeObj) {
        return recipeObj.InCriteria.Resources;
    }

    public int GetStorageCount(Resource r) {
        int ret = 0;
        foreach (var resource in OutputBuffer) {
            if (Resource.CompareIDs(r.GetID(), resource.GetID())) {
                ret++;
            }
        }

        ret += InputBuffer.CountForgivable(r.GetID());
        if (_shouldPrint) {
            print("InputBuffer Total Storage: " + InputBuffer.ToList().Count());
            print("InputBuffer cur storage: " + InputBuffer.CountForgivable(r.GetID()));
        }

        return ret;
    }

    public bool StorageFull(Resource r) {
        int maxStorage = 0;
        foreach (var k in _maxStorage.Keys) {
            if (Resource.CompareIDs(r.GetID(), k.GetID())) {
                maxStorage = _maxStorage[k];
            }
        }

        if (_shouldPrint) {
            print("Storage: " + GetStorageCount(r));
            print("Max storage: " + maxStorage);
        }

        return GetStorageCount(r) >= maxStorage;
    }

    public bool NextMachineFull(Resource r) {
        bool ret = false;
        foreachMachine(new List<MachinePort>(OutputPorts), m => {
            if (m.StorageFull(r)) {
                ret = true;
            }
        });
        return ret;
    }

    protected virtual bool _checkEnoughInput(Recipe recipeObj) {
        bool ret = true;
        if (recipeObj == null) {
            print(name);
            print(transform.parent.name);
        }

        foreach (ResourceNum rn in GetInCriteria(recipeObj)) {
            if (_shouldPrint) {
                print(rn.resource.id + " " + rn.resource.GetID() + " " + rn.num);
                print(InputBuffer.ToList().Count);
                foreach (ResourceID k in InputBuffer._backer.Keys) {
                    print(k + " " + InputBuffer.CompareKeys(rn.resource.id, k) + " " + InputBuffer._backer[k].Count);
                }
            }

            if (!InputBuffer.HasEnough(rn.resource.id, rn.num)) {
                ret = false;
                break;
            }
        }

        return ret;
    }

    public ResourceDictQueue Skim(Recipe recipeObj, List<Delegate> dequeuers) {
        ResourceNum[] rns = GetInCriteria(recipeObj);
        ResourceDictQueue ret = new ResourceDictQueue();
        
        foreach (ResourceNum rn in rns) {
            InputBuffer.ForEachForgivable(rn.resource.id, q => {
                if (_shouldPrint) {
                    print("Skim prelim " + rn.resource.id);
                }

                if (q.Count >= rn.num) {
                    for (int i = 0; i < rn.num; ++i) {
                        if (_shouldPrint) print("trying again");
                        try {
                            ret.Add(q.Peek());
                            if (_shouldPrint) {
                                print("skimming " + q.Peek().name);
                            }

                            dequeuers.Add(new Func<Resource>(() => q.Dequeue()));
                        } catch {
                            Debug.LogWarning("Error: resource not found");
                        }
                    }
                }
            });
        }

        return ret;
    }

    public void PrepareTick() {
        _pokedThisTick = false;
    }

    /// <summary>
    /// Completes one cycle of logic.
    /// Produces output if needed.
    /// </summary>
    public virtual void Tick() {
        if (!_isActive) {
            OnDestruction();
        }

        if (!_pokedThisTick) {
            _pokedThisTick = true;
            MoveResourcesIn();
            foreach (var recipeObj in recipes) {
                bool enoughInput = _checkEnoughInput(recipeObj);
                if (_shouldPrint) {
                    print("Enough ticks: " + (_ticksSinceProduced >= recipeObj.ticks));
                    print("enoughInput: " + enoughInput);
                }

                if (enoughInput && _ticksSinceProduced >= recipeObj.ticks) {
                    if (_shouldPrint) {
                        print("Here I am");
                    }

                    List<Delegate> deleters = new List<Delegate>();
                    ResourceDictQueue skimmed = Skim(recipeObj, deleters);
                    List<Resource> result = recipeObj.Create(skimmed, transform.position, deleters);
                    
                    foreach (Resource r in result) {
                        if (_shouldPrint) print("Creating: " + r.name);
                        OutputBuffer.Enqueue(r);
                    }
                    foreach (var d in deleters) {
                        d.DynamicInvoke();
                    }
                    _ticksSinceProduced = 0;
                }
                else {
                    _ticksSinceProduced++;
                }

                foreachMachine(new List<MachinePort>(InputPorts), m => m.Tick());
            }
        }

        if (_shouldBreak) {
            Debug.Break();
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Unity Specific function. Defines how debug arrows are drawn.
    /// </summary>
    public void OnDrawGizmosSelected() {
        Vector3 curPos = transform.position + new Vector3(0.1f, 0.1f, 0);
        /*foreachMachine(new List<MachinePort>(OutputPorts), m => {
            Vector3 direction = m.transform.position +new Vector3(0.1f, 0.1f, 0) - curPos;
            Helper.DrawArrow(curPos, direction, Color.green);
        });
        foreachMachine(new List<MachinePort>(InputPorts), m => {
            Vector3 direction = curPos-m.transform.position - new Vector3(0.1f, 0.1f, 0);
            Helper.DrawArrow(m.transform.position, direction, Color.blue);
        });*/
    }
#endif

    public int GetNumOutputMachines() {
        int ret = 0;
        foreach (OutputPort p in OutputPorts) {
            if (p.ConnectedMachine != null) ret++;
        }

        return ret;
    }

    /// <summary>
    /// Creates a new output port that feeds into <paramref name="m"/>.
    /// Also creates new input port on <paramref name="m"/> that feeds from this machine.
    /// </summary>
    /// <param name="m">The machine to connect to</param>
    public virtual void AddOutputMachine(Machine m) {
        ClearOutputPorts();
        AddOutputPort(m);
    }

    protected void AddOutputPort(Machine m) {
        Vector3 portPos = (m.transform.position + transform.position) / 2;
        OutputPort newPort = Conductor.GetPooler().InstantiateOutputPort(portPos, transform);
        newPort.ConnectedMachine = m;
        OutputPorts.Add(newPort);
        m.AddInputMachine(this);
    }

    /// <summary>
    /// Creates a new input port that feeds from <paramref name="m"/>.
    /// </summary>
    /// <param name="m">The machine to connect to</param>
    public virtual void AddInputMachine(Machine m) {
        Vector3 portPos = (m.transform.position + transform.position) / 2;
        InputPort newPort = Conductor.GetPooler().InstantiateInputPort(portPos, transform);
        newPort.ConnectedMachine = m;
        InputPorts.Add(newPort);
    }

    public override void OnInteract(PlayerController p) { }

    public override void OnDeInteract(PlayerController p) {
        _dragDirection = Vector2.zero;
        Interactable onInteractable = p.OnInteractable();
        Machine onMachine = null;
        if (onInteractable != null) {
            onMachine = onInteractable.gameObject.GetComponent<Machine>();

            /*//Ugh why
            if (onMachine == null) {
                onMachine = onInteractable.gameObject.GetComponent<Conveyor>();
            }

            if (onMachine == null) {
                onMachine = onInteractable.gameObject.GetComponent<UnlockConveyorInner>();
            }*/
        }

        List<Machine> conveyors = InstantiateFromBluePrints(_dragBluePrints, onMachine);
        ClearDragBluePrints();
        ConfigureDragPorts(conveyors, onMachine);
    }


    /** <summary>
     *      For each blueprint in [dragBluePrints], instantiate a new machine
     *      Assumes the blueprint list is ordered by distance from this machine
     * </summary>
     **/
    public List<Machine> InstantiateFromBluePrints(List<ConveyorBlueprint> dragBluePrints, Machine onMachine) {
        List<Machine> ret = new List<Machine>();
        for (int i = 0; i < dragBluePrints.Count; i++) {
            ConveyorBlueprint bluePrint = dragBluePrints[i];
            Transform bluePrintTransform = bluePrint.transform;

            // If this is the last conveyor and the player is on a machine,
            // add onMachine to the return list, then break
            if (i == dragBluePrints.Count - 1 && onMachine) {
                ret.Add(onMachine);
                break;
            }

            // if (bluePrintTransform.position)
            // RaycastHit

            // Instantiate a new conveyor
            Conductor.checkForOverlappingMachines(bluePrintTransform.position);
            Machine instMachine = Conductor.GetPooler().InstantiateConveyor(
                bluePrintTransform.position,
                bluePrintTransform.rotation
            );
            //TODO: make more efficient
            instMachine.GetComponentInChildren<SpriteRenderer>().sprite =
                bluePrint.GetComponentInChildren<SpriteRenderer>().sprite;
            ret.Add(instMachine);
        }

        return ret;
    }

    /**
     * <summary>
     *      Sets the input and output ports of each conveyor in [conveyors].
     *      Treats the onMachine like another conveyor.
     * </summary>
     */
    public void ConfigureDragPorts(List<Machine> conveyors, Machine onMachine) {
        for (int i = 0; i < conveyors.Count; ++i) {
            Machine curMachine = conveyors[i];
            // If this is the first conveyor in the line, set the machine to it.
            if (i == 0) {
                AddOutputMachine(curMachine);
            }

            // If this is the last conveyor in the line and the player is on a machine,
            // Set the output of the new conveyor to the new machine
            if (i >= conveyors.Count - 1) {
                break;
            }
            else {
                curMachine.AddOutputMachine(conveyors[i + 1]);
            }
        }
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        ClearDragBluePrints();

        Vector2 delta = newPos - transform.position;
        _dragDirection = GetNewInitDragDirection(_dragDirection, delta);

        // Get the component of delta in the direction of dir
        // Then draw blueprints in that direction
        int n1 = (int) Math.Round(Math.Abs(Vector2.Dot(delta, _dragDirection)));

        Vector3 startPos2 = transform.position + (Vector3) _dragDirection * n1;
        Vector2 orthoDir = delta - n1 * _dragDirection;
        int n2 = (int) Math.Round(orthoDir.magnitude);
        orthoDir.Normalize();

        _dragBluePrints.AddRange(RenderConveyorBluePrintLine(n1, transform.position, _dragDirection, orthoDir));

        // Get the component of delta orthogonal to the direction of dir
        // Then draw blueprints in that direction
        if (n2 != 0) {
            _dragBluePrints.AddRange(RenderConveyorBluePrintLine(n2, startPos2, orthoDir));
        }
    }


    public void ClearDragBluePrints() {
        foreach (ConveyorBlueprint m in _dragBluePrints) {
            Destroy(m.gameObject);
        }

        _dragBluePrints.Clear();
    }

    /// <summary>
    /// Draws <paramref name="n"/> conveyors starting at <paramref name="startPos"/>,
    /// going in direction <paramref name="dir"/>
    /// </summary>
    /// <returns>List of the conveyor belt blueprints to draw</returns>
    public List<ConveyorBlueprint> RenderConveyorBluePrintLine(int n, Vector3 startPos, Vector2 dir, Vector2 orthoDir) {
        List<ConveyorBlueprint> ret = new List<ConveyorBlueprint>();
        // Account for dir.x being 0 which causes a div by 0 error
        for (int i = 1; i < n + 1; ++i) {
            ConveyorBlueprint add = Conductor.GetPooler().CreateConveyorBluePrint(startPos + (Vector3) (dir * i));
            if (i < n) {
                add.SetEdgeSprite(dir);
            }
            else if (orthoDir != Vector2.zero) {
                add.SetCornerSprite(dir, orthoDir);
            }
            else {
                add.SetEdgeSprite(dir);
            }

            ret.Add(add);
        }

        return ret;
    }

    public void OnDestruction() {
        ClearOutputPorts();
        ClearInputPorts();
        ClearResources();
    }

    public void ClearOutputPorts() {
        foreach (OutputPort port in OutputPorts) {
            port.ConnectedMachine.RemoveInput(this);

            //The following destroys all conveyors that were attached with this machine
            //if (typeof(Conveyor).IsInstanceOfType(port.ConnectedMachine))
            //{
            //    port.ConnectedMachine
            //        .GetComponent<NormalDestroy>().OnDestruct();
            //}
        }

        OutputPorts.Clear();
    }

    public void ClearInputPorts() {
        foreach (InputPort port in InputPorts) {
            port.ConnectedMachine.RemoveOutput(this);
        }

        InputPorts.Clear();
    }

    public void ClearResources() {
        foreach (Resource r in InputBuffer.ToList()) {
            Destroy(r.gameObject);
        }

        foreach (Resource r in OutputBuffer) {
            Destroy(r.gameObject);
        }

        InputBuffer.Clear();
        OutputBuffer.Clear();
    }

    public virtual void RemoveOutput(Machine m) {
        foreach (OutputPort port in OutputPorts) {
            if (port.ConnectedMachine.Equals(m)) {
                OutputPorts.Remove(port);
                break;
            }
        }
    }

    public virtual void RemoveInput(Machine m) {
        foreach (InputPort port in InputPorts) {
            if (port.ConnectedMachine.Equals(m)) {
                InputPorts.Remove(port);
                break;
            }
        }
    }

    public List<ConveyorBlueprint> RenderConveyorBluePrintLine(int n, Vector3 startPos, Vector2 dir) {
        return RenderConveyorBluePrintLine(n, startPos, dir, Vector2.zero);
    }

    public T GetPortToPoke<T>(List<T> ports) {
        ports.Sort();
        return ports[0];
    }

    public OutputPort GetOutputPortToPoke() {
        return GetPortToPoke<OutputPort>(OutputPorts);
    }

    public InputPort GetInputPortToPoke() {
        return GetPortToPoke<InputPort>(InputPorts);
    }
}