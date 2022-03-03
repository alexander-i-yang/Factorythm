using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TickEvent {
    public int Ticks;
    public UnityEvent Trigger;

    public void Evaluate(int ticks) {
        if (ticks == Ticks) {
            Trigger.Invoke();
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