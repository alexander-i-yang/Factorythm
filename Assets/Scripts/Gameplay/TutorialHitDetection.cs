using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHitDetection : MonoBehaviour
{
    [SerializeField] private GameObject[] disappears;
    [SerializeField] private GameObject[] appears;
    

    void Update() {
        //moved = true;
        //RaycastHit2D hit = Physics2D.Raycast(
        //  transform.position,
        //  new Vector2(0, 0),
        //  10.0f,
        //  LayerMask.GetMask("Player"));
        //if (hit.transform != null) {
        //  moved = false;
        //}
        //hit = Physics2D.Raycast(
        //  transform.position,
        //  new Vector2(1, 0),
        //  1.1f,
        //  LayerMask.GetMask("Player"));
        //if (hit.transform != null) {
        //  moved = false;
        //}
        //hit = Physics2D.Raycast(
        //  transform.position,
        //  new Vector2(-1, 0),
        //  1.1f,
        //  LayerMask.GetMask("Player"));
        //if (hit.transform != null) {
        //  moved = false;
        //}
        //hit = Physics2D.Raycast(
        //  transform.position,
        //  new Vector2(0, -1),
        //  1.1f,
        //  LayerMask.GetMask("Player"));
        //if (hit.transform != null) {
        //  moved = false;
        //}
        //hit = Physics2D.Raycast(
        //  transform.position,
        //  new Vector2(0, 1),
        //  1.1f,
        //  LayerMask.GetMask("Player"));
        //if (hit.transform != null) {
        //  moved = false;
        //}
    }

    public void movedOnBeat()
    {
        foreach (GameObject appear in appears)
        {
            appear.SetActive(true);
        }
        foreach (GameObject disappear in disappears)
        {
            disappear.SetActive(false);
        }
    }
}
