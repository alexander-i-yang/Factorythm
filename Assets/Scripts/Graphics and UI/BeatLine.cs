using System.Collections;
using UnityEngine;

public class BeatLine : MonoBehaviour {
    private float _moveStartTime;
    public BeatBar beatBar;

    void Start() {
        _moveStartTime = Time.time;
    }

    void Update() {
        float newX = Helper.ActualLerp(beatBar.StartPos.x, beatBar.EndPos.x, Time.time-_moveStartTime);
        
        //Set localPos.x to newX
        var localPosition = transform.localPosition;
        localPosition = new Vector3(newX, localPosition.y, localPosition.z);
        transform.localPosition = localPosition;
    }

    public void Move(Vector3 start, Vector3 end) {
        
    }

    /*public void Move(Vector3 newPos, bool destroyOnComplete = false) {
        _beforePosition = transform.position;
        _afterPosition = new Vector3(newPos.x, newPos.y, transform.position.z);
        StartCoroutine(_moveCoroutine(destroyOnComplete));
    }

    IEnumerator _moveCoroutine(bool destroyOnComplete) {
        for (float ft = 0; ft <= moveTime; ft += Time.deltaTime) {
            transform.position = Helper.ActualLerp(
                _beforePosition,
                _afterPosition,
                movementCurve.Evaluate((float) (ft / moveTime))
            );
            yield return null;
        }

        if (destroyOnComplete) {
            Destroy(transform.parent.gameObject);
        }

        transform.position = _afterPosition;
    }*/
}