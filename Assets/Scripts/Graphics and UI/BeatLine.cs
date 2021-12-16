using System.Collections;
using UnityEngine;
// Not rigorously tested. Proceed with caution.
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
            t = 1;
            StartCoroutine(Fade(beatBar.DissolveTime));
            // StartCoroutine(Fade(0.2f));
            // Destroy(gameObject);
        }

        float newX = Helper.ActualLerp(startX, endX, t);
        
        //Set localPos.x to newX
        var localPosition = transform.localPosition;
        localPosition = new Vector3(newX, localPosition.y, localPosition.z);
        transform.localPosition = localPosition;
    }

    IEnumerator Fade(float time) {
        for (float ft = time; ft > 0; ft -= Time.deltaTime) {
            Color col = _mySR.color;
            col.a = ft / time;
            _mySR.color = col;
            yield return null;
        }
        Destroy(gameObject);
    }
}