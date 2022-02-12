using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If you want to make a new destructable method, make a new C# script and put Destructable as the base class
//Then just put it on your object and you're good to go
public abstract class Destructable : MonoBehaviour
{
    //Called when the object is destroyed
    public abstract void OnDestruct();
}
