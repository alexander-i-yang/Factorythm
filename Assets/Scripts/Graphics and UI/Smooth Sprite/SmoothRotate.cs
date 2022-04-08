using System.Collections;
using UnityEngine;

public class SmoothRotate : MonoBehaviour {
    public AnimationCurve movementCurve;
    public double MoveTime = 0.1;
    public double degrees = 30;

    private Quaternion _beforeRot;
    private Quaternion _afterRot;

    public void Rotate() {
        Rotate(degrees);
    }

    public void Rotate(double deg) {
        _beforeRot = transform.rotation;
        transform.Rotate(new Vector3(0, 0, (float) deg));
        ExecuteRotate();
    }

    public void ExecuteRotate() {
        _afterRot = transform.rotation;
        transform.rotation = _beforeRot;
        StartCoroutine(_rotateCoroutine());
    }

    public void RotateTo(Quaternion rotateTo) {
        _beforeRot = transform.rotation;
        transform.rotation = rotateTo;
        ExecuteRotate();
    }

    public void RotateTo(float toDegrees) {
        RotateTo(Quaternion.Euler(0, 0, toDegrees));
    }

    IEnumerator _rotateCoroutine() {
        for (float ft = 0; ft <= MoveTime; ft += Time.deltaTime) {
            Quaternion newRot =
                Quaternion.LerpUnclamped(_beforeRot, _afterRot, movementCurve.Evaluate((float) (ft / MoveTime)));
            transform.rotation = newRot;

            yield return null;
        }
    }
    
    public void BounceRotate(float deg) {
        Quaternion beforeRot = transform.rotation;
        transform.Rotate(new Vector3(0, 0, deg));
        Quaternion afterRot = transform.rotation;
        transform.rotation = beforeRot;
        StartCoroutine(_bounceCoroutine(afterRot, beforeRot));
    }

    IEnumerator _bounceCoroutine(Quaternion bounceTo, Quaternion bounceFrom) {
        double moveTime = MoveTime / 2;
        for (float ft = 0; ft <= moveTime; ft += Time.deltaTime) {
            Quaternion newRot = Quaternion.LerpUnclamped(
                bounceFrom,
                bounceTo, 
                (float) (ft/moveTime)
            );
            transform.rotation = newRot;
            yield return null;
        }

        for (float ft = 0; ft <= moveTime; ft += Time.deltaTime) {
            Quaternion newRot = Quaternion.LerpUnclamped(
                bounceTo,
                bounceFrom,
                (float) (ft / moveTime)
            );
            transform.rotation = newRot;
            yield return null;
        }

        transform.rotation = bounceFrom;
    }
}