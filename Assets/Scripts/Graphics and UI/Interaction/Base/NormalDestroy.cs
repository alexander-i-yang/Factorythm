using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

//Handles normal destructions of objects
public class NormalDestroy : Destructable {
    private MachineSfx _deleteSFX;
    
    void Awake() {
        var machineSfxs = GetComponents<MachineSfx>();
        foreach (var sfx in machineSfxs) {
            if (sfx.startCondition == MachineSfx.StartCondition.ON_DELETE) {
                _deleteSFX = sfx;
            }
        }
    }

    //Called when the object is destroyed
    public override void OnDestruct()
    {
        gameObject.GetComponent<Machine>().OnDestruction();
        gameObject.SetActive(false);
        print(_deleteSFX.exposed_src);
        print(_deleteSFX.startCondition);
        _deleteSFX.UnPause();
    }
}
