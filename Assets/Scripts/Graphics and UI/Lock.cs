using UnityEngine;

public class Lock : MonoBehaviour {
    private Animator _myAnimator;
    private MachineSfx _lockSfx;

    void Awake() {
        _myAnimator = GetComponent<Animator>();
        _lockSfx = GetComponent<MachineSfx>();
    }

    public void Unlock() {
        _myAnimator.SetTrigger("Unlock");
    }

    public void PlayUnlockSFX() {
        _lockSfx.UnPause();
    }
}