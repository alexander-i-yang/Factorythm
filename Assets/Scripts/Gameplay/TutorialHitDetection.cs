using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHitDetection : MonoBehaviour
{
    [SerializeField] private GameObject disappear;
    [SerializeField] private GameObject appear;
    [SerializeField] private GameObject appear2;
    [SerializeField] private GameObject disappear2;
    [SerializeField] private GameObject disappear3;
    [SerializeField] private GameObject disappear4;
    public bool moved;

    void Update() {
        moved = true;
        RaycastHit2D hit = Physics2D.Raycast(
          transform.position,
          new Vector2(0, 0),
          10.0f,
          LayerMask.GetMask("Player"));
        if (hit.transform != null) {
          moved = false;
        }
        hit = Physics2D.Raycast(
          transform.position,
          new Vector2(1, 0),
          1.1f,
          LayerMask.GetMask("Player"));
        if (hit.transform != null) {
          moved = false;
        }
        hit = Physics2D.Raycast(
          transform.position,
          new Vector2(-1, 0),
          1.1f,
          LayerMask.GetMask("Player"));
        if (hit.transform != null) {
          moved = false;
        }
        hit = Physics2D.Raycast(
          transform.position,
          new Vector2(0, -1),
          1.1f,
          LayerMask.GetMask("Player"));
        if (hit.transform != null) {
          moved = false;
        }
        hit = Physics2D.Raycast(
          transform.position,
          new Vector2(0, 1),
          1.1f,
          LayerMask.GetMask("Player"));
        if (hit.transform != null) {
          moved = false;
        }
        if (moved) {
          appear.SetActive(true);
          appear2.SetActive(true);
          disappear.SetActive(false);
          disappear2.SetActive(false);
          disappear3.SetActive(false);
          disappear4.SetActive(false);
        }
    }
}
