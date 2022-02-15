using UnityEngine;

public class InteractableSquare : Draggable {
    public bool move;
    public Vector2 velocity;
    private SmoothSprite _mySS;
    
    // Start is called before the first frame update
    void Start() {
        _mySS = GetComponentInChildren<SmoothSprite>();
    }

    public void Tick() {
        print(move);
        if (move) {
            Vector3 newPos = transform.position + (Vector3) velocity;
            _mySS.Move(newPos);
            transform.position = newPos;
        }
    }
    
    public override void OnInteract(PlayerController p) {
        _mySS.SpriteRenderer.color = Color.blue;
        move = false;
    }

    public override void OnDeInteract(PlayerController p) {
        move = true;
        _mySS.SpriteRenderer.color = Color.red;
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        velocity = newPos - transform.position;
        _mySS.Move(newPos);
        transform.position = newPos;
    }
}