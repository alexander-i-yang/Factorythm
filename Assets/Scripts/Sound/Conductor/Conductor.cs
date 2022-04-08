using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// A singleton class that controls the state of the entire game. Kind of like the "main" function of the game.
/// If you need a global property (the current song, the _player's combo, etc.) it's probably in here.
/// </summary>
[RequireComponent(typeof(Pooler))]
public class Conductor : MonoBehaviour {
    public static Conductor Instance;
    public BeatClipHelper BeatClipHelper {get;} = new BeatClipHelper();

    public BeatClipSO[] PlayList;

    private PlayerController _player;

    private GameObject _mainCamera;
    
    private BeatClipSO CurrentBeatClip
    {
        get { return PlayList[_index]; }
    }

    public float BPM => CurrentBeatClip.BPM;

    private int _index = 0;

    private Pooler _pooler;

    public bool RhythmLock;
    public int TickNum { get; private set; }

    [NonSerialized] public AudioSource MusicSource;
    [NonSerialized] public UIManager MyUIManager;

    private BeatStateMachine _stateMachine;

    public int CurCombo { get; private set; }
    private int MaxCombo = 0;
    private bool _wasOnBeat;

    public int Cash = 0;
    public FMOD.Studio.Bus MasterBus;

    public int SpeedMultiplier = 1;

    private bool _paused;
    public bool Paused
    {
        get { return _paused; }
        set
        {
            if (value != _paused)
            {
                _paused = value;
                if (currentSong.isValid())
                {
                    if(currentSong.setPaused(_paused) != FMOD.RESULT.OK)
                    {
                        throw new Exception("Cannot toggle pause for some reason");
                    }
                }
            }
        }
    }

    public float currentSongProgress
    {
        get
        {
            int tMs = 0, tTMs = 1;
            currentSong.getTimelinePosition(out tMs);
            FMOD.Studio.EventDescription ed;
            currentSong.getDescription(out ed);
            ed.getLength(out tTMs);
            return (float) tMs / tTMs;
        }
    }

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        BeatClipHelper.Reset(CurrentBeatClip);

        // DontDestroyOnLoad(gameObject);
        // MusicSource = gameObject.AddComponent<AudioSource>();
        // MusicSource.clip = BeatClipHelper.BeatClip.MusicClip;

        MyUIManager = FindObjectOfType<UIManager>();
        _stateMachine = GetComponent<BeatStateMachine>();
        _mainCamera = FindObjectOfType<CameraFollow>().gameObject;
    }

    FMOD.Studio.EventInstance currentSong;
    // Start is called before the first frame update
    void Start() {
        // MusicSource.Play();
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");

        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //FMODUnity.RuntimeManager.PlayOneShot("event:/DysonSphereSong");
        _pooler = GetComponent<Pooler>();
        _player = FindObjectOfType<PlayerController>();
        StartCurrentClip();
    }

    private void StartClip(BeatClipSO bcs)
    {
        if (currentSong.isValid())
        {
            currentSong.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentSong.release();
        }

        TickNum = 0;
        BeatClipHelper.Reset(CurrentBeatClip);
        currentSong = FMODUnity.RuntimeManager.CreateInstance(bcs.fmodSongReference);
        currentSong.start();
    }

    private void StartCurrentClip() { StartClip(CurrentBeatClip); }

    public void SetProgress01(float t)
    {
        int tTMs = 1;
        FMOD.Studio.EventDescription ed;
        currentSong.getDescription(out ed);
        ed.getLength(out tTMs);
        currentSong.setTimelinePosition(Mathf.FloorToInt(t * tTMs));
    }

    private void NextSong()
    {
        _index = (_index + 1) % PlayList.Length;
        StartCurrentClip();
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

        FMOD.Studio.PLAYBACK_STATE playback;
        if (currentSong.isValid() && currentSong.getPlaybackState(out playback) == FMOD.RESULT.OK)
        {
            if (playback == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                // go to next song
                NextSong();
            }
        }
    }

    bool UpdateSongPos() {
        if (currentSong.isValid())
        {
            int timeMs = 0;
            if (currentSong.getTimelinePosition(out timeMs) == FMOD.RESULT.OK)
            {
                return BeatClipHelper.UpdateSongPos(timeMs / 1000f);
            }
        }
        return false;
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

        _player.Tick();
    }

    public bool AttemptMove() {
        bool ret = _stateMachine.AttemptMove();
        return ret;
    }

    public void IncrCurCombo() {
        if (CurCombo >= 12) {
            MyUIManager.Gauge.Overflow();
        } else {
            MyUIManager.Gauge.Incr(CurCombo);
        }
        SetCurCombo(CurCombo+1);
    }

    public void ResetCombo() {
        SetCurCombo(0);
        MyUIManager.Gauge.ResetGauge();
    }

    public void SetCurCombo(int c) {
        if (RhythmLock) {
            CurCombo = c;
            // MyUIManager.CurLabel.text = "Combo: " + c;
            MaxCombo = (CurCombo > MaxCombo) ? CurCombo : MaxCombo;
            // MyUIManager.MaxLabel.text = "Max Combo: " + MaxCombo;
        }
        
        if (0 <= CurCombo && CurCombo <= 3) {
            SpeedMultiplier = 1;
        } else if (4 <= CurCombo && CurCombo <= 7) {
            SpeedMultiplier = 2;
        } else if (8 <= CurCombo/* && CurCombo <= 12*/) {
            SpeedMultiplier = 4;
        }
    }

    public void DisableCombo() {
        print("Disable combo");
        RhythmLock = false;
    }

    public void EnableCombo() {
        RhythmLock = true;
    }

    public void Sell(Resource r) {
        Cash += r.price;
    }

    public void EnableCameraFollow()
    {
        _mainCamera.GetComponent<CameraFollow>().enabled = true;
    }

    public void DisableCameraFollow()
    {
        _mainCamera.GetComponent<CameraFollow>().enabled = false;
    }

    public static Pooler GetPooler() {
        return Conductor.Instance._pooler;
    }

    public static void checkForOverlappingMachines(Vector3 pos)
    {
        List<Machine> overlappingMachines = Helper.OnComponents<Machine>(pos);
        foreach (Machine machine in overlappingMachines)
        {
            try
            {
                machine.GetComponent<NormalDestroy>().OnDestruct();
                checkForOverlappingMachines(pos);
            }
            catch (NullReferenceException) {
                Debug.LogWarning("Normal Destroy not found");
            }
            
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Conductor))]
public class ConductorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Conductor c = (serializedObject.targetObject as Conductor);

        float csp = c.currentSongProgress;
        float f = EditorGUILayout.Slider(csp, 0, 1);
        if (Mathf.Abs(csp - f) > 0.05f)
        {
            c.SetProgress01(f);
        }

        if (GUILayout.Button("Toggle Pause"))
        {
            c.Paused = !c.Paused;
        }
    }
}
#endif