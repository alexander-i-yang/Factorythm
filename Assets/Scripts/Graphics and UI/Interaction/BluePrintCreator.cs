using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BluePrintCreator : Button {
    [SerializeField] private GameObject machineBPInstance;
    private MachineBluePrint _myMachineBluePrint;

    private Vector3 _spawnPoint;

    private void Start() {
        _spawnPoint = transform.Find("SpawnPoint").position;
    }

    public override void OnInteract(PlayerController p) {
        if (_myMachineBluePrint == null) {
            _myMachineBluePrint = Conductor.GetPooler().CreateMachineBluePrint(machineBPInstance, _spawnPoint);
            _myMachineBluePrint.SmoothSprite.transform.position = transform.position;
            _myMachineBluePrint.SmoothSprite.Move(_spawnPoint);

            var origColor = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = new Color(origColor.r - 0.1f, origColor.g - 0.1f, origColor.b - 0.1f);
        }
    }

    public override void OnDeInteract(PlayerController p) {
        var origColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(origColor.r + 0.1f, origColor.g + 0.1f, origColor.b + 0.1f);
    }

    public void Unlock() {
        gameObject.SetActive(true);
    }
}