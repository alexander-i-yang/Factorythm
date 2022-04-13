using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SliderController : MonoBehaviour {
    public string FormatString = "Volume: {0}";
    
    public int Min = 0;
    public int Max = 10;
    private int _cur ;

    private BusController _busController;

    public BusControllerName BusName;
    
    public enum BusControllerName {
        MUSIC,
        SFX,
    }

    public int Cur {
        get { return _cur; }
        private set {
            _cur = (int) Helper.Clamp(Min, Max, value);
            _text.text = String.Format(FormatString, Cur);
            _busController.SetVolume(_cur/10.0);
        }
    }

    private TextMeshProUGUI _text;
    
    public void Start() {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        if (BusName == BusControllerName.MUSIC) {
            _busController = Conductor.Instance.MusicBus;
        } else if (BusName == BusControllerName.SFX) {
            _busController = Conductor.Instance.SFXBus;
        }
        Cur = 10;
    }

    public void Incr() {
        Cur++;
    }
    
    public void Decr() {
        Cur--;
    }
}