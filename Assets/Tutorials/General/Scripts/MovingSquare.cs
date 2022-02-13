using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSquare : MonoBehaviour {
    public bool move;
    public Vector2 direction;
    
    private SmoothSprite mySS;
    
    // Start is called before the first frame update
    void Start() {
        mySS = GetComponentInChildren<SmoothSprite>();
    }

    // Update is called once per frame
    /*void Update() {
        if (move) {
            transform.position += (Vector3) direction;
        }
    }*/

    public void Tick() {
        if (move) {
            mySS.Move(transform.position + (Vector3) direction);
            transform.position += (Vector3) direction;
        }
    }
}