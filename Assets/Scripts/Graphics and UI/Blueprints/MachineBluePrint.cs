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
            Conductor.checkForOverlappingMachines(transform.position);
            Destroy(gameObject);
            Conductor.GetPooler().InstantiateMachine(MachineCopy, p.transform.position);
        }
        else {
            Destroy(gameObject);
        }
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        SmoothSprite.Move(newPos);
        if (CanPlace(newPos)) {
            //SmoothSprite.SpriteRenderer.color = Color.green;
            SmoothSprite.SpriteRenderer.color = new Color(0.5f, 1.0f, 0.5f, 0.8f);
        } else {
            //SmoothSprite.SpriteRenderer.color = Color.red;
            SmoothSprite.SpriteRenderer.color = new Color(1.0f, 0.3f, 0.3f, 0.8f);
        }
    }

    public virtual bool CanPlace(Vector3 pos) {
        return true;
    }

    /// <summary>
    /// [March 12th, 2022] Update: Added a return value for this method if extension is needed for more than just
    /// head and stem tile tags.
    /// [March 15th, 2022] Moved CheckTileOn to MachineBluePrint and removed boolean flags.
    /// </summary>
    /// <returns> the Raycast's hit ray</returns>
    public RaycastHit2D CheckTileOn(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector3(pos.x, pos.y, 0),
            new Vector3(0, 0, -1),
            10.0f,
            LayerMask.GetMask("Default"));

        return hit;
    }
}