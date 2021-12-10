using System;
using TreeEditor;
using UnityEngine;

public class MachineBluePrint : Draggable {
    public GameObject MachineCopy;

    private SmoothSprite _smoothSprite;
    
    private void Start() {
        _smoothSprite = GetComponentInChildren<SmoothSprite>();
    }

    public override void OnInteract(PlayerController p) {
        print("Helo");
        Color c = _smoothSprite.SpriteRenderer.color;
        c.a = 0.8f;
        _smoothSprite.SpriteRenderer.color = c;
    }

    public override void OnDeInteract(PlayerController p) {
        Conductor.GetPooler().InstantiateMachine(MachineCopy, p.transform.position);
        Destroy(gameObject);
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        _smoothSprite.Move(newPos);
        // throw new System.NotImplementedException();
    }
}