using System;
using TreeEditor;
using UnityEngine;

public class MachineBluePrint : Draggable {
    public Machine MachineCopy;

    private SmoothSprite _smoothSprite;
    
    private void Start() {
        _smoothSprite = GetComponentInChildren<SmoothSprite>();
    }

    public override void OnInteract(PlayerController p) {
        print("Helo");
        Color c = Color.white;
        c.a = 1f;
        _smoothSprite.SR.color = c;
    }

    public override void OnDeInteract(PlayerController p) {
        // throw new System.NotImplementedException();
    }

    public override void OnDrag(PlayerController p, Vector3 newPos) {
        
        // throw new System.NotImplementedException();
    }
}