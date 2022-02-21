using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls coneyor logic. Only extra functionality is different sprite shapes.
/// </summary>
public class Conveyor : Machine {
    [SerializeField] public Sprite[] Sprites;
    private SpriteRenderer _mySR;

    private int _spriteIndex = 0;
    private Vector2 _betweenMachines;
    
    protected override void Awake() {
        base.Awake();
        base.Start();
        _mySR = GetComponent<SpriteRenderer>();
    }

    public override void AddOutputMachine(Machine m) {
        base.AddOutputMachine(m);
        ResetSprite();
    }

    public override void AddInputMachine(Machine m) {
        base.AddInputMachine(m);
        ResetSprite();
    }

    private void ResetSprite() {
        if (OutputPorts.Count == 0 || InputPorts.Count == 0) {
            return;
        }

        Vector2 inputLoc = InputPorts[0].transform.localPosition;
        Vector2 outputLoc = OutputPorts[0].transform.localPosition;
        Vector2 betweenMachines =  inputLoc-outputLoc;
        float angleBtwn = Vector2.SignedAngle(betweenMachines, Vector2.right)+180;
        int index = (int) (angleBtwn / 45);
        index = index >= 8 ? 0 : index;

        if (index % 2 == 1) {
            betweenMachines = inputLoc + outputLoc;
            angleBtwn = Vector2.SignedAngle(betweenMachines, Vector2.right) + 180;
            index = (int) (angleBtwn / 45);
            index = index >= 8 ? 0 : index;
        }
        
        _betweenMachines = betweenMachines;
        _spriteIndex = index;
        _mySR.sprite = Sprites[index];
    }

    public void OnDrawGizmos() {
        base.OnDrawGizmos();
    }
}