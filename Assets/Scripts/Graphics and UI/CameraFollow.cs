using System.Collections;
using UnityEngine;

/// <summary>
/// Script on the camera that makes it pan smoothly to its next position.
/// </summary>
public class CameraFollow : MonoBehaviour {
    public Transform Target;
    public AnimationCurve SmoothCurve;

    [SerializeField]
    private float _smoothSpeed = 40f;
    private Vector2 _velocity = Vector2.zero;

    private void Awake()
    {
        _smoothSpeed = 40f;
    }

    void Start() {
        // Follow = transform.parent.transform;    
    }

    private void LateUpdate() {
        Follow();
    } 

    public void TempFollow(Transform focus, float seconds)
    {
        StartCoroutine(TempFocus(focus, seconds));
    }

    private IEnumerator TempFocus(Transform focus, float seconds)
    {
        Transform tempTarget = Target;
        float tempSpeed = _smoothSpeed;

        _smoothSpeed = 150f;
        Target = focus;

        yield return new WaitForSeconds(seconds);

        Target = tempTarget;
        _smoothSpeed = tempSpeed;
    }

    private void Follow()
    {
        //Vector2 desiredPos = Target.position;
        //Vector2 smoothPos = Vector2.Lerp(transform.position, desiredPos, 0.1f);
        //transform.position = new Vector3(smoothPos.x, smoothPos.y, transform.position.z);

        Vector2 desiredPosition = Target.position;
        Vector2 smoothedPosition 
            = Vector2.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}