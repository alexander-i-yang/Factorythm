using System;
using System.Linq;
using UnityEngine;

public class UnlockPortMachine : Machine {
    public Criteria UnlockCriteria;

    public bool Done { get; private set; } = false;

    public override void OnDrag(PlayerController pc, Vector3 newPos) {
        return;
    }

    public override void OnInteract(PlayerController pc) {
        print("press");
    }

    protected override void _produce() {
        base._produce();
        print("done");
        if (!Done) {
            bool isDone = true;
            print("produce");
            foreach (Resource r in OutputBuffer) {
                for (var i = 0; i < UnlockCriteria.resources.Length; i++) {
                    ResourceNum rn = UnlockCriteria.resources[i];
                    if (rn.num > 0) {
                        isDone = false;
                    }

                    if (rn.resource.ResourceName == r.ResourceName) {
                        UnlockCriteria.resources[i].num--;
                    }
                }
            }

            if (isDone) {
                Done = isDone;
            }
        }
    }
}