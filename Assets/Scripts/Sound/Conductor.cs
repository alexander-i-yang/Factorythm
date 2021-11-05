using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Conductor : MonoBehaviour {
    private static Conductor _instance;
    public BeatClip currentClip;
    public Animator BeatStateAnimator { get; private set; }

    public GameObject ConveyorBelt;
    public GameObject OutputPort;
    public GameObject InputPort;
    public int TickNum { get; private set; }

    [NonSerialized] private List<Machine> _allMachines;
    [NonSerialized] public AudioSource MusicSource;
    [NonSerialized] public UI MyUI;

    private BeatStateMachine _stateMachine;
    
    public int CurCombo { get; private set; }
    private bool _wasOnBeat;

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        currentClip.Init();
        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = currentClip.MusicClip;
        if (ConveyorBelt == null) {
            throw new Exception("Conveyor Belt gameobj reference set to null!");
        }

        MyUI = FindObjectOfType<UI>();
        BeatStateAnimator = GetComponent<Animator>();
        _stateMachine = GetComponent<BeatStateMachine>();
    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.Play();
        _allMachines = new List<Machine>(FindObjectsOfType<Machine>());
        TickNum = 0;
    }

    public bool StateIsOnBeat() {
        if(BeatStateAnimator) return BeatStateAnimator.GetCurrentAnimatorStateInfo(0).IsName("On Beat");
        return false;
    }
    
    public bool SongIsOnBeat() {
        // return true;
        return (currentClip.TimeSinceBeat() < 0.1 || currentClip.TimeTilNextBeat() < 0.2);
    }

    // Update is called once per frame
    void Update() {
        bool isNewBeat = UpdateSongPos();
        if (isNewBeat) {
            Tick();
        }

        /*bool onBeat = SongIsOnBeat();
        bool transition = false;
        if (onBeat && _stateMachine.OnState<OffBeatState>()) {
            _stateMachine.Transition<OnBeatState>();
            transition = true;
        } else if (!onBeat && _stateMachine.OnState<OffBeatState>()) {
            _stateMachine.Transition<OffBeatState>();
            transition = true;
        }*/

        /*if (transition) {
            print("Transition");
            print(_stateMachine.CurState.GetType());
        }*/

        /*if (StateIsOnBeat() && !SongIsOnBeat()) {
            BeatStateAnimator.Play("Off Beat");
        } else if(!StateIsOnBeat() && SongIsOnBeat()) {
            BeatStateAnimator.Play("On Beat");
        }*/
    }

    bool UpdateSongPos() {
        return currentClip.UpdateSongPos();
    }

    public Machine InstantiateConveyor(Vector2 newPos, Quaternion direction) {
        Machine newConveyor = Instantiate(ConveyorBelt, new Vector3(newPos.x, newPos.y, ConveyorBelt.transform.position.z), direction).GetComponent<Machine>();
        _allMachines.Add(newConveyor);
        return newConveyor;
    }

    public OutputPort InstantiateOutputPort(Vector3 newPos, Transform parent) {
        return Instantiate(OutputPort, newPos, transform.rotation, parent).GetComponent<OutputPort>();
    }
    
    public InputPort InstantiateInputPort(Vector3 newPos, Transform parent) {
        return Instantiate(InputPort, newPos, transform.rotation, parent).GetComponent<InputPort>();
    }

    private void OnDrawGizmos() {
        Handles.Label(new Vector3(3, 3, -0.5f), (currentClip.TimeSinceBeat() < 0.15)+"");
        Handles.Label(new Vector3(3.5f, 3, -0.5f), (currentClip.TimeSinceBeat())+"");
        Gizmos.DrawWireCube(new Vector3(3, 3, -0.5f), new Vector3(currentClip.TimeSinceBeat()/currentClip.SecPerBeat, 1, -0.5f));
        
        Handles.Label(new Vector3(3, 4, -0.5f), (currentClip.TimeTilNextBeat() <0.3)+"");
        Handles.Label(new Vector3(3.5f, 4, -0.5f), (currentClip.TimeTilNextBeat())+"");
        Gizmos.DrawWireCube(new Vector3(3, 4, -0.5f), new Vector3(currentClip.TimeTilNextBeat()/currentClip.SecPerBeat, 1, -0.5f));

        if (_stateMachine) Handles.Label(new Vector3(3, 2, -0.5f), (_stateMachine.CurState.GetType().ToString())+"A");
    }

    public void Tick() {
        TickNum++;
        foreach(Machine m in _allMachines) {m.PrepareTick();}
        foreach (Machine machine in _allMachines) {
            if (machine.GetNumOutputMachines() == 0) {
                machine.Tick();
            }
        }
    }

    public bool AttemptMove() {
        bool onBeat = StateIsOnBeat();
        if (onBeat) {
            SetCurCombo(CurCombo+1);
            // _movedThisBeat = true;
        } else {
            SetCurCombo(0);
        }
        return onBeat;
    }

    public void SetCurCombo(int c) {
        CurCombo = c;
        MyUI.Label.text = "Combo: " + c;
        print("Set combo: " + c);
    }
}