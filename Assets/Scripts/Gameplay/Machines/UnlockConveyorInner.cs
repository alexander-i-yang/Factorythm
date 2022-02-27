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
            ResourceNum[] rns = UnlockCriteria.resources;
            for (var i = 0; i < rns.Length; i++) {
                if (rns[i].resource.ResourceName == r.ResourceName) {
                    rns[i].num--;
                }
                if (rns[i].num > 0) {
                    isDone = false;
                }
            }

            Done = isDone;
        }
    }

    public void Unlock() {
        recipeObj = Conductor.GetPooler().ConveyorRecipe;
    }
}