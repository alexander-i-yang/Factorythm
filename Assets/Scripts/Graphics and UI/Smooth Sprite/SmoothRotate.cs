using System.Collections;
using TMPro;
using UnityEngine;

public class SmoothRotate : MonoBehaviour {
    public AnimationCurve movementCurve;
    public double moveTime = 0.1;
    public double degrees = 30;

    private Quaternion _beforeRot;
    private Quaternion _afterRot;

    public void Rotate() {
        print("AAA");
        _beforeRot = transform.rotation;
        transform.Rotate(new Vector3(0, 0, (float) degrees));
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

        // transform.rotation = _afterRot;
    }
}