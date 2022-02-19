using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles normal destructions of objects
public class NormalDestroy : Destructable
{
    //Called when the object is destroyed
    public override void OnDestruct()
    {
        gameObject.GetComponent<Machine>().OnDestruction();
        gameObject.SetActive(false);
    }
}
