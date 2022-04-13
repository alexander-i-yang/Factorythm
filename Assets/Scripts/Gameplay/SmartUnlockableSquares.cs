using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LockedRoom))]
public class SmartUnlockableSquares : UnlockableSquares
{
    public override void Unlock() {
        base.Unlock();
        var lockedRooms = GetComponentsInChildren<LockedRoom>();
        foreach (var lockedRoom in lockedRooms) {
            if (lockedRoom.gameObject.name != gameObject.name) lockedRoom.gameObject.SetActive(false);
        }
    }
}
