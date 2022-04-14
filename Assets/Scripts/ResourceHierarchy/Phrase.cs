using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


[Serializable]
public class Phrase : Resource
{
    public bool isGoal;
    //public Phrase goal;
    //public GameObject thisPhrase;
    public List<ResourceNum> Notes;

    public void CheckGoal() {
      if (Notes.Count == 1 && Notes[0].resource.GetID() == ResourceID.HALF_CHRORD && Notes[0].num >= 12) {
        this.id = ResourceID.PHRASE_GOAL;
        isGoal = true;
      }
    }
}
