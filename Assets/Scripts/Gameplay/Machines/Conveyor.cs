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

    public int Angle;
    public int Index;
    public Vector2 BetweenMachines;
    public static readonly string[] ANIMATIONS = {"Right", "Down", "Left", "Up"};

    public static readonly string[] CURVE_ANIMATIONS = {
        "right up curve conv",
        "down left curve conv",
        "left up curve conv",
        "down right curve conv",
        "up right curve conv",
        "left down curve conv",
        "up left curve conv",
        "right down curve conv",
    };

    public static readonly string[] END_ANIMATIONS =
        {"right end conv", "down end conv", "left end conv", "up end conv"};

    public static readonly string[] START_ANIMATIONS =
        {"right start conv", "down start conv", "left start conv", "up start conv"};
    
    public static readonly string[] ONE_ANIMATIONS =
        {"right one conv", "down one conv", "left one conv", "up one conv"};

    private String _animationName;

    protected override void Awake() {
        base.Awake();
        _mySR = GetComponent<SpriteRenderer>();
        _myAnimator = GetComponent<Animator>();
        ResetSprite();
    }

    public override void AddOutputMachine(Machine m) {
        base.AddOutputMachine(m);
        ResetSprite();
    }

    public override void AddInputMachine(Machine m) {
        ClearInputPorts();
        base.AddInputMachine(m);
        ResetSprite();
    }

    public override void RemoveInput(Machine m) {
        base.RemoveInput(m);
        //Not sure why the base function call doesn't work so included this as a hack
        InputPorts.Clear();
        ResetSprite();
    }

    public override void RemoveOutput(Machine m) {
        base.RemoveOutput(m);
        //Not sure why the base function call doesn't work so included this as a hack
        OutputPorts.Clear();
        ResetSprite();
    }

    private void ResetSprite() {
        Vector2 inputLoc;
        Vector2 outputLoc;
        bool isCurve = false;
        bool inputEndConv = InputPorts.Count == 0;
        bool outputEndConv = OutputPorts.Count == 0;
        string[] animationArray;

        if (inputEndConv && outputEndConv) {
            inputLoc = new Vector2(-0.5f, 0);
            outputLoc = new Vector2(0.5f, 0);
        } else {
            if (inputEndConv) {
                inputLoc = -1 * OutputPorts[0].transform.localPosition;
            } else {
                inputLoc = InputPorts[0].transform.localPosition;
            }

            if (outputEndConv) {
                outputLoc = -1 * InputPorts[0].transform.localPosition;
            } else {
                outputLoc = OutputPorts[0].transform.localPosition;
            }
        }

        Vector2 betweenMachines = inputLoc - outputLoc;
        //The vector is rounded to avoid floating point math errors.
        int angleBtwn = (int) Vector2.SignedAngle(Helper.RoundVectorHalf(betweenMachines), Vector2.right) + 180;

        int index = angleBtwn / 45;
        index = index >= 8 ? 0 : index;
        Angle = angleBtwn;
        if (index % 2 == 1) {
            isCurve = true;
            Vector2 addMachines = inputLoc + outputLoc;
            int addAngleBtwn = (int) Vector2.SignedAngle(addMachines, Vector2.right) + 180;
            Angle = addAngleBtwn;
            index = addAngleBtwn / 45;

            if (angleBtwn > 180) {
                index--;
            }
        } else {
            index /= 2;
        }

        Index = index;
        BetweenMachines = betweenMachines;
        // _mySR.sprite = Sprites[index];
        //_myAnimator.SetInteger("Index", index);


        if (outputEndConv && inputEndConv) {
            animationArray = ONE_ANIMATIONS;
        } else if (outputEndConv) {
            animationArray = END_ANIMATIONS;
        } else if (inputEndConv) {
            animationArray = START_ANIMATIONS;
        } else if (isCurve) {
            animationArray = CURVE_ANIMATIONS;
        } else {
            animationArray = ANIMATIONS;
        }

        _animationName = animationArray[index];
        _myAnimator.Play("Base Layer." + _animationName, -1, 1);
    }

    public override void Tick() {
        // _myAnimator.Play("Base Layer.None");
        _myAnimator.Play("Base Layer." + _animationName, -1, 0f);
        base.Tick();
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