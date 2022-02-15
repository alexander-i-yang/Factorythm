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
        if (move) {
            Vector3 newPos = transform.position + (Vector3) velocity;
            _mySS.Move(newPos);
            transform.position = newPos;
        }
    }

    public override void OnInteract(PlayerController p) {
        print("Interact");
    }

    public override void OnDeInteract(PlayerController p) {
        print("De Interact");
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        print("Drag");
    }
}