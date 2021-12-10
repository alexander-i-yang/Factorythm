using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothSprite : MonoBehaviour
{
    
    public AnimationCurve movementCurve;
    public double moveTime = 0.1;
    
    private float _timeAtMoved;
    private Vector3 _beforePosition;
    private Vector3 _afterPosition;

    public SpriteRenderer SpriteRenderer { get; private set; }

    // Start is called before the first frame update
    void Start() {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Move(Vector3 newPos, bool destroyOnComplete = false) {
        _timeAtMoved = Time.time;
        _beforePosition = transform.position;
        _afterPosition = new Vector3(newPos.x, newPos.y, transform.position.z);
        StartCoroutine(_moveCoroutine(destroyOnComplete));
    }
    
    IEnumerator _moveCoroutine(bool destroyOnComplete) {
        for (float ft = 0; ft <= moveTime; ft += Time.deltaTime) {
            transform.position = Helper.ActualLerp(
                _beforePosition, 
                _afterPosition, 
                movementCurve.Evaluate((float) (ft/moveTime))
                );
            yield return null;
        }

        if (destroyOnComplete) {
            Destroy(transform.parent.gameObject);
        }

        transform.position = _afterPosition;
    }
}
