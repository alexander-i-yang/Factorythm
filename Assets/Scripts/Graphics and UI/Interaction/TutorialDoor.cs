using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : MonoBehaviour
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
        CheckPlayerOn();
    }
    public void CheckPlayerOn()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(0, 0, -1), 10.0f, LayerMask.GetMask("Player"));
        if (hit.transform != null) {
          TutorialLock.SetActive(false);
          //Destroy(TutorialLock);
          Destroy(_myGameObject);
        }
    }
}
