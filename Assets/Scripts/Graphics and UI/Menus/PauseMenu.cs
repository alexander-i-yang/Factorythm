using System.Collections;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public bool hidden = true;

    [SerializeField]
    private GameObject _pauseMenuUI;
    private Animator _pauseAnimator;
    private PlayerController _player;
    private Vector2 _pausedPosition;
    private TextMeshProUGUI _countDown;

    private bool _showingOptions = false;


    public static bool isPaused = false;
    private Vector3 _restPos;

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
        _restPos = _pauseMenuUI.transform.localPosition;
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

    public void ShowOptions() {
        // _pauseAnimator.ResetTrigger("Options");
        if (!_showingOptions) {
            _showingOptions = true;
            StartCoroutine(SlideLeftOrRight(-1));
        }
    }

    public void HideOptions() {
        if (_showingOptions) {
            _showingOptions = false;
            StartCoroutine(SlideLeftOrRight(1));
        }
    }

    public bool SurpassesPauseMenu(Vector2 pos)
    {
        return _pausedPosition.x + 9 < pos.x
        || _pausedPosition.x - 9 > pos.x
        || _pausedPosition.y + 5 < pos.y
        || _pausedPosition.y - 5 > pos.y;
    }

    public void ResumeGame () {
        //Time.timeScale = 1;
        _showingOptions = false;
        StartCoroutine(ResumeByStages());
    }

    IEnumerator SlideLeftOrRight(int direction) {
        var beforePos = _pauseMenuUI.transform.position;
        double moveBy = 20*direction;
        float duration = 0.333f;
        for (float i = 0; i<duration; i+=Time.deltaTime) {
            _pauseMenuUI.transform.position = new Vector3((float) (beforePos.x + moveBy*i/duration), beforePos.y, beforePos.z);
            yield return null;
        }
        _pauseMenuUI.transform.position = new Vector3((float) (beforePos.x + moveBy), beforePos.y, beforePos.z);
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
        _player.MySS.Move(_pausedPosition, duration: 0.5f, curved: false);

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

        _pauseMenuUI.transform.localPosition = _restPos;
    }
}