using System;
using UnityEngine;

/// <summary>
/// A blueprint for head mines. Extends from MachineBluePrint.cs.
/// </summary>
public class HeadBluePrint : MachineBluePrint
{
    public override bool CanPlace(Vector3 pos) {
        RaycastHit2D hit = this.CheckTileOn(pos);
        if (hit.transform == null) {
            return false;
        } else {
            return hit.transform.gameObject.CompareTag("HeadTiles");
        }
    }
}