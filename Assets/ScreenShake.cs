using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

public class ScreenShake : MonoBehaviour
{
    [Header("Small Screen Shake")]
    [SerializeField] private float positionStrength_s = 0.08f;
    [SerializeField] private float rotationStrength_s = 0.1f;
    [SerializeField] private float frequency = 25f;
    [SerializeField] private int numBounces = 5;
    [SerializeField] private float delay_s = 0f;

    [Header("Large Screen Shake")]
    [SerializeField] private float positionStrength_l = 0.8f;
    [SerializeField] private float rotationStrength_l = 0.5f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float delay_l = 0f;

    /// <summary>
    /// Shake the screen significantly. (Set the shake parameters in the ScreenShake component.)
    /// </summary>
    public void LargeShake()
    {
        CameraShaker.Presets.Explosion2D(positionStrength_l, rotationStrength_l, duration);
    }

    /// <summary>
    /// Shake the screen significantly after a set delay. (Set the delay length and shake parameters in the ScreenShake component.)
    /// </summary>
    public void DelayedLargeShake()
    {
        Invoke("LargeShake", delay_l);
    }

    /// <summary>
    /// Shake the screen a little. (Set the shake parameters in the ScreenShake component.)
    /// </summary>
    public void SmallShake()
    {
        CameraShaker.Presets.ShortShake2D(positionStrength_s, rotationStrength_s, frequency, numBounces);
    }

    /// <summary>
    /// Shake the screen a little after a set delay. (Set the delay length and shake parameters in the ScreenShake component.)
    /// </summary>
    public void DelayedSmallShake()
    {
        Invoke("SmallShake", delay_s);
    }
}
