using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ResourceNum {
    [SerializeField] public Resource resource;
    public int num;

    public List<Resource> toList() {
        return Enumerable.Repeat(resource,num).ToList();
    }

    public ResourceNum(Resource r, int n) {
        resource = r;
        num = n;
    }
}