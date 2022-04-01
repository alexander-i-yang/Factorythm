using System;
using System.Linq;
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
    
    private BeatClipSO CurrentBeatClip
    {
        get { return PlayList[_index]; }
    }

    public float BPM => CurrentBeatClip.BPM;

    private int _index = 0;

    private Pooler _pooler;

    public bool RhythmLock = false;
    [NonSerialized] public AudioSource MusicSource;
    [NonSerialized] public UIManager MyUIManager;

    private BeatStateMachine _stateMachine;

    public int CurCombo { get; private set; }
    private int MaxCombo = 0;
    public bool ComboEnabled { get; private set; }
    private bool _wasOnBeat;

    public int Cash = 0;
    public FMOD.Studio.Bus MasterBus;

    private bool _paused;

    private AppearAfterNTicks[] _appearAfterNTicksArray;

    public Slide[] Slides { get; private set; }
    public int SlideIndex;
    public Slide CurSlide;

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

    }

    FMOD.Studio.EventInstance currentSong;
    // Start is called before the first frame update
    void Start() {
        // MusicSource.Play();
        FMODUnity.RuntimeManager.LoadBank("Master");
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        _appearAfterNTicksArray = FindObjectsOfType<AppearAfterNTicks>();
        foreach (var a in _appearAfterNTicksArray) {
            a.gameObject.SetActive(false);
        }

        Slides = FindObjectsOfType<Slide>();
        Slides = Slides.OrderBy(o => o.name).ToArray();
        foreach (var s in Slides) {
            s.gameObject.SetActive(false);
        }
        
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

        if (Input.GetMouseButtonDown(0)) {
            if (SlideIndex > Slides.Length-2) {
                // SceneManager.LoadScene("Milestone 2");
            } else {
                SlideIndex++;
                SetSlide(SlideIndex);
            }
        } else if (Input.GetMouseButtonDown(1)) {
            if (SlideIndex > 0) {
                SlideIndex--;
                SetSlide(SlideIndex);
            }
        }
    }

    void SetSlide(int ind) {
        CurSlide = Slides[ind];
        foreach (var s in Slides) {
            s.gameObject.SetActive(s == CurSlide);
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
        // MyUIManager.Tick();

        foreach (var a in _appearAfterNTicksArray) {
            a.Evaluate((int)BeatClipHelper.SongPositionInBeats);
        }

        var cons = FindObjectsOfType<SmoothSpritesController>();
        foreach (var con in cons) {
            con.Move();
        }
        var ss = FindObjectsOfType<SmoothRotate>();
        foreach (SmoothRotate r in ss) {
            r.Rotate();
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
            MyUIManager.CurLabel.text = "Combo: " + c;
            MaxCombo = (CurCombo > MaxCombo) ? CurCombo : MaxCombo;
            MyUIManager.MaxLabel.text = "Max Combo: " + MaxCombo;
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
