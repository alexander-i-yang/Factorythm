using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D _myRb;
    private Conductor _conductor;
    private SmoothSprite _mySR;

    // Start is called before the first frame update
    void Start() {
        _myRb = GetComponent<Rigidbody2D>();
        _mySR = GetComponentInChildren<SmoothSprite>();
        _conductor = FindObjectOfType<Conductor>();
    }

    // Update is called once per frame
    void Update() {
        int inputH = checkInputH();
        // int inputH = (int) ((_conductor.LastSongPosWholeBeats%2-0.5)*2);
        int inputV = checkInputV();
        if (_conductor.IsInputOnBeat()) {
            if (inputH != 0) {
                // if(_conductor.justTicked) {
                Vector3 newPos = _myRb.position + new Vector2(inputH, 0);
                _mySR.Move(newPos);
                _myRb.MovePosition(newPos);
            }

            if (inputV != 0) {
                Vector3 newPos = _myRb.position + new Vector2(0, inputV);
                _mySR.Move(newPos);
                _myRb.MovePosition(newPos);
            }
        }

        if (_conductor) {
            if (_conductor.IsInputOnBeat()) {
                _mySR.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else {
                _mySR.GetComponent<SpriteRenderer>().color = Color.blue;
            }
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
        if (_conductor) { 
            Handles.Label(transform.position, ""+_conductor.IsInputOnBeat());
        }
    }
}
