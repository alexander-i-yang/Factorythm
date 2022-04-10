using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For use with a particle system component. Takes control of the particle
/// system and uses it to create smoke.
/// 
/// Requires a call to ThrowSmoke() to emit any bursts of smoke; this can be
/// done with an Animation Event to throw smoke on key frames.
/// </summary>
public class SetSmoke : MonoBehaviour
{
    [Header("Important")]
    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;
    [TextArea, Tooltip("Notes on how to use this script.")]
    public string Instructions = "1. Add a ParticleSystem component to this game object. No need to touch it.\n" +
        "2. Call ThrowSmoke() when a smoke emission is desired, such as in an Animation Event.";

    [Header("Particle System Modules")]
    // To access the particle system, module variables must be set to reference each module:
    private ParticleSystem.MainModule psMain;
    private ParticleSystem.EmissionModule psEmission;
    private ParticleSystem.ShapeModule psShape;
    private ParticleSystem.VelocityOverLifetimeModule psVelocity;
    private ParticleSystem.ForceOverLifetimeModule psForce;
    private ParticleSystem.NoiseModule psNoise;


    [Header("Smoke")]
    [SerializeField, Range(0, 50), Tooltip("Number of smoke particles per burst.")]
    private int numParticles = 5;
    [SerializeField, Range(0f, 0.5f), Tooltip("Size of each smoke particle.")]
    private float smokeSize = 0.0625f;
    [SerializeField, Tooltip("Color possibilities for each smoke particle.")]
    private Gradient colorRange = new Gradient
    {
        alphaKeys = new[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        },

        colorKeys = new[]
        {
            new GradientColorKey(Color.white, 0f),
            new GradientColorKey(new Color(150 / 255f, 150 / 255f, 150 / 255f), 0.50f),
            new GradientColorKey(new Color(50 / 255f, 50 / 255f, 50 / 255f), 0.5001f),
            new GradientColorKey(Color.black, 1f)
        }
    };
    [SerializeField, Range(0f, 10f), Tooltip("How long each smoke particle lasts.")]
    private float smokeLifetime = 1f;


    [Header("Smoke Movement")]
    [SerializeField, Tooltip("How fast the smoke spews sideways.")]
    private float smokeSpread = 2.0f;
    [SerializeField, Tooltip("How fast the smoke moves up.")]
    private float smokeSpeed = 0.8f;
    [SerializeField, Tooltip("How much the smoke stops moving up over its lifetime, in fraction of its initial speed.")]
    private float smokeLiftoffDecay = 0.75f;
    [SerializeField, Tooltip("How much the smoke wavers from side to side.")]
    private float smokeWaveringX = 0.15f;
    [SerializeField, Tooltip("How much the smoke wavers up and down.")]
    private float smokeWaveringY = 0.10f;
    [SerializeField, Tooltip("Apply a wind force on the smoke.")]
    private Vector2 wind = new Vector2(0f, 0f);


    [Header("Smoke Origin")]
    [SerializeField, Range(-1f, 1f), Tooltip("Horizontal position of the smoke origin relative to the GameObject.")]
    private float xPosition = 0f;
    [SerializeField, Range(-1f, 1f), Tooltip("Vertical position of the smoke origin relative to the GameObject.")]
    private float yPosition = 0.425f;
    private float zPosition = -0.01f;

    [SerializeField, Tooltip("Dimensions of the box from which smoke can spawn.")]
    private Vector2 originArea = new Vector2(0.08f, 0f);

    private void Start()
    {
        if (ps == null)
        {
            Debug.LogError("No particle system found for SetSmoke.");
            this.enabled = false;
        }
    }
    
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psRenderer = GetComponent<ParticleSystemRenderer>();

        if (ps == null)
        {
            Debug.LogError("No particle system found for SetSmoke.");
            return;
        }

        // To access the particle system, module variables must be set to reference each module.
        psMain = ps.main;
        psEmission = ps.emission;
        psShape = ps.shape;
        psVelocity = ps.velocityOverLifetime;
        psForce = ps.forceOverLifetime;
        psNoise = ps.noise;

        // Set all the smoke parameters
        psMain.startLifetime = smokeLifetime;
        psMain.startSpeed = 0f;

        psEmission.enabled = false;

        psShape.shapeType = ParticleSystemShapeType.Box;

        psVelocity.enabled = true;
        psVelocity.x = new ParticleSystem.MinMaxCurve(0, new AnimationCurve(), new AnimationCurve());
        psVelocity.y = new ParticleSystem.MinMaxCurve(0, new AnimationCurve(), new AnimationCurve());
        psVelocity.z = new ParticleSystem.MinMaxCurve(0, new AnimationCurve(), new AnimationCurve());
        
        psForce.enabled = true;

        psNoise.enabled = true;
        psNoise.separateAxes = true;
        psNoise.frequency = 0.5f;
        psNoise.scrollSpeed = 0.15f;
        psNoise.octaveCount = 2;

        psRenderer.material = Resources.Load<Material>("SmokeMaterial");

        // Send desired particle system values from this script to the particle system component.
        ModifyParticleSystem();
    }

    public void ThrowSmoke()
    {
        if (ps == null)
        {
            return;
        }

        #if UNITY_EDITOR
        // For testing during play.
        ModifyParticleSystem();
        #endif

        ps.Emit(numParticles);
    }

    private void ModifyParticleSystem()
    {
        // Change smoke size. If you want randomly sized smoke particles (why?), set it up as (smokeMinSize, smokeMaxSize).
        psMain.startSize = new ParticleSystem.MinMaxCurve(smokeSize);
        // Change smoke color.
        psMain.startColor = new ParticleSystem.MinMaxGradient
        {
            gradient = colorRange,
            mode = ParticleSystemGradientMode.RandomColor
        };
        // Change smoke lifetime.
        psMain.startLifetime = smokeLifetime;


        // Set up the smoke horizontal spread (a horizontal velocity curve)
        int frames = 50;

        Keyframe[] keyframes1 = new Keyframe[frames];
        Keyframe[] keyframes2 = new Keyframe[frames];

        for (int i = 0; i < frames; i++)
        {
            float progress = i / (frames - 1f);
            float value = Mathf.Exp(-20f * progress) - progress * Mathf.Exp(-20f); // A decay that always starts at 1 and ends at 0

            keyframes1[i] = new Keyframe(progress, value);
            keyframes2[i] = new Keyframe(progress, -value);
        }

        AnimationCurve lowerCurveX = new AnimationCurve(keyframes2);
        AnimationCurve upperCurveX = new AnimationCurve(keyframes1);
        psVelocity.x = new ParticleSystem.MinMaxCurve(smokeSpread, lowerCurveX, upperCurveX);


        // Set up the smoke vertical speed (vertical velocity range over time)
        frames = 20;

        Keyframe[] keyframes3 = new Keyframe[frames];
        Keyframe[] keyframes4 = new Keyframe[frames];

        for (int i = 0; i < frames; i++)
        {
            float progress = i / (frames - 1f);
            float value = 1 - smokeLiftoffDecay * progress;
            value = value < 0f ? 0f : value;

            keyframes3[i] = new Keyframe(progress, value);
            keyframes4[i] = new Keyframe(progress, 0.85f * value);
        }

        AnimationCurve lowerCurveY = new AnimationCurve(keyframes4);
        AnimationCurve upperCurveY = new AnimationCurve(keyframes3);
        psVelocity.y = new ParticleSystem.MinMaxCurve(smokeSpeed, lowerCurveY, upperCurveY);


        // Set up noise, sort of. If you want the smoke particles to waver differently, go modify the frequency, scroll speed, and damping over in the particle system component.
        if (smokeWaveringX == 0f && smokeWaveringY == 0f && psNoise.enabled)
        {
            psNoise.enabled = false;
        }
        else if (smokeWaveringX != 0f || smokeWaveringY != 0f)
        {
            psNoise.enabled = true;
        }
        psNoise.strengthX = new ParticleSystem.MinMaxCurve(-smokeWaveringX, smokeWaveringX);
        psNoise.strengthY = new ParticleSystem.MinMaxCurve(-smokeWaveringY, smokeWaveringY);


        // Apply wind.
        if (psForce.enabled)
        {
            psForce.x = wind.x;
            psForce.y = wind.y;
        }
        

        // Change position of smoke particle spawn area (relative to the game object), scale of smoke particle spawn area
        psShape.scale = new Vector3(originArea.x, originArea.y, 0);
        psShape.position = new Vector3(xPosition * transform.localScale.y, yPosition * transform.localScale.y, zPosition);


    }
}