using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public static bool GameStarted = false;
    
    private Animator _pauseAnimator;
    
    void Awake() {
        _pauseAnimator = GetComponent<Animator>();
    }
    
    void Start() {
        Conductor.Instance.DisableCameraFollow();
        Conductor.Instance.RhythmLock = false;
        PauseMenu.isPaused = true;
    }

    public void StartGame() {
        Conductor.Instance.EnableCameraFollow();
        Conductor.Instance.RhythmLock = true;
        PauseMenu.isPaused = false;
        GameStarted = true;
        _pauseAnimator.Play("SlideBack",0);
    }

    public void ShowCredits() {
        StartCoroutine(SlideLeftOrRight(-1));
    }

    public void HideCredits() {
        StartCoroutine(SlideLeftOrRight(1));
    }

    IEnumerator SlideLeftOrRight(int direction) {
        var beforePos = transform.position;
        double moveBy = 20*direction;
        float duration = 0.333f;
        for (float i = 0; i<duration; i+=Time.deltaTime) {
            transform.position = new Vector3((float) (beforePos.x + moveBy*i/duration), beforePos.y, beforePos.z);
            yield return null;
        }
        transform.position = new Vector3((float) (beforePos.x + moveBy), beforePos.y, beforePos.z);
    }
}