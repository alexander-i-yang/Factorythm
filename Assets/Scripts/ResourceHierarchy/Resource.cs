using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ResourceMatter {
    Solid,
    Liquid,
    None
}

public enum ResourceName {
    DEFAULT,
    HEAD,
    STEM,
    EIGHTH,
    QUARTER,
    HALF,
    CHRORD,
    PHRASE,
}

[Serializable]
public class Resource : MonoBehaviour, DictQueueElement<ResourceName> {
    public int price;
    public ResourceName Name;
    public ResourceMatter matterState;
    private Vector2 _dragDirection;
    public SmoothSprite MySmoothSprite { get; private set; }

    public void Awake() {
        MySmoothSprite = GetComponentInChildren<SmoothSprite>();
        if (Name == ResourceName.DEFAULT) {
            throw new ArgumentNullException("FACTORYTYM ERROR: give resource " + gameObject.name + " a type!");
        }
    }
    
    public ResourceName GetType() {
        return Name;
    }
}

public class ResourceDictQueue : DictQueue<ResourceName, Resource> {
    public override bool CompareKeys(ResourceName k1, ResourceName k2) {
        if (k1 == ResourceName.DEFAULT || k2 == ResourceName.DEFAULT) {
            return true;
        }
        else {
            return k1 == k2;
        }
    }
}