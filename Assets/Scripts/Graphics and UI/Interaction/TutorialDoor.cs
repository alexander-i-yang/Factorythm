using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : Interactable
{
    public GameObject TutorialLock;
    public GameObject _myGameObject;
    public bool getsDestroyed;

    void Start() {
      if (transform.position != new Vector3(0.5f, -4.5f, 3.0f)) {
        getsDestroyed = true;
      }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z) && getsDestroyed) {
          //_myGameObject.SetActive(false);
          Destroy(_myGameObject);
        }
    }

    public override void OnInteract(PlayerController p) {
        throw new System.NotImplementedException();
    }

    public override void OnDeInteract(PlayerController p) {
        throw new System.NotImplementedException();
    }
}
