using System;
using UnityEngine;

public enum ResourceMatter {
    Solid,
    Liquid,
    None
}

[Serializable]
public class Resource : MonoBehaviour {
    public int price;
    public string ResourceName;
    public ResourceMatter matterState;
    public SmoothSprite MySmoothSprite { get; private set; }

    void Awake() {
        MySmoothSprite = GetComponentInChildren<SmoothSprite>();
    }
}