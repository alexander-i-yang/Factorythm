using FMOD.Studio;

public class BusController {
    private Bus _bus;
    private double _vol;
    
    public BusController(string path) {
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