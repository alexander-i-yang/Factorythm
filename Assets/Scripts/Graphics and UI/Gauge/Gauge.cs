using UnityEngine;

public class Gauge : MonoBehaviour {
    private SmoothRotate _needleRotate;

    void Start() {
        _needleRotate = GetComponentInChildren<SmoothRotate>();
    }

    public void Incr(int Combo) {
        if (0 <= Combo && Combo <= 3) {
            _needleRotate.Rotate(-15);
        } else if (4 <= Combo && Combo <= 8) {
            _needleRotate.Rotate(-30);
        } else if (8 <= Combo /*&& Combo <= 3*/) {
            _needleRotate.Rotate(-15);
        }
    }

    public void ResetGauge() {
        _needleRotate.RotateTo(56);
    }

    public void Overflow() {
        _needleRotate.BounceRotate(-10);
    }
}