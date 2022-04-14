using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public bool hidden = true;

    [SerializeField]
    private GameObject _pauseMenuUI;
    private Animator _pauseAnimator;
    private PlayerController _player;
    private Vector2 _pausedPosition;
    private TextMeshProUGUI _countDown;


    public static bool isPaused = false;

    private void Awake()
    {
        try
        {
            _pauseAnimator = _pauseMenuUI.GetComponent<Animator>();
        } 
        catch (System.Exception ex)
        {
            Debug.Log("No Animator attached to current Pause Menu");
        }
        
        _player = GameObject.FindObjectOfType<PlayerController>();
        _countDown = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start() { }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (isPaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    private void PauseGame () {
        FMODUnity.RuntimeManager.PauseAllEvents(true);
        //Time.timeScale = 0;
        Conductor.Instance.DisableCombo();
        Conductor.Instance.DisableCameraFollow();
        _pausedPosition = new Vector2(_player.transform.position.x, _player.transform.position.y);
        Conductor.Instance.MyUIManager.BeatBar.PauseBeatBar();

        if (_pauseAnimator != null)
        {
            _pauseAnimator.ResetTrigger("Pause");
            _pauseAnimator.SetTrigger("Pause");
        }
        
        isPaused = !isPaused;
        hidden = !hidden;
        gameObject.SetActive(true);
    }

    public void ResumeGame () {
        //Time.timeScale = 1;
        StartCoroutine(ResumeByStages());
    }

    private IEnumerator ResumeByStages()
    {
        _player.DisableActions();
        if (_pauseAnimator != null)
        {
            _pauseAnimator.ResetTrigger("UnPause");
            _pauseAnimator.SetTrigger("UnPause");
        }

        Conductor.Instance.EnableCombo();
        _countDown.text = "3";
        _player.MySS.Move(_pausedPosition, duration: 2f);

        yield return new WaitForSeconds(1f);

        _countDown.text = "2";
        
        yield return new WaitForSeconds(1f);

        _countDown.text = "1";
        _player.MySS.transform.localPosition = Vector2.zero;
        _player.transform.position = _pausedPosition;

        yield return new WaitForSeconds(1f);

        _countDown.text = "";
        FMODUnity.RuntimeManager.PauseAllEvents(false);
        isPaused = !isPaused;
        hidden = !hidden;
        Conductor.Instance.MyUIManager.BeatBar.UnPauseBeatBar();
        _player.EnableActions();
        Conductor.Instance.EnableCameraFollow();
    }
}