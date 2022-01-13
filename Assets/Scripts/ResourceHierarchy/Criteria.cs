using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Criteria {
    [SerializeField] public ResourceNum[] resources;
    public int Cash;

    public List<Resource> toList() {
        List<Resource> ret = new List<Resource>();
        foreach (ResourceNum rn in resources) {
            ret.AddRange(rn.toList());
        }
        return ret;
    }
    
    public bool CheckInputs(List<Resource> inputs) {
        var inputOccurences = occurenceDict(inputs);
        bool enoughInput = true;
        foreach (ResourceNum rn in resources) {
            bool resourceInInput = false;
            foreach (var inputOccurence in inputOccurences) {
                Debug.Log(inputOccurence.Key + " " + rn.resource);
                Debug.Log(_isInputValidR(rn.resource, inputOccurence.Key));
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
        if (recipeResource.ResourceName == "null") {
            return recipeResource.ResourceName == compareAgainst.ResourceName;
        } else if (recipeResource.matterState == ResourceMatter.None) {
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