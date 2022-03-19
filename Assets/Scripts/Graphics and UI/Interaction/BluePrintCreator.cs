using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BluePrintCreator : Button {
    [SerializeField] private GameObject machineBPInstance;
    private MachineBluePrint _myMachineBluePrint;

    private Vector3 _spawnPoint;

    private SpriteRenderer _icon;

    private Vector3 _iconUp;
    public Vector3 IconDown;

    public int cost;

    private void Start() {
        _spawnPoint = transform.Find("SpawnPoint").position;
        _icon = transform.Find("Icon").GetComponent<SpriteRenderer>();
        base.Start();
        _iconUp = _icon.transform.localPosition;
        _icon.sprite = machineBPInstance.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    public override void OnInteract(PlayerController p) {
      if (Conductor.Instance.Cash >= cost) {
        Conductor.Instance.Cash -= cost;
        if (_myMachineBluePrint == null) {
            _myMachineBluePrint = Conductor.GetPooler().CreateMachineBluePrint(machineBPInstance, _spawnPoint);
            _myMachineBluePrint.SmoothSprite.transform.position = transform.position;
            _myMachineBluePrint.SmoothSprite.Move(_spawnPoint);
        }
      }
      base.OnInteract(p);
      _icon.transform.localPosition = IconDown;
    }

    public override void OnDeInteract(PlayerController p) {
        base.OnDeInteract(p);
        _icon.transform.localPosition = _iconUp;
    }

    public void Unlock() {
        gameObject.SetActive(true);
    }
}
