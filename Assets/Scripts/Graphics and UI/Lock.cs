using UnityEngine;

public class Lock : MonoBehaviour {
    private Animator _myAnimator;

    void Awake() {
        _myAnimator = GetComponent<Animator>();
    }

    public void Unlock() {
        _myAnimator.SetTrigger("Unlock");
    }
}