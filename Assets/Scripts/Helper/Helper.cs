using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Static helper methods. Add whatever you want!
/// </summary>
class Helper {
    public static float ActualLerp(float a, float b, float t) {
        return a + (b - a) * t;
    }

    public static Vector3 ActualLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3(ActualLerp(a.x, b.x, t), ActualLerp(a.y, b.y, t), ActualLerp(a.z, b.z, t));
    }
    
    //https://forum.unity.com/threads/debug-drawarrow.85980/
    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);
       
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Gizmos.DrawRay(pos + direction/2, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction/2, left * arrowHeadLength);
    }

    public static String DictToString<Key, Val>(Dictionary<Key, Val> d) {
        return "{" + string.Join(",", d.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
    }

    public static Vector2 RoundVector(Vector2 v) {
        return new Vector2((float) Math.Round(v.x), (float) Math.Round(v.y));
    }
}