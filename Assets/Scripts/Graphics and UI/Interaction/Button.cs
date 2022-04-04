using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D), typeof(Animator))]
public class Button : Interactable {
    public UnityEvent OnPress;
    private Animator _myAnimator;

    public void Start() {
        _myAnimator = GetComponent<Animator>();
    }
    
    public override void OnInteract(PlayerController p) {
        OnPress.Invoke();
        _myAnimator.SetBool("IsUp", false);
        base.OnInteract(p);
        // _mySR.sprite = Interact;
    }

    public override void OnDeInteract(PlayerController p) {
        // GetComponent<SpriteRenderer>().color = Color.blue;
        // _mySR.sprite = DeInteract;
        _myAnimator.SetBool("IsUp", true);
        base.OnInteract(p);
    }
}