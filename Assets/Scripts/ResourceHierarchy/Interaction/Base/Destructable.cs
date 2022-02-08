using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    //Called when the object is destroyed, but OnDestroy is a unity method, so it is called this stupid thing
    public void OnDestruct()
    {
        gameObject.SetActive(false);
        Debug.Log(gameObject.name + "Destroyed");
    }
}
