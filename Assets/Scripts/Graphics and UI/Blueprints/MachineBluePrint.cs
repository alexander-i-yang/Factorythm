using System;
using UnityEngine;

/// <summary>
/// A semi-transparent draggable object that instantiates a new machine where it's dropped.
/// </summary>
public class MachineBluePrint : Draggable {
    public GameObject MachineCopy;

    public SmoothSprite SmoothSprite { get; private set; }

    private void Awake() {
        SmoothSprite = GetComponentInChildren<SmoothSprite>();
    }

    public override void OnInteract(PlayerController p) {
        Color c = SmoothSprite.SpriteRenderer.color;
        c.a = 0.8f;
        SmoothSprite.SpriteRenderer.color = c;
    }

    public override void OnDeInteract(PlayerController p) {
        if (CanPlace(SmoothSprite.transform.position)) {
            Destroy(gameObject);
            Conductor.GetPooler().InstantiateMachine(MachineCopy, p.transform.position);
        }
        else {
            Destroy(gameObject);
        }
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        SmoothSprite.Move(newPos);
        // throw new System.NotImplementedException();
    }

    public virtual bool CanPlace(Vector3 pos) {
        return true;
    }
}