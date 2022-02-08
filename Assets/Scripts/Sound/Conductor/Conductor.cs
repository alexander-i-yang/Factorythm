using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pooler))]
public class Conductor : MonoBehaviour {
    public static Conductor Instance;
    public BeatClipHelper BeatClipHelper {get;} = new BeatClipHelper();
    [SerializeField] public BeatClipSO CurrentBeatClip;
    private Pooler _pooler;

    public bool RhythmLock = false;
    public int TickNum { get; private set; }

    [NonSerialized] public AudioSource MusicSource;
    [NonSerialized] public UIManager MyUIManager;

    private BeatStateMachine _stateMachine;

    public int CurCombo { get; private set; }
    public bool ComboEnabled { get; private set; }
    private bool _wasOnBeat;

    public int Cash = 0;

    FMOD.Studio.Bus MasterBus;



    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        BeatClipHelper.Reset(CurrentBeatClip);
        //DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = BeatClipHelper.BeatClip.MusicClip;
        //MusicSource.clip = Resources.Load<AudioClip>("Assets/Audio/BeatClips/Dyson Sphere.asset");

        MyUIManager = FindObjectOfType<UIManager>();
        _stateMachine = GetComponent<BeatStateMachine>();

        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
    }

    // Start is called before the first frame update
    void Start() {
        //MusicSource.Play();
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.PlayOneShot("event:/DysonSphereSong");

        _pooler = GetComponent<Pooler>();
        TickNum = 0;
    }

    public bool SongIsOnBeat() {
        return BeatClipHelper.IsOnBeat();
    }

    // Update is called once per frame
    void Update() {
        // UpdateSongPos();
        bool isNewBeat = UpdateSongPos();
        if (isNewBeat) {
            TrueTick();
            if (!RhythmLock) {
                MachineTick();
            }
        }
    }

    bool UpdateSongPos() {
        return BeatClipHelper.UpdateSongPos();
    }

    //Called whenever you want to update all machines
    public void MachineTick() {
        TickNum++;

        foreach(Machine m in _pooler.AllMachines) {
            if (m.gameObject.activeSelf)
            {
                m.PrepareTick();
            }
        }
        foreach (Machine machine in _pooler.AllMachines) {
            if (machine.gameObject.activeSelf)
            {
                if (machine.GetNumOutputMachines() == 0)
                {
                    machine.Tick();
                }
            }
        }
    }

    // Called whenever the song hits a new beat
    public void TrueTick() {
        MyUIManager.Tick();

        var cons = FindObjectsOfType<SmoothSpritesController>();
        foreach (var con in cons) {
            con.Move();
        }
    }

    public bool AttemptMove() {
        bool ret = _stateMachine.AttemptMove();
        return ret;
    }

    public void IncrCurCombo() {
        SetCurCombo(CurCombo+1);
    }

    public void SetCurCombo(int c) {
        if (ComboEnabled) {
            CurCombo = c;
            MyUIManager.Label.text = "Combo: " + c;
        }
    }

    public void DisableCombo() {
        ComboEnabled = false;
    }

    public void EnableCombo() {
        ComboEnabled = true;
    }

    public void Sell(Resource r) {
        Cash += r.price;
    }

    public static Pooler GetPooler() {
        return Conductor.Instance._pooler;
    }
}
