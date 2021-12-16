using System;
using UnityEngine;

public abstract class MachinePort : MonoBehaviour {
    public Machine ConnectedMachine;
    private EdgeCollider2D _myCollider;
    
    protected void Start() {
        _myCollider = GetComponent<EdgeCollider2D>();
    }

    /*
     * TODO: Returns rotation in the global space relative to the center of its parent machine. 
     */
    public float GetRelativeRotation() {
        throw new NotImplementedException();
        Transform parentTransform;
        Vector3 relative = (parentTransform = transform.parent.transform).InverseTransformPoint(transform.position);
        float relativeRot = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        float parentRot = parentTransform.rotation.eulerAngles.z;
        print(relativeRot + " " + parentRot);
        return relativeRot-parentRot;
    }
}