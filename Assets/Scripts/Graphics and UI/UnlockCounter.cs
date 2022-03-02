using System;
using TMPro;
using UnityEngine;

public class UnlockCounter : MonoBehaviour {
    private SpriteRenderer _mySR;
    private TextMeshProUGUI _myText;
    
    public void Awake() {
        _mySR = GetComponentInChildren<SpriteRenderer>();
        _myText = GetComponent<TextMeshProUGUI>();
    }

    public void SetSprite(Resource r) {
        _mySR.sprite = r.MySmoothSprite.SpriteRenderer.sprite;
    }

    public void Updatecounter(int i) {
        if (_myText == null) {
            Awake();
        }
        print(GetComponent<TextMeshProUGUI>());
        _myText.SetText("x" + i);
    }
}