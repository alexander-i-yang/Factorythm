using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Recipe : MonoBehaviour { 
	public ResourceNum[] InResources;
	public ResourceNum[] OutResources;

	public int ticks = 1;

	private void Start() {
		if (InResources == null) {
			InResources = new ResourceNum[0];
		}
		if (InResources == null) {
			OutResources = new ResourceNum[0];
		}
	}

	public static List<Resource> toList(ResourceNum[] rns) {
		var ret = new List<Resource>();
		foreach (ResourceNum rn in rns) {
			ret.AddRange(rn.toList());
		}
		return ret;
	}

	public List<Resource> inToList() {
		return toList(InResources);
	}
	
	public List<Resource> outToList() {
		return toList(OutResources);
	}
}
