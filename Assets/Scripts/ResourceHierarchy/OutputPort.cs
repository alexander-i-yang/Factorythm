using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputPort : MonoBehaviour {
    public Machine OutputMachine;

    /*
     * Returns rotation in the global space relative to the center of its parent machine. 
     */
    public float GetRelativeRotation() {
        Transform parentTransform;
        Vector3 relative = (parentTransform = transform.parent.transform).InverseTransformPoint(transform.position);
        float relativeRot = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        float parentRot = parentTransform.rotation.eulerAngles.z;
        return 
    }
}
