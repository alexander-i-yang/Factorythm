using UnityEngine;

public class UnlockConveyorInner : Conveyor {
    public ResourceCriteria UnlockCriteria;
    public GameObject UnlockCounterObj;

    private UnlockCounter _unlockCounter;
    private bool _unlocked = false;

    public void Awake() {
        base.Awake();
    }

    public void Start() {
        base.Start();
        _unlockCounter = UnlockCounterObj.GetComponent<UnlockCounter>();
        //TODO: automatic sprite setting
        // _unlockCounter.SetSprite(UnlockCriteria.resources[0].resource);
        _unlockCounter.Updatecounter(UnlockCriteria.Resources[0].num);
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

    protected override ResourceNum[] GetInResources(Recipe recipeObj) {
        return _unlocked ? base.GetInResources(recipeObj) : UnlockCriteria.Resources;
    }

    protected override void MoveHere(Resource r, bool destroyOnComplete) {
        base.MoveHere(r, destroyOnComplete);
        if (!Done) {
            bool isDone = true;
            ResourceNum[] rns = GetInResources(recipes[0]);
            for (var i = 0; i < rns.Length; i++) {
                if (rns[i].resource.Name == r.Name) {
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
        recipes[0] = Conductor.GetPooler().ConveyorRecipe;
        _unlocked = true;
    }
}