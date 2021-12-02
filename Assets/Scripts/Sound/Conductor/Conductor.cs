using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Conductor : MonoBehaviour {
    public static Conductor Instance;
    public BeatClip currentClip;

    public GameObject ConveyorBelt;
    public GameObject OutputPort;
    public GameObject InputPort;
    public int TickNum { get; private set; }

    [NonSerialized] private List<Machine> _allMachines;
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
        print(this);
        print(currentClip);
        currentClip.Init();
        DontDestroyOnLoad(gameObject);
        MusicSource = gameObject.AddComponent<AudioSource>();
        MusicSource.clip = currentClip.MusicClip;
        if (ConveyorBelt == null) {
            throw new Exception("Conveyor Belt gameobj reference set to null!");
        }

        MyUI = FindObjectOfType<UI>();
        _stateMachine = GetComponent<BeatStateMachine>();
    }

    // Start is called before the first frame update
    void Start() {
        MusicSource.Play();
        _allMachines = new List<Machine>(FindObjectsOfType<Machine>());
        TickNum = 0;
    }

    public bool StateIsOnBeat() {
        if (_stateMachine) return _stateMachine.IsOnState<OnBeatState>();
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
            // Tick();
        }
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

        // if (_stateMachine) Handles.Label(new Vector3(3, 2, -0.5f), (_stateMachine.CurState.GetType().ToString())+"A");
        Handles.Label(new Vector3(3.5f, 2, -0.5f), ("Cash: " + Cash));
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
}