using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

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
    public ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;
    [SerializeField(), Tooltip("Choose whether to set up the particle system for smoke bursts. Sets start speed to 0, disables emission, and adds the smoke material if there is not one already.")]
    public bool initializeParticleSystem = true;


    [Header("Particle System Modules")]
    // To access the particle system, module variables must be set to reference each module:
    private ParticleSystem.MainModule psMain;
    private ParticleSystem.EmissionModule psEmission;
    private ParticleSystem.ShapeModule psShape;
    private ParticleSystem.VelocityOverLifetimeModule psVelocity;
    private ParticleSystem.ForceOverLifetimeModule psForce;
    private ParticleSystem.NoiseModule psNoise;


    [Header("Smoke")]
    [SerializeField, Tooltip("If disabled, this script will not seize control of the main module.")]
    private bool controlSmokeAppearance = true;
    [SerializeField, Range(0, 50), Tooltip("Number of smoke particles per burst.")]
    private int numParticles = 5;
    [SerializeField, Range(0f, 0.5f), Tooltip("Size of each smoke particle.")]
    private float smokeSize = 0.0625f;
    [SerializeField, Tooltip("Color possibilities for each smoke particle.")]
    private Gradient colorRange = new Gradient
    {
        alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(150 / 255f, 150 / 255f, 150 / 255f), 0.50f), new GradientColorKey(new Color(50 / 255f, 50 / 255f, 50 / 255f), 0.5001f), new GradientColorKey(Color.black, 1f) }
    };
    [SerializeField, Range(0f, 10f), Tooltip("How long each smoke particle lasts.")]
    private float smokeLifetime = 1f;


    [Header("Smoke Movement")]
    [SerializeField, Tooltip("If disabled, this script will not seize control of the Velocity Over Lifetime module, Noise module, and Force modules.")]
    private bool controlSmokeMovement = true;
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
    [SerializeField, Tooltip("If disabled, this script will not seize control of the Shape module.")]
    private bool controlSmokeOrigin = true;
    [SerializeField, Range(-1f, 1f), Tooltip("Horizontal position of the smoke origin relative to the GameObject.")]
    private float xPosition = 0f;
    [SerializeField, Range(-1f, 1f), Tooltip("Vertical position of the smoke origin relative to the GameObject.")]
    private float yPosition = 0.425f;
    private float zPosition = -0.01f;
    [SerializeField, Tooltip("Dimensions of the box from which smoke can spawn.")]
    private Vector2 originArea = new Vector2(0.08f, 0f);


    [HideInInspector]
    public int eventFrame = 0;


    void Awake()
    {
        bool hadToAdd = false;

        if (ps == null)
        {
            // Get the particle system component.
            ps = gameObject.GetComponent<ParticleSystem>();

            // Add a particle system component to the Game Object if one is not already assigned.
            if (ps == null)
            {
                ps = gameObject.AddComponent<ParticleSystem>();
                hadToAdd = true;
            }

            // Freak out if there is still no particle system component.
            if (ps == null)
            {
                Debug.LogError("No particle system found for SetSmoke.");
                return;
            }
        }

        // Set up references to and configure the particle system component as necessary.
        InitializeParticleSystem(initializeParticleSystem || hadToAdd);
        
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
        if (controlSmokeAppearance)
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
        }

        if (controlSmokeMovement)
        {
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


            // Set up a 0 smoke z speed to be of the same type as the others.
            psVelocity.z = new ParticleSystem.MinMaxCurve(0, new AnimationCurve(), new AnimationCurve());


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
        }

        if (controlSmokeOrigin)
        {
            // Change position of smoke particle spawn area (relative to the game object), scale of smoke particle spawn area
            psShape.position = new Vector3(xPosition * transform.localScale.y, yPosition * transform.localScale.y, zPosition);
            psShape.scale = new Vector3(originArea.x, originArea.y, 0);
        }
    }

    private void InitializeParticleSystem(bool setupRequired)
    {
        // To access the particle system, module variables must be set to reference each module.
        // The main module is assigned here and not in a conditional because it is used in multiple setting categories.
        psMain = ps.main;

        if (setupRequired)
        {
            // Remove start speed
            psMain.startSpeed = 0f;

            // Disable emission
            psEmission = ps.emission;
            psEmission.enabled = false;

            // Get the particle system renderer component, which is not actually part of the particle system component.
            psRenderer = GetComponent<ParticleSystemRenderer>();
            if (true)
            {
                psRenderer.material = Resources.Load<Material>("SmokeMaterial");
            }
        }

        if (controlSmokeMovement)
        {
            psVelocity = ps.velocityOverLifetime;
            psVelocity.enabled = true;

            psNoise = ps.noise;
            psNoise.enabled = true;
            psNoise.separateAxes = true;
            psNoise.frequency = 0.5f;
            psNoise.scrollSpeed = 0.15f;
            psNoise.octaveCount = 2;
            psNoise.strengthX = new ParticleSystem.MinMaxCurve(0, 0);
            psNoise.strengthY = new ParticleSystem.MinMaxCurve(0, 0);
            psNoise.strengthZ = new ParticleSystem.MinMaxCurve(0, 0);

            psForce = ps.forceOverLifetime;
            psForce.enabled = true;
        }

        if (controlSmokeOrigin)
        {
            psShape = ps.shape;
            psShape.position = new Vector3(0, 0, zPosition);
            psShape.shapeType = ParticleSystemShapeType.Box;
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SetSmoke))]
public class SetSmokeEditor : Editor
{
    SetSmoke t;
    SerializedObject GetTarget;

    AnimationClip clip;
    AnimationEvent evt = new AnimationEvent();
    SerializedProperty eventFrame;

    void OnEnable()
    {
        t = (SetSmoke)target;
        GetTarget = new SerializedObject(t);

        clip = AnimationUtility.GetAnimationClips(t.gameObject)[0];
        evt.functionName = "ThrowSmoke";
        eventFrame = GetTarget.FindProperty("eventFrame");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
        EditorGUILayout.TextArea("Call ThrowSmoke() as an Animation Event through the Animation tab or the menu below. Remember to delete the event if you remove this component.\n", EditorStyles.textArea);

        base.OnInspectorGUI();

        GetTarget.Update();

        ///////////////////////////////////////////////
        if (clip != null)
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Animation Event", EditorStyles.boldLabel);

            eventFrame.intValue = EditorGUILayout.IntSlider("ThrowSmoke() Event Frame", eventFrame.intValue, 0, (int)(clip.length * clip.frameRate));

            // Set the only ThrowSmoke() event to the desired frame, leaving other animation events alone.
            if (GUILayout.Button("Set ThrowSmoke() Animation Event To This Frame"))
            {
                evt.time = eventFrame.intValue / clip.frameRate;

                // Get the animation events and count how many aren't ThrowSmoke(), which is probably zero but just in case
                AnimationEvent[] events = AnimationUtility.GetAnimationEvents(clip);
                int numEventsToLeave = 0;

                for (int i = 0; i < events.Length; i++)
                {
                    if (events[i].functionName != "ThrowSmoke")
                    {
                        numEventsToLeave++;
                    }
                }

                // Add all the non-ThrowSmoke() events into another array
                AnimationEvent[] eventsToLeave = new AnimationEvent[numEventsToLeave + 1];

                int ind = 0;
                for (int i = 0; i < events.Length; i++)
                {
                    if (events[i].functionName != "ThrowSmoke")
                    {
                        eventsToLeave[ind] = events[i];
                        ind++;
                    }
                }

                // ... But add one more event, the new ThrowSmoke() event.
                eventsToLeave[ind] = evt;

                AnimationUtility.SetAnimationEvents(clip, eventsToLeave);
            }
        }
        ///////////////////////////////////////////////

        GetTarget.ApplyModifiedProperties();
    }
}
#endif