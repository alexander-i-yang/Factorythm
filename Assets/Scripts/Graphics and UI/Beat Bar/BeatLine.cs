using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// The bar at the bottom with lines that move to the beat.
/// Not rigorously tested. Proceed with caution.
/// </summary>
public class BeatLine : MonoBehaviour {
    public BeatBar beatBar;
    private SpriteRenderer _mySR;

    void Start() {
        // Record when the line started moving:
        _mySR = GetComponent<SpriteRenderer>();
        StartCoroutine(Move());
    }

    IEnumerator Move() {
        float startX = beatBar.StartPos.x;
        float endX = beatBar.EndPos.x;
        float i = 0;
        bool fading = false;
        
        while (!fading) {
            // Determine the progress of the beat line's movement across the bar (t = 1 means destination reached)
            float t = (float)(i / ((startX - endX) / beatBar.GetVelocity()));
        
            // Stop and dissolve the beat line if the end is reached
            if (t > 1) {
                t = 1;
                fading = true;
                StartCoroutine(Helper.Fade(
                    _mySR,
                    beatBar.DissolveTime,
                    new Color(1, 1, 1, 0), 
                    e => Destroy(gameObject)
                ));

                // StartCoroutine(Fade(0.2f));
                // Destroy(gameObject);
            }

            // Determine x position of beat line on the bar based on its progress
            float newX = Helper.ActualLerp(startX, endX, t);
        
            // Set localPosition.x to newX so the beat line moves to its correct spot
            var localPosition = transform.localPosition;
            localPosition = new Vector3(newX, 0.032f, -1);
            transform.localPosition = localPosition;
            i += PauseMenu.isPaused ? 0 : Time.deltaTime;
            yield return null;
        }
    }
}