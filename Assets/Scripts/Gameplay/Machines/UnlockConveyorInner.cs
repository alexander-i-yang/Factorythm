using System;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class UnlockConveyorInner : Conveyor {
    public Criteria UnlockCriteria;

    public bool Done { get; private set; } = false;

    public override void OnDrag(PlayerController pc, Vector3 newPos) {
        return;
    }

    public override void OnInteract(PlayerController pc) {
        // print("press");
    }

    protected override void MoveHere(Resource r, bool destroyOnComplete) {
        base.MoveHere(r, destroyOnComplete);
        if (!Done) {
            bool isDone = true;
            for (var i = 0; i < UnlockCriteria.resources.Length; i++) {
                ResourceNum rn = UnlockCriteria.resources[i];
                if (rn.num > 0) {
                    isDone = false;
                }

                if (rn.resource.ResourceName == r.ResourceName) {
                    UnlockCriteria.resources[i].num--;
                }
            }

            Done = isDone;
        }
    }
}