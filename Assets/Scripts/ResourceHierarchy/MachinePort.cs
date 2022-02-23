using System;
using UnityEngine;

public abstract class MachinePort : MonoBehaviour, IComparable {
    public Machine ConnectedMachine;
    private EdgeCollider2D _myCollider;

    public int TicksSincePoked { get; private set; } = 0;

    protected void Start() {
        _myCollider = GetComponent<EdgeCollider2D>();
    }

    /*
     * TODO: Returns rotation in the global space relative to the center of its parent machine. 
     */
    public float GetRelativeRotation(Vector2 compare) {
        return Vector2.SignedAngle(transform.position, compare);
    }

    public void IncrTick() {
        TicksSincePoked++;
    }

    public void ResetTick() {
        TicksSincePoked = 0;
    }
    
    public int CompareTo(object obj) {
        if (obj == null) return 1;

        MachinePort other = obj as MachinePort;
        if (other != null) {
            int c = TicksSincePoked.CompareTo(other.TicksSincePoked);
            if (c == 0) {
                c = GetRelativeRotation(Vector2.up).CompareTo(other.GetRelativeRotation(Vector2.up));
            }
            return c;
        } else {
            throw new ArgumentException("Wrong obj is not a MachinePort");
        }
    }

}