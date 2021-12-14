using System.Collections;
using UnityEngine;

public class BeatLine : MonoBehaviour {
    private float _moveStartTime;
    public BeatBar beatBar;
    private SpriteRenderer _mySR;

    void Start() {
        _moveStartTime = Time.time;
        _mySR = GetComponent<SpriteRenderer>();
    }

    void Update() {
        float startX = beatBar.StartPos.x;
        float endX = beatBar.EndPos.x;
        
        float t = (float) ((Time.time-_moveStartTime)/((startX-endX)/beatBar.GetVelocity()));
        if (t > 1) {
            Destroy(gameObject);
        }

        float newX = Helper.ActualLerp(startX, endX, t)-1;// Terrible practice, idk how to fix
        
        //Set localPos.x to newX
        var localPosition = transform.localPosition;
        localPosition = new Vector3(newX, localPosition.y, localPosition.z);
        transform.localPosition = localPosition;
    }

    IEnumerator Fade(float time) {
        for (float ft = 0; ft <= time; ft += Time.deltaTime) {
            Color col = _mySR.color;
            col.a = ft / time;
            _mySR.color = col;
            yield return null;
        }
        Destroy(gameObject);
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