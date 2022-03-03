using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class Button : Interactable {
    public UnityEvent OnPress;
    private SpriteRenderer _mySR;

    public Sprite DeInteract;
    public Sprite Interact;

    public void Start() {
        _mySR = GetComponent<SpriteRenderer>();
    }
    
    public override void OnInteract(PlayerController p) {
        OnPress.Invoke();
        _mySR.sprite = Interact;
    }

    public override void OnDeInteract(PlayerController p) {
        // GetComponent<SpriteRenderer>().color = Color.blue;
        _mySR.sprite = DeInteract;
    }
}