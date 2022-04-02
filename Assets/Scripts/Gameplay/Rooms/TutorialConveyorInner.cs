using UnityEngine;

public class TutorialConveyorInner : MonoBehaviour {

    [SerializeField] private GameObject appear;
    [SerializeField] private GameObject appear2;
    [SerializeField] private GameObject appear3;
    [SerializeField] private GameObject disappear;
    [SerializeField] private GameObject disappear2;
    [SerializeField] private GameObject disappear3;

    /*protected virtual void Unlock() {
      appear.SetActive(true);
      disappear.SetActive(false);
      disappear2.SetActive(false);
      base.Unlock();
    }*/

    void Update() {
      RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        new Vector2(1, 0),
        1,
        LayerMask.GetMask("Room"));
      if (hit.transform == null) {
        appear.SetActive(true);
        appear2.SetActive(true);
        appear3.SetActive(true);
        disappear.SetActive(false);
        disappear2.SetActive(false);
        disappear3.SetActive(false);
      }
    }
    /*void OnCollisionEnter2D() {
      Debug.Log("hit");
    }

    void OnCollisionExit2D() {
      appear.SetActive(true);
      disappear.SetActive(false);
      disappear2.SetActive(false);
      disappear3.SetActive(false);
    }*/

}
