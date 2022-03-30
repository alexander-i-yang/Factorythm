using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LockedRoom))]
public class UnlockableSquares : MonoBehaviour
{
    public bool isActive;
    // public GameObject square;
    private SpriteRenderer _mySR;
    private BoxCollider2D _myCollider;

    [NonSerialized] public Machine[] Machines;
    [NonSerialized] public UnlockConveyorInner[] ConveyorInners;

    private LockedRoom _lockedRoom;

    public GameObject[] MachineBluePrintCreators;
    private List<BluePrintCreator> _bluePrintCreators;
    private Canvas _canvas;

    private Lock _lockSprite;

    public UnityEvent UnlockEvent;

    void Awake() {
        _mySR = gameObject.GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<BoxCollider2D>();
        Machines = GetComponentsInChildren<Machine>();
        ConveyorInners = GetComponentsInChildren<UnlockConveyorInner>();
        _lockedRoom = GetComponent<LockedRoom>();
        _bluePrintCreators = new List<BluePrintCreator>();
        if (MachineBluePrintCreators != null) {
            foreach (var bpc in MachineBluePrintCreators) {
                _bluePrintCreators.Add(bpc.GetComponent<BluePrintCreator>());
            }
        }

        _canvas = GetComponentInChildren<Canvas>();
        _lockSprite = GetComponentInChildren<Lock>();
    }
    
    void Update() {
        _mySR.enabled = isActive;
        _myCollider.enabled = isActive;

        if (isActive) {
            bool done = CheckIfDone();
            if (done) {
                FindObjectOfType<CameraFollow>().TempFollow(transform, 5f);
                Unlock();
            }
        }
    }

    protected virtual void Unlock() {
        _lockedRoom.enabled = false;
        _mySR.enabled = false;

        foreach (var c in ConveyorInners) {
            c.Unlock();
        }

        isActive = false;
        if (_canvas != null) {
            _canvas.gameObject.SetActive(false);
        }

        if (_bluePrintCreators != null) {
            foreach (var bpc in _bluePrintCreators) {
                bpc.Unlock();
            }
        }

        if (_lockSprite) {
            _lockSprite.Unlock();
        }
        UnlockEvent.Invoke();
    }

    protected virtual bool CheckIfDone() {
        bool done = true;
            
        foreach (var c in ConveyorInners) {
            if (!c.Done) {
                done = false;
                break;
            }
        }

        return done;
    }
}
