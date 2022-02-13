using System.Collections;
using UnityEngine;

/// <summary>
/// Makes a sprite move smoothly to a location.
/// You can define the animation curve in the inspector.
/// </summary>
public class SmoothSprite : MonoBehaviour
{
    
    public AnimationCurve movementCurve;
    public double moveTime = 0.1;
    
    private Vector3 _beforePosition;
    private Vector3 _afterPosition;

    public SpriteRenderer SpriteRenderer { get; private set; }

    void Start() {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Move(Vector3 newPos, bool destroyOnComplete = false, bool isLocalPos = false) {
        _beforePosition = isLocalPos ? transform.localPosition : transform.position;
        _afterPosition = new Vector3(newPos.x, newPos.y, transform.position.z);
        StartCoroutine(_moveCoroutine(destroyOnComplete, isLocalPos));
    }
    
    IEnumerator _moveCoroutine(bool destroyOnComplete, bool isLocalPos) {
        for (float ft = 0; ft <= moveTime; ft += Time.deltaTime) {
            Vector3 newPos = Helper.ActualLerp(
                _beforePosition, 
                _afterPosition, 
                movementCurve.Evaluate((float) (ft/moveTime))
            );
            if (isLocalPos) {
                transform.localPosition = newPos;
            } else {
                transform.position = newPos;
            }

            yield return null;
        }

        if (destroyOnComplete) {
            Destroy(transform.parent.gameObject);
        }

        transform.position = _afterPosition;
    }
}
