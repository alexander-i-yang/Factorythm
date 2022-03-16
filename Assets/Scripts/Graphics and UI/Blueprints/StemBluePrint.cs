using System;
using UnityEngine;

/// <summary>
/// A blueprint for stem mines. Extends from MachineBluePrint.cs.
/// </summary>
public class StemBluePrint : MachineBluePrint
{
    public override bool CanPlace(Vector3 pos) {
        RaycastHit2D hit = CheckTileOn(pos);
        if (hit.transform == null) {
            return false;
        } else {
            return hit.transform.gameObject.CompareTag("StemTiles");
        }
    }
}
