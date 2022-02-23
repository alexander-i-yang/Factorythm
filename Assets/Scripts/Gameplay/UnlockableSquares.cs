using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableSquares : MonoBehaviour
{
    public bool isActive;
    // public GameObject square;
    private SpriteRenderer renderer;
    private BoxCollider2D _myCollider;

    public List<UnlockPortMachine> machines;

    void Awake() 
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<BoxCollider2D>();
    }
    
    void Update() 
    {
        renderer.enabled = isActive;
        _myCollider.enabled = isActive;
    }
}
