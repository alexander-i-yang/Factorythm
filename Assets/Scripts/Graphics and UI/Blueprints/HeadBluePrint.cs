using System;
using UnityEngine;

/// <summary>
/// A blueprint for head mines. Extends from MachineBluePrint.cs.
/// </summary>
public class HeadBluePrint : MachineBluePrint
{
    public override bool CanPlace(Vector3 pos) {
        PlayerController p = (PlayerController) (GameObject.Find("Player").GetComponent(typeof(PlayerController)));
        return p.CanPlaceHeadMine;
    }
}
