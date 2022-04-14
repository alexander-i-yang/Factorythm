using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum ResourceMatter {
    Solid,
    Liquid,
    None
}

public enum ResourceID {
    DEFAULT,
    HEAD,
    STEM,
    EIGHTH,
    QUARTER,
    HALF,
    EIGHTH_CHRORD,
    QUARTER_CHRORD,
    HALF_CHRORD,
    PHRASE,
    PHRASE_GOAL,
}

[Serializable]
public class Resource : MonoBehaviour, DictQueueElement<ResourceID> {
    public int price;
    public ResourceID id;
    public ResourceMatter matterState;
    public SmoothSprite MySmoothSprite { get; private set; }

    public void Awake() {
        MySmoothSprite = GetComponentInChildren<SmoothSprite>();
        if (id == ResourceID.DEFAULT) {
            throw new ArgumentNullException("FACTORYTYM ERROR: give resource " + gameObject.name + " a type!");
        }
    }

    public ResourceID GetID() {
        return id;
    }

    public static bool CompareIDs(ResourceID k1, ResourceID k2) {
        if (k1 == ResourceID.DEFAULT || k2 == ResourceID.DEFAULT) {
            return true;
        }
        else {
            return k1 == k2;
        }
    }
}

public class ResourceDictQueue : DictQueue<ResourceID, Resource> {
    public override bool CompareKeys(ResourceID k1, ResourceID k2) {
        return Resource.CompareIDs(k1, k2);
    }
}
