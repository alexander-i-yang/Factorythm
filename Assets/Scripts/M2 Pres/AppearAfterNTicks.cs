using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TickEvent {
    public int Ticks;
    public UnityEvent Trigger;
    private bool _hasTriggered;
    
    public void Evaluate(int ticks) {
        if (ticks == Ticks && !_hasTriggered) {
            Trigger.Invoke();
            _hasTriggered = true;
        }
    }
}

public class AppearAfterNTicks : MonoBehaviour {
    public TickEvent[] TickEvents;

    public void Evaluate(int ticks) {
        foreach (var e in TickEvents) {
            e.Evaluate(ticks);
        }
    }
}