using UnityEngine;

public class Joiner : Machine {
    protected override bool _checkEnoughInput(Recipe recipeObj) {
        bool ret = true;
        if (recipeObj == null) {
            print(name);
            print(transform.parent.name);
        }

        foreach (ResourceNum rn in GetInCriteria(recipeObj)) {
            if (_shouldPrint) {
                print(rn.resource.id + " " + rn.resource.GetID() + " " + rn.num);
                print(InputBuffer.ToList().Count);
                foreach (ResourceID k in InputBuffer._backer.Keys) {
                    print(k + " " + InputBuffer.CompareKeys(rn.resource.id, k) + " " + InputBuffer._backer[k].Count);
                }
            }

            if (!InputBuffer.HasEnough(rn.resource.id, 1)) {
                ret = false;
                break;
            }
        }

        return ret;
    }
}