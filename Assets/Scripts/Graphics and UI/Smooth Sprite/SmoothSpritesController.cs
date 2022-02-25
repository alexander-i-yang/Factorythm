using UnityEngine;

/// <summary>
/// <b>Legacy.</b> Controls multiple smoothSpriteLoops.
/// </summary>
public class SmoothSpritesController : MonoBehaviour {
    private SmoothSpriteLoop[] smoothSprites;

    void Awake() {
        smoothSprites = GetComponentsInChildren<SmoothSpriteLoop>();
    }

    public void Move() {
        foreach (var s in smoothSprites) {
            //print("s");
            s.Move();
        }
    }
}