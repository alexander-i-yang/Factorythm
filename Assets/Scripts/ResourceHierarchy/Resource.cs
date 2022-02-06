using System;
using UnityEngine;

public enum ResourceMatter {
    Solid,
    Liquid,
    None
}

[Serializable]
public class Resource : Draggable {
    public int price;
    public string ResourceName;
    public ResourceMatter matterState;
    private Vector2 _dragDirection;
    public SmoothSprite MySmoothSprite { get; private set; }

    public override void OnDeInteract(PlayerController p)
    {
        Debug.Log("OnDeInteract has been called");
    }

    public override void OnDrag(PlayerController p, Vector3 newPos)
    {
        Debug.Log("I'm dragging copper");
        MySmoothSprite.Move(newPos);
    }

    public override void OnInteract(PlayerController p)
    {
        Debug.Log("You attemped to pick me up!");
        Color c = MySmoothSprite.SpriteRenderer.color;
        c.a = 0.8f;
        MySmoothSprite.SpriteRenderer.color = c;
    }

    void Awake() {
        MySmoothSprite = GetComponentInChildren<SmoothSprite>();
    }
    
    
}