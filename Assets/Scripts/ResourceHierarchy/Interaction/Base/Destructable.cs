using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Destructable : MonoBehaviour
{
    //Called when the object is destroyed, but OnDestroy is a unity method, so it is called this stupid thing
    public abstract void OnDestruct();
}
