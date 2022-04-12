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

    [SerializeField]
    private GameObject lockSpriteObj;
    private Lock _lockSprite;
    private ScreenShake ScreenShake;

    public Material LockMat;
    private Material _lockMat;
    public UnityEvent UnlockEvent;

    void Awake() {
        _mySR = lockSpriteObj.GetComponent<SpriteRenderer>();

        _lockMat = new Material(LockMat);
        _mySR.material = _lockMat;
        _lockMat.SetFloat("_T", 0);
        _lockMat.SetFloat("_L", (lockSpriteObj.transform.localScale.x - 1) / 2);
        _mySR.enabled = true;

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
        ScreenShake = GetComponent<ScreenShake>();
    }

    void Update() {
        // _mySR.enabled = isActive;
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
        //_mySR.enabled = false;

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
            ScreenShake.DelayedLargeShake();
        }

        UnlockEvent.Invoke();
        StartCoroutine(UnlockAnimation());
    }

    private IEnumerator UnlockAnimation()
    {
        float t = -1.15f;

        while (t < 1)
        {

            if (t >= 0)
            {
                _lockMat.SetFloat("_T", t);
                t += Time.deltaTime * Conductor.Instance.BPM / 60;
            } else
            {
                t += Time.deltaTime;
            }

            yield return null;
        }
        _mySR.enabled = false;
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
