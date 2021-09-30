using UnityEngine;

class MyMath {
    public static float ActualLerp(float a, float b, float t) {
        return a + (b - a) * t;
    }

    public static Vector3 ActualLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3(ActualLerp(a.x, b.x, t), ActualLerp(a.y, b.y, t), ActualLerp(a.z, b.z, t));
    }
}