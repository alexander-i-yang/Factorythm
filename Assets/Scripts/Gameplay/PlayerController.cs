using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D _myRb;
    private BoxCollider2D _myCollider;
    private Conductor _conductor;
    private SmoothSprite _mySR;

    private Room _curRoom;
    
    private bool _wasHoldingZ;
    public Interactable CurInteractable { get; private set; }

    // Start is called before the first frame update
    void Start() {
        _myRb = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _mySR = GetComponentInChildren<SmoothSprite>();
        _conductor = FindObjectOfType<Conductor>();
    }
    
    // Update is called once per frame
    void Update() {
        int inputH = checkInputH();
        int inputV = checkInputV();
        Vector3 newPos = Vector3.zero;
        bool moved = false;
        bool attemptMove = inputH != 0 || inputV != 0;


        if (attemptMove) {
            bool onBeat = _conductor.AttemptMove();
            if (onBeat) {
                if (inputH != 0) {
                    newPos = _myRb.position + new Vector2(inputH, 0);
                }

                if (inputV != 0) {
                    newPos = _myRb.position + new Vector2(0, inputV);
                }

                _mySR.Move(newPos);
                _myRb.MovePosition(newPos);
                moved = true;
            }
        }

        bool curZ = Input.GetKey(KeyCode.Z);
        if (curZ) {
            if (!_wasHoldingZ) {
                CurInteractable = OnInteractable(transform.position);
                CurInteractable.OnInteract(this);
            } else {
                if (moved && CurInteractable != null) {
                    CurInteractable.OnDrag(this, newPos);
                }
            }
        }

        _wasHoldingZ = curZ;

        if (_conductor.SongIsOnBeat()) {
            _mySR.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else {
            _mySR.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void FixedUpdate() {
        Collider2D roomCollider = CheckRoomOverlap();
        if (roomCollider) {
            if (!_curRoom) {
                _curRoom = roomCollider.GetComponent<Room>();
                _curRoom.OnPlayerEnter(this);
            }
        } else {
            if (_curRoom) {
                _curRoom.OnPlayerExit(this);
            }

            _curRoom = null;
        }
    }

    private Collider2D CheckRoomOverlap() {
        //So the player touching the edge of the collider isn't counted as an overlap
        Vector3 alpha = new Vector3(0.05f, 0.05f);
        Vector2 topLeftCorner = _myCollider.bounds.min + alpha;
        Vector2 topRightCorner = _myCollider.bounds.max - alpha;
        Collider2D overlapCollider = Physics2D.OverlapArea(
            topLeftCorner, 
            topRightCorner,
            LayerMask.GetMask("Room")
        );
        return overlapCollider;
    }
    
    public Interactable OnInteractable(Vector3 pos) {
        RaycastHit2D hit = Physics2D.Raycast(
            pos,
            new Vector3(0, 0, 1),
            10.0f, 
            LayerMask.GetMask("Interactable"));
        if (hit.transform != null) {
            return hit.transform.GetComponent<Interactable>();
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
            // Handles.Label(transform.position, ""+_conductor.SongIsOnBeat());
        // }
        if(CurInteractable != null) Handles.Label(CurInteractable.transform.position, "Prev");
        if(_conductor) Handles.Label(transform.position, _conductor.CurCombo+"");
    }

    public void SetCurInteractable(Interactable i) {
        CurInteractable = i;
    }
}