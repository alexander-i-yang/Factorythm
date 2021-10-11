using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

abstract public class Recipe : MonoBehaviour { 
	public ResourceNum[] InResources;
	public ResourceNum[] OutResources;

	public int ticks;
	public bool CreatesNew { get; private set; }

	public void Initiate() {
		
		CreatesNew = !InResources.SequenceEqual(OutResources);
	}

	public static List<Resource> toList(ResourceNum[] rns) {
		var ret = new List<Resource>();
		foreach (ResourceNum rn in rns) {
			ret.AddRange(rn.toList());
		}
		return ret;
	}

	public List<Resource> outToList() {
		return toList(OutResources);
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

	abstract protected bool _isInputValidR(Resource recipeResource, Resource compareAgainst);
	
	public bool CheckInputs(List<Resource> inputs) {
		var inputOccurences = occurenceDict(inputs);
		bool enoughInput = true;
        foreach (ResourceNum rn in InResources) {
            bool resourceInInput = false;
            foreach (var inputOccurence in inputOccurences) {
                if (inputOccurence.Value >= rn.num && _isInputValidR(inputOccurence.Key, rn.resource)) {
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
}

/*
 * Possible structure for later:
 * The machine reads each port's input and checks whether the input can be stored.
 *     The check is conducted by comparing the recipe's input and the port's output.
 *     If the port's output is valid, it is absorbed into the machine's storage.
 *     If the port's output is invalid, nothing happens to the port.
 * At the end of the grab phase, storage is transferred to output.
 */