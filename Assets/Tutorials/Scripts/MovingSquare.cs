using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSquare : MonoBehaviour {
    public bool move;
    public Vector2 velocity;
    private SmoothSprite _mySS;
    // Start is called before the first frame update
    void Start() {
        _mySS = GetComponentInChildren<SmoothSprite>();
    }

    // Update is called once per frame
    public void Tick() {
        if (move) {
            Vector3 newPos = transform.position + (Vector3) velocity;
            _mySS.Move(newPos);
            transform.position = newPos;
        }
    }
}
