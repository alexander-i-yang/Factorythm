using System.Collections;
using FMOD.Studio;
using UnityEngine;

public class BusController : MonoBehaviour {
    private Bus _bus;
    private double _vol;

    public void Init(string path) {
        StartCoroutine(InitRoutine(path));
    }

    IEnumerator InitRoutine(string path) {
        while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded) {
            yield return null;
        }
        _bus = FMODUnity.RuntimeManager.GetBus(path);
        _bus.stopAllEvents(STOP_MODE.ALLOWFADEOUT);
    }

    public void IncrVolume() {
        SetVolume(Helper.Clamp(0, 1, _vol+0.1));
    }
    
    public void DecrVolume() {
        SetVolume(Helper.Clamp(0, 1, _vol+0.1));
    }

    public void SetVolume(double vol) {
        _vol = vol;
        _bus.setVolume((float) _vol);
    }
}