using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Area {
    
    //Returns true if you can place the machine in this area.
    bool CanPlaceHere(Machine m);
}
