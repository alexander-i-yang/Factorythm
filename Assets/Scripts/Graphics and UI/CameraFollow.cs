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

    private Vector3 shakeVector = Vector3.zero;
    private Vector3 previousShakeVector = Vector3.zero;

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
            = Vector2.SmoothDamp(transform.position - previousShakeVector, desiredPosition, ref _velocity, _smoothSpeed * Time.deltaTime);
        Vector3 newPosition = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z) + shakeVector;
        transform.position = newPosition;
    }



    /// <summary>
    /// Shakes the screen in according to the parameters given.
    /// </summary>
    /// <param name="severity">The distance of the first shake.</param>
    /// <param name="numShakes">The number of shakes that occur.</param>
    /// <param name="duration">The duration of the entire shaking.</param>
    /// <param name="direction">The direction of the first shake.</param>
    /// <param name="randomness">The tendency of the shakes to move randomly.</param>
    public void ShakeScreen(float severity, int numShakes, float duration, Vector3 direction, float randomness)
    {
        StartCoroutine(ScreenShake(severity, numShakes, duration, direction, randomness));
    }

    /// <summary>
    /// Shakes the screen a little in a random direction. (Uses 0.03f, 3, 0.2f, a random 2D vector, and 0.1f)
    /// </summary> 
    public void SmallShake()
    {
        StartCoroutine(ScreenShake(0.03f, 3, 0.2f, Random.insideUnitCircle.normalized, 0.1f));
    }

    /// <summary>
    /// Shakes the screen a little, starting in the direction specified. (Uses 0.05f, 3, 0.3f, your direction, and 0.1f)
    /// </summary> 
    public void SmallShake(Vector3 direction)
    {
        StartCoroutine(ScreenShake(0.05f, 3, 0.3f, direction, 0.1f));
    }

    readonly AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private IEnumerator ScreenShake(float severity, int numShakes, float duration, Vector3 direction, float randomness)
    {
        int shakeIndex = 0;
        float shakeProgress = 0f;
        float shakeDuration = duration / numShakes;

        Vector3 previousShakePath = Vector3.zero;
        Vector3 shakePath = direction.normalized * severity * Mathf.Pow((1 - (float)shakeIndex / numShakes), 2);

        while (shakeIndex < numShakes)
        {
            if (shakeProgress < 1f)
            {
                shakeProgress += Time.deltaTime / shakeDuration;

                previousShakeVector = shakeVector;
                shakeVector = (moveCurve.Evaluate(shakeProgress) * (shakePath - previousShakePath)) + previousShakePath;
            }
            else
            {
                shakeProgress = 0;

                previousShakeVector = shakeVector;
                shakeVector = shakePath;

                shakeIndex++;
                if (shakeIndex < numShakes)
                {
                    Vector3 randomAdjustmentVector = Random.insideUnitCircle.normalized * randomness;

                    previousShakePath = shakePath;
                    shakePath = (-shakePath.normalized + randomAdjustmentVector).normalized * severity * Mathf.Pow((1 - (float)shakeIndex/numShakes),2);
                }
            }

            yield return 0;
        }

        previousShakeVector = shakeVector;
        shakeVector = Vector3.zero;

        yield return 0;
        previousShakeVector = Vector3.zero;
    }

    // For testing purposes, this Update code calls regular shakes so you can try out different shakes.
    /*
    [Header("Screen Shake Parameters")]
    [SerializeField] private float period = 0.48f;
    [SerializeField] private float severity = 0.05f;
    [SerializeField] private int numShakes = 6;
    [SerializeField] private float duration = 0.2f;

    bool shook = false;
    public void Update()
    {
        if (Time.time % period < 0.15f && !shook)
        {
            shook = true;
            ShakeScreen(severity, numShakes, duration);
        }

        if (Time.time % period > period - 0.15f && shook)
        {
            shook = false;
        }
    }
    */
}