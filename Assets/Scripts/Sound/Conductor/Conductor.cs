using System;
using UnityEngine;

[RequireComponent(typeof(Pooler))]
public class Conductor : MonoBehaviour {
    public static Conductor Instance;
    public BeatClip currentClip;
    private Pooler _pooler;

    public bool RhythmLock = false;
    
    public int TickNum { get; private set; }

    [NonSerialized] public AudioSource MusicSource;
    [NonSerialized] public UI MyUI;

    private BeatStateMachine _stateMachine;
    
    public int CurCombo { get; private set; }
    public bool ComboEnabled { get; private set; }
    private bool _wasOnBeat;

    public int Cash = 0;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        currentClip.Init();
        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = currentClip.MusicClip;

        MyUI = FindObjectOfType<UI>();
        _stateMachine = GetComponent<BeatStateMachine>();
    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.Play();
        _pooler = GetComponent<Pooler>();
        TickNum = 0;
    }

    public bool SongIsOnBeat() {
        return currentClip.IsOnBeat();
    }

    // Update is called once per frame
    void Update() {
        UpdateSongPos();
        /*bool isNewBeat = UpdateSongPos();
        if (isNewBeat) {
            Tick();
        }*/
    }

    bool UpdateSongPos() {
        return currentClip.UpdateSongPos();
    }

    public void Tick() {
        TickNum++;
        foreach(Machine m in _pooler.AllMachines) {m.PrepareTick();}
        foreach (Machine machine in _pooler.AllMachines) {
            if (machine.GetNumOutputMachines() == 0) {
                machine.Tick();
            }
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
            MyUI.Label.text = "Combo: " + c;
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