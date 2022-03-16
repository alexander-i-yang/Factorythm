using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls coneyor logic. Only extra functionality is different sprite shapes.
/// </summary>
public class Conveyor : Machine {
    [SerializeField] public Sprite[] Sprites;
    private SpriteRenderer _mySR;
    private Animator _myAnimator;
    
    private int _spriteIndex = 0;
    private Vector2 _betweenMachines;
    private Vector3 _inputLoc;
    private Vector3 _outputLoc;

    public float Angle;
    public Vector2 BetweenMachines;
    public static readonly string[] ANIMATIONS = {"ConveyorRLB", "ConveyorDL", "ConveyorUPL", "ConveyorLU", "ConveyorRLT", "ConveyorUR", "ConveyorUDR", "ConveyorRD"};

    protected override void Awake() {
        base.Awake();
        _mySR = GetComponent<SpriteRenderer>();
        _myAnimator = GetComponent<Animator>();
        ResetSprite();
    }

    protected override void Start() {
        base.Start();
    }

    public override void AddOutputMachine(Machine m) {
        ClearOutputPorts();
        base.AddOutputMachine(m);
        ResetSprite();
    }

    public override void AddInputMachine(Machine m) {
        ClearInputPorts();
        base.AddInputMachine(m);
        ResetSprite();
    }

    private void ResetSprite() {
        if (OutputPorts.Count == 0 && InputPorts.Count == 0) {
            return;
        }

        Vector2 inputLoc; 
        Vector2 outputLoc;

        if (InputPorts.Count() == 0) {
            inputLoc = -1*OutputPorts[0].transform.localPosition;
        } else {
            inputLoc = InputPorts[0].transform.localPosition;
        }
        
        if (OutputPorts.Count() == 0) {
            outputLoc = -1*InputPorts[0].transform.localPosition;
        } else {
            outputLoc = OutputPorts[0].transform.localPosition;
        }

        _inputLoc = inputLoc;
        _outputLoc = outputLoc;
        Vector2 betweenMachines =  inputLoc-outputLoc;
        BetweenMachines = betweenMachines;
        _betweenMachines = Helper.RoundVectorHalf(_betweenMachines);
        //The vector is rounded to avoid floating point math errors.
        int angleBtwn = (int) Vector2.SignedAngle(Helper.RoundVectorHalf(betweenMachines), Vector2.right)+180;
        Angle = angleBtwn;
        int index = (int) (angleBtwn / 45);
        index = index >= 8 ? 0 : index;

        if (index % 2 == 1) {
            betweenMachines = inputLoc + outputLoc;
            angleBtwn = (int) Vector2.SignedAngle(betweenMachines, Vector2.right) + 180;
            index = (int) (angleBtwn / 45);
            index = index >= 8 ? 0 : index;
        }

        _spriteIndex = index;
        // _mySR.sprite = Sprites[index];
        //_myAnimator.SetInteger("Index", index);

        String animationName = ANIMATIONS[index];
        _myAnimator.Play("Base Layer." + animationName);
    }

    /*#if UNITY_EDITOR
    public void OnDrawGizmos() {
        base.OnDrawGizmos();
        Handles.Label(transform.position + new Vector3(0.1f, 0.1f, -3), "I " + _inputLoc);
        Handles.Label(transform.position + new Vector3(0.1f, 0.3f, -3), "O " + _outputLoc);
        Handles.Label(transform.position + new Vector3(0.1f, 0.5f, -3), "S " + _spriteIndex + " A " + Angle);
        Handles.Label(transform.position + new Vector3(0.1f, 0.7f, -3), "BH" + _betweenMachines + " B" + BetweenMachines);  
    }
    #endif*/
}