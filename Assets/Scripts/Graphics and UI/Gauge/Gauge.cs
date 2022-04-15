using System.Collections;
using UnityEngine;

public class Gauge : MonoBehaviour {
    private SmoothRotate _needleRotate;
    private SetSmoke _particleEmitter;
    private SpriteRenderer _mySR;

    void Start() {
        _needleRotate = GetComponentInChildren<SmoothRotate>();
        _particleEmitter = GetComponent<SetSmoke>();
        _mySR = GetComponent<SpriteRenderer>();
    }

    public void Incr(int Combo) {
        if (0 <= Combo && Combo <= 3) {
            _needleRotate.Rotate(-15);
        }
        else if (4 <= Combo && Combo <= 8) {
            _needleRotate.Rotate(-30);
        }
        else if (8 <= Combo /*&& Combo <= 3*/) {
            _needleRotate.Rotate(-15);
        }
    }

    public void ResetGauge() {
        _needleRotate.RotateTo(56);
    }

    public void Overflow() {
        _needleRotate.BounceRotate(-10);
    }

    public void ThrowSmoke() {
        _particleEmitter.ThrowSmoke();
        StartCoroutine(RedShake());
        StartCoroutine(Helper.Shake(transform));
    }

    IEnumerator RedShake() {
        StartCoroutine(Helper.Fade(
            _mySR,
            0.2f,
            new Color(1, 0.5f, 0.5f)
        ));
        yield return new WaitForSeconds(0.2f + 0.5f);
        StartCoroutine(Helper.Fade(
            _mySR,
            0.2f,
            new Color(1, 1, 1)
        ));
    }

}