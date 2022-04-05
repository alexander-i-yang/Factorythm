using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public bool hidden = true;
    public GameObject pauseMenuUI;



    public static bool isPaused = false;

    void Start() {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
          if (isPaused) {
            ResumeGame();
          } else {
            PauseGame();
          }
        }

    }

    private void PauseGame (){
          FMODUnity.RuntimeManager.PauseAllEvents(true);
          //Time.timeScale = 0;
          Conductor.Instance.DisableCombo();
          Conductor.Instance.DisableCameraFollow();
          pauseMenuUI.SetActive(true);
          isPaused = !isPaused;
          hidden = !hidden;
    }

    public void ResumeGame () {
      //Time.timeScale = 1;
      FMODUnity.RuntimeManager.PauseAllEvents(false);
      Conductor.Instance.EnableCombo();
        Conductor.Instance.EnableCameraFollow();
      pauseMenuUI.SetActive(false);
      isPaused = !isPaused;
      hidden = !hidden;
    }

}
