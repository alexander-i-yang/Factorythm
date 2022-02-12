using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDestroy : Destructable
{
    public override void OnDestruct()
    {
        gameObject.SetActive(false);
    }
}
