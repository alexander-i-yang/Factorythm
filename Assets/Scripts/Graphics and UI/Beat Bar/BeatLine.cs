using System.Collections;
using UnityEngine;

/// <summary>
/// The bar at the bottom with lines that move to the beat.
/// Not rigorously tested. Proceed with caution.
/// </summary>
public class BeatLine : MonoBehaviour {
    private float _moveStartTime;
    public BeatBar beatBar;
    private SpriteRenderer _mySR;

    void Start() {
        // Record when the line started moving:
        _moveStartTime = Time.time;

        _mySR = GetComponent<SpriteRenderer>();
    }

    void Update() {
        float startX = beatBar.StartPos.x;
        float endX = beatBar.EndPos.x;

        // Determine the progress of the beat line's movement across the bar (t = 1 means destination reached)
        float t = (float)((Time.time - _moveStartTime) / ((startX - endX) / beatBar.GetVelocity()));

        // Stop and dissolve the beat line if the end is reached
        if (t > 1) {
            t = 1;
            StartCoroutine(Fade(beatBar.DissolveTime));
            // StartCoroutine(Fade(0.2f));
            // Destroy(gameObject);
        }

        // Determine x position of beat line on the bar based on its progress
        float newX = Helper.ActualLerp(startX, endX, t);
        
        // Set localPosition.x to newX so the beat line moves to its correct spot
        var localPosition = transform.localPosition;
        localPosition = new Vector3(newX, 0.032f, -1);
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