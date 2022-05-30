using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialMineDetection : MonoBehaviour
{
  /*[SerializeField] private GameObject disappear;
  [SerializeField] private GameObject appear;
  [SerializeField] private GameObject appear2;
  [SerializeField] private GameObject disappear2;*/
  public UnityEvent done;
  /*[SerializeField] private GameObject disappear3;
  [SerializeField] private GameObject disappear4;*/

  void Update() {
      RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        new Vector2(0, 0),
        10.0f,
        LayerMask.GetMask("Interactable"));
      if (hit.transform == null) {
        done.Invoke();
        /*appear.SetActive(true);
        appear2.SetActive(true);
        disappear.SetActive(false);
        disappear2.SetActive(false);
        disappear3.SetActive(false);
        disappear4.SetActive(false);*/
      }
  }
}
