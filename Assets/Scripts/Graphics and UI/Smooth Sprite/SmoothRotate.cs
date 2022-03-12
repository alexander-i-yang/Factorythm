using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class SmoothRotate : MonoBehaviour {
    public AnimationCurve movementCurve;
    public double moveTime = 0.1;
    public double degrees = 30;

    private Quaternion _beforeRot;
    private Quaternion _afterRot;

    private double position = 0;
    
    public void Alternate() {
        double rotateBy;
        if (position > 0) {
            rotateBy = -2*degrees;
        } else {
            rotateBy = 2*degrees;
        }
        Rotate(rotateBy);
        position += rotateBy;
    }

    public void Rotate() {
        Rotate(degrees);
    }

    public void Rotate(double deg) {
        _beforeRot = transform.rotation;
        transform.Rotate(new Vector3(0, 0, (float) deg));
        _afterRot = transform.rotation;
        transform.rotation = _beforeRot;
        StartCoroutine(_rotateCoroutine());
    }

    IEnumerator _rotateCoroutine() {
        for (float ft = 0; ft <= moveTime; ft += Time.deltaTime) {
            Quaternion newRot =
                Quaternion.LerpUnclamped(_beforeRot, _afterRot, movementCurve.Evaluate((float) (ft / moveTime)));
            transform.rotation = newRot;

            yield return null;
        }
    }

    #if UNITY_EDITOR
    public void OnDrawGizmos() {
        Handles.Label(transform.position, "" + position);
        Handles.Label(transform.position + new Vector3(0, 0.2f, 0), "" + (int)transform.rotation.z);
    }
    #endif
}