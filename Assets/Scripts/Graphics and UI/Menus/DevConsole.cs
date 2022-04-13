using System;
using TMPro;
using UnityEngine;

public class DevConsole : MonoBehaviour {
    private bool _show = false;

    private TMP_InputField _input;

    public static readonly String[] COMMANDS = {"t", "unlock", "b"};
    public Action<string>[] COMMAND_ACTIONS = {ClearTutorial, UnlockRoom, RhythmLockToggle};

    void Awake() {
        SetShow(true); //We manually set everything inactive to make it easier to edit, undo this to find references
        _input = GetComponentInChildren<TMP_InputField>();
        SetShow(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Minus)) {
            SetShow(!_show);
        }

        if (_show) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                ProcessCommand(_input.text);
                _input.text = "";
                FocusField();
            }
        }
    }

    void ProcessCommand(String cmd) {
        var cmdArr = cmd.Split(' ');
        int ind = Array.IndexOf(COMMANDS, cmdArr[0]);
        try {
            COMMAND_ACTIONS[ind](cmd);
        } catch (Exception e) {
            Debug.LogException(e);
            print(ind);
        }
    }

    void SetShow(bool s) {
        _show = s;
        for (int i = 0; i < transform.childCount; ++i) {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(_show);
        }

        if (_show && _input) {
            FocusField();
        }
    }

    void FocusField() {
        _input.Select();
        _input.ActivateInputField();
    }

    static void ClearTutorial(string e) {
        var boundaries = GameObject.Find("Boundaries");
        if (boundaries) { boundaries.gameObject.SetActive(false);}

        var hq = GameObject.Find("HQ");
        if (hq) {
            for (int i = 0; i < hq.transform.childCount; ++i) {
                Transform child = hq.transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
        }
    }

    static void UnlockRoom(string cmd) {
        var rooms = GameObject.FindObjectsOfType<UnlockableSquares>();
        var cmdArr = cmd.Split(' ');
        if (cmdArr.Length == 1) {
            foreach (var room in rooms) {
                room.Unlock();
            }
        } else {
            int arg = Convert.ToInt32(cmdArr[1]);
            Array.Sort(
                rooms, 
                (a, b) => (int) (a.transform.position.y - b.transform.position.y)
            );
            rooms[arg].Unlock();
        }
    }

    static void RhythmLockToggle(string cmd) {
        Conductor.Instance.RhythmLock = !Conductor.Instance.RhythmLock;
    }
}