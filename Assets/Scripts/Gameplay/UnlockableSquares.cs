using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableSquares : MonoBehaviour
{
    public bool isActive;
    // public GameObject square;
    private SpriteRenderer renderer;
    void Awake() 
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void Update() 
    {
        renderer.enabled = isActive;
    }
}
