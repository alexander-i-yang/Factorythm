using System.Linq;
using UnityEngine;

public class Conveyor : Machine {
    public GameObject copper;
    void Display() {
        foreach (var resource in storage) {
            Instantiate(resource);
        }
    }

    void Start() {
        base.Start();
    }
}