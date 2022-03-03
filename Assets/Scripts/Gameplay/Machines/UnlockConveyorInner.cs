using System;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class UnlockConveyorInner : Conveyor {
    public Criteria UnlockCriteria;
    public GameObject UnlockCounterObj;

    private UnlockCounter _unlockCounter;

    public void Start() {
        base.Start();
        _unlockCounter = UnlockCounterObj.GetComponent<UnlockCounter>();
        //TODO: automatic sprite setting
        // _unlockCounter.SetSprite(UnlockCriteria.resources[0].resource);
        _unlockCounter.Updatecounter(UnlockCriteria.resources[0].num);
    }

    public bool Done { get; private set; } = false;

    public override void OnDrag(PlayerController pc, Vector3 newPos) {
        if (Done) {
            base.OnDrag(pc, newPos);
        }
    }

    public override void OnInteract(PlayerController pc) {
        if (Done) {
            base.OnInteract(pc);
        }
    }

    protected override void MoveHere(Resource r, bool destroyOnComplete) {
        base.MoveHere(r, destroyOnComplete);
        if (!Done) {
            bool isDone = true;
            ResourceNum[] rns = UnlockCriteria.resources;
            for (var i = 0; i < rns.Length; i++) {
                if (rns[i].resource.ResourceName == r.ResourceName) {
                    rns[i].num--;
                    _unlockCounter.Updatecounter(rns[i].num);
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