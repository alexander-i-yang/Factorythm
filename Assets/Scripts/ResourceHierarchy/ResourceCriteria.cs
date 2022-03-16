using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input or Output criteria for a recipe. Can specify cash, specific resources, or types of resources.
/// </summary>
[Serializable]
public struct ResourceCriteria {
    [SerializeField] public ResourceNum[] Resources;
    public int Cash;

    public List<Resource> ToList(Vector3 position) {
        List<Resource> ret = new List<Resource>();
        foreach (ResourceNum rn in Resources) {
            foreach (Resource r in rn.toList()) {
                var instantiatePos = new Vector3(position.x, position.y, r.transform.position.z);
                var newObj = Conductor.GetPooler().InstantiateResource(r, instantiatePos, Quaternion.identity);
                ret.Add(newObj);
            }
        }
        return ret;
    }

    public bool CheckInputs(List<Resource> inputs) {
        var inputOccurences = occurenceDict(inputs);
        bool enoughInput = true;
        foreach (ResourceNum rn in Resources) {
            bool resourceInInput = false;
            foreach (var inputOccurence in inputOccurences) {
                if (inputOccurence.Value >= rn.num && _isInputValidR(rn.resource, inputOccurence.Key)) {
                    resourceInInput = true;
                    break;
                }
            }
            if (!resourceInInput) {
                enoughInput = false;
                break;
            }
        }
        
        return enoughInput;
    }
    
    public bool _isInputValidR(Resource recipeResource, Resource compareAgainst) {
        // Compare in descending order of specificity
        if (recipeResource.Name != ResourceName.DEFAULT) {
            return recipeResource.Name == compareAgainst.Name;
        } else if (recipeResource.matterState != ResourceMatter.None) {
            return recipeResource.matterState == compareAgainst.matterState;
        } else {
            return true;
        }
    }
    
    //https://stackoverflow.com/questions/15862191/counting-the-number-of-times-a-value-appears-in-an-array
    public static Dictionary<Resource, int> occurenceDict(List<Resource> resources) {
        var ret = new Dictionary<Resource, int>();
        var g = resources.GroupBy( i => i );
        foreach (var grp in g) {
            ret[grp.Key] = grp.Count();
        }
        return ret;
    }
}