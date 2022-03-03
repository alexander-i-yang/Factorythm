using System;
using UnityEngine;

/// <summary>
/// A blueprint for stem mines. Extends from MachineBluePrint.cs.
/// </summary>
public class StemBluePrint : MachineBluePrint
{
    public override bool CanPlace(Vector3 pos) {
        PlayerController p = (PlayerController) (GameObject.Find("Player").GetComponent(typeof(PlayerController)));
        return p.CanPlaceStemMine;
    }
}
