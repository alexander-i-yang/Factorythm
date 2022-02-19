using UnityEngine;

/// <summary>
/// Script on the camera that makes it pan smoothly to its next position.
/// </summary>
public class CameraFollow : MonoBehaviour {
    public Transform Follow;
    public AnimationCurve SmoothCurve;
    
    void Start() {
        // Follow = transform.parent.transform;
    }

    private void LateUpdate() {
        Vector2 desiredPos = Follow.position;
        Vector2 smoothPos = Vector2.Lerp(transform.position, desiredPos, 0.1f);
        transform.position = new Vector3(smoothPos.x, smoothPos.y, transform.position.z);
    } 
}