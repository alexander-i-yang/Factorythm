using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable {
    public UnityEvent OnPress;
    
    public override void OnInteract(PlayerController p) {
        OnPress.Invoke();
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public override void OnDeInteract(PlayerController p) {
        GetComponent<SpriteRenderer>().color = Color.blue;
    }
}