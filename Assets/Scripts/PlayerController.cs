using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D _myRb;
    private Conductor _conductor;
    private SmoothSprite _mySR;

    private bool _wasHoldingZ;
    private Machine _prevMachine;

    // Start is called before the first frame update
    void Start() {
        _myRb = GetComponent<Rigidbody2D>();
        _mySR = GetComponentInChildren<SmoothSprite>();
        _conductor = FindObjectOfType<Conductor>();
    }

    // Update is called once per frame
    void Update() {
        int inputH = checkInputH();
        int inputV = checkInputV();
        Vector3 newPos = Vector3.zero;
        bool moved = false;
        if (_conductor.IsInputOnBeat()) {
            if (inputH != 0) {
                newPos = _myRb.position + new Vector2(inputH, 0);
                _mySR.Move(newPos);
                _myRb.MovePosition(newPos);
                moved = true;
            }

            if (inputV != 0) {
                newPos = _myRb.position + new Vector2(0, inputV);
                _mySR.Move(newPos);
                _myRb.MovePosition(newPos);
                moved = true;
            }
        }

        bool curZ = Input.GetKey(KeyCode.Z);
        if (curZ) {
            if (!_wasHoldingZ) {
                _prevMachine = onMachine();
            } else {
                if (moved && _prevMachine != null) {
                    Machine newConveyor = _conductor.InstantiateConveyor(newPos);
                    newConveyor.AddInputMachine(_prevMachine);
                    _prevMachine.AddOutputMachine(newConveyor);
                }
            }
        }

        _wasHoldingZ = curZ;

        if (_conductor) {
            if (_conductor.IsInputOnBeat()) {
                _mySR.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else {
                _mySR.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
    }

    Machine onMachine() {
        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position, 
                new Vector3(0, 0, 1), 
                10.0f, 
                LayerMask.GetMask("Machine"));
        if (hit.transform != null) {
            return hit.transform.GetComponent<Machine>();
        } else {
            return null;
        }
    }

    int checkInputH() {
        bool leftPress = Input.GetKeyDown("left");
        bool rightPress = Input.GetKeyDown("right");
        return leftPress ? -1 : (rightPress ? 1 : 0);
    }
    
    int checkInputV() {
        bool upPress = Input.GetKeyDown("up");
        bool downPress = Input.GetKeyDown("down");
        return upPress ? 1 : (downPress ? -1 : 0);
    }
    void OnDrawGizmos() {
        // if (_conductor) { 
            // Handles.Label(transform.position, ""+_conductor.IsInputOnBeat());
        // }
        
        Handles.Label(transform.position, "" + (onMachine() != null));
    }
}
