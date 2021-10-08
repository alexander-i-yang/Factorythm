using System;
using UnityEngine;

[Serializable]
public class Resource : MonoBehaviour {
    public int price;
    public string ResourceName;
    public SmoothSprite MySmoothSprite { get; private set; }

    void Start() {
        MySmoothSprite = GetComponent<SmoothSprite>();
    }
}