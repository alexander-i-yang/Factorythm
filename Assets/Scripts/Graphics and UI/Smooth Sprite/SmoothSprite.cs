using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes a sprite move smoothly to a location.
/// You can define the animation curve in the inspector.
/// </summary>
public class SmoothSprite : MonoBehaviour
{
    
    public AnimationCurve movementCurve;
    public float moveTime = 0.1f;

    public SpriteRenderer SpriteRenderer { get; private set; }

    public bool IsRunning { get; private set; }

    void Start() {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Move(Vector3 newPos, bool destroyOnComplete = false, bool isLocalPos = false, float duration = 0f) {
        Vector3 beforePosition = isLocalPos ? transform.localPosition : transform.position;
        Vector3 afterPosition = new Vector3(newPos.x, newPos.y, transform.position.z);
        StartCoroutine(_moveCoroutine(beforePosition, afterPosition, destroyOnComplete, isLocalPos, duration <= 0 ? moveTime : duration));
    }
    
    IEnumerator _moveCoroutine(Vector3 beforePosition, Vector3 afterPosition, bool destroyOnComplete, bool isLocalPos, float duration) {
        IsRunning = true;
        for (float ft = 0; ft <= duration; ft += Time.deltaTime) {
            Vector3 newPos = Helper.ActualLerp(
                beforePosition, 
                afterPosition, 
                movementCurve.Evaluate((float) (ft/duration))
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

        transform.position = afterPosition;
        IsRunning = false;
    }
    
    public void BounceAnimate(Vector3 bounceFrom, Vector3 bounceTo) {
        bounceFrom = new Vector3(bounceFrom.x, bounceFrom.y, transform.position.z);
        bounceTo = new Vector3(bounceTo.x, bounceTo.y, transform.position.z);
        StartCoroutine(_bounceCoroutine(bounceFrom, bounceTo));
    }

    IEnumerator _bounceCoroutine(Vector3 bounceFrom, Vector3 bounceTo) {
        for (float ft = 0; ft <= moveTime/2; ft += Time.deltaTime) {
            Vector3 newPos = Helper.ActualLerp(
                bounceFrom, 
                bounceTo, 
                movementCurve.Evaluate((float) (ft/moveTime/2))
            );
            transform.position = newPos;

            yield return null;
        }
        for (float ft = 0; ft <= moveTime/2; ft += Time.deltaTime) {
            Vector3 newPos = Helper.ActualLerp(
                bounceTo, 
                bounceFrom, 
                movementCurve.Evaluate((float) (ft/moveTime/2))
            );
            transform.position = newPos;

            yield return null;
        }
    }
    
    
}
