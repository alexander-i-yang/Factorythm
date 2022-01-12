using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Criteria {

    [Header("Resources")]
    [SerializeField] public ResourceNum[] resources;
    
    [Header("Cash")]
    public int Cash;
    
    public List<Resource> toList() {
        List<Resource> ret = new List<Resource>();
        foreach (ResourceNum rn in resources) {
            ret.AddRange(rn.toList());
        }
        return ret;
    }
}