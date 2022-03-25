using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Subclass of Machine.cs without a recipe.
/// Overrides MoveResourceIn() and Tick().
/// </summary>
public class PhraseMachine : Machine
{
    private static int _beatsSinceInput;
    public Phrase phrase;

    protected override void MoveResourcesIn() {
        foreachMachine(new List<MachinePort>(InputPorts), m => {
            if (_shouldPrint) {
                print("Checking: " + m.name + " ");
            }
            if (m.OutputBuffer.Count > 0) {
                Resource popped = m.OutputBuffer.Peek();
                if (popped.id != ResourceID.HEAD && popped.id != ResourceID.PHRASE) {
                    m.OutputBuffer.Dequeue();
                    InputBuffer.Add(popped);
                    if (_shouldPrint) {
                        print("Moving: " + popped.GetType() + " in");
                    }

                    MoveHere(popped, false);
                }
            }
        });
    }

    /// <summary>
    /// Completes one cycle of logic for phrase machine.
    /// </summary>
    public override void Tick() {
        if (!_isActive)
        {
            OnDestruction();
        }

        if (!_pokedThisTick) {
            _pokedThisTick = true;
            _beatsSinceInput++;
            MoveResourcesIn();
            if (_beatsSinceInput % 4 == 0 && InputBuffer._backer.Count > 0 && !NextMachineFull(phrase)) {
                OutputBuffer.Enqueue(CreatePhrase(InputBuffer));
                _ticksSinceProduced = 0;
                _beatsSinceInput = 0;
                InputBuffer.Clear();
            }
            else {
                _ticksSinceProduced++;
            }
            foreachMachine(new List<MachinePort>(InputPorts), m => m.Tick());
        }

        if (_shouldBreak) {
            Debug.Break();
        }
    }

    private Phrase CreatePhrase(ResourceDictQueue inputs) {
        Vector3 instantiatePos = transform.position;
        instantiatePos.z = -1.5f;
        Phrase instantiatePhrase = Instantiate(phrase, instantiatePos, Quaternion.identity);
        foreach (KeyValuePair<ResourceID, Queue<Resource>> entry in inputs._backer) {
            instantiatePhrase.Notes.Add(new ResourceNum(entry.Value.Peek(), entry.Value.Count));
        }
        return instantiatePhrase;
    }
}
