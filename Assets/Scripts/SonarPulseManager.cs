using UnityEngine;
using System.Collections.Generic;

public class SonarPulseManager : MonoBehaviour
{
    public static SonarPulseManager Instance;

    struct Pulse
    {
        public Vector3 origin;
        public float startTime;
        public float radius;
        public float fade;
        public float width;
        public float maxRadius;

    }

    List<Pulse> pulses = new List<Pulse>();

    public float pulseSpeed = 15f;
    public int maxPulses = 10;
    public SphereCollider detectionCollider;
    public float maxDetectionRadius = 40f;
    public float pulseWidth = 7.25f;

    Vector4[] pulseOrigins;
    float[] pulseTimes;
    float[] pulseRadii;
    float[] pulseFades;
    float[] pulseWidths;

    void Awake()
    {
        Instance = this;

        pulseOrigins = new Vector4[maxPulses];
        pulseTimes = new float[maxPulses];
        pulseRadii = new float[maxPulses];
        pulseFades = new float[maxPulses];
        pulseWidths = new float[maxPulses];
    }

    public void EmitPulse(Vector3 origin)
    {
        if (pulses.Count >= maxPulses)
            pulses.RemoveAt(0);

        Pulse newPulse;
        newPulse.origin = origin;
        newPulse.startTime = Time.time;
        newPulse.radius = 0f;
        newPulse.fade = 1f;
        newPulse.width = pulseWidth;
        newPulse.maxRadius = maxDetectionRadius;
        detectionCollider.transform.position = origin;
        detectionCollider.radius = 0.1f;
        pulses.Add(newPulse);
    }
    public void EmitPulse(Vector3 origin, float radius)
    {
        if (pulses.Count >= maxPulses)
            pulses.RemoveAt(0);

        Pulse newPulse;
        newPulse.origin = origin;
        newPulse.startTime = Time.time;
        newPulse.radius = 0f;
        newPulse.fade = 1f;
        newPulse.width = pulseWidth;
        newPulse.maxRadius = radius;
        detectionCollider.transform.position = origin;
        detectionCollider.radius = 0.1f;
        pulses.Add(newPulse);
    }

    void Update()
    {
        int count = pulses.Count;

        for (int i = 0; i < count; i++)
        {
            Pulse p = pulses[i];
            p.radius += pulseSpeed * Time.deltaTime;
            p.fade = Mathf.InverseLerp(pulses[i].maxRadius, 0f, p.radius);

            pulseOrigins[i] = pulses[i].origin;
            pulseTimes[i] = pulses[i].startTime;
            pulseRadii[i] = pulses[i].radius;
            pulseFades[i] = pulses[i].fade;
            pulseWidths[i] = pulses[i].width;

            pulses[i] = p;
        }
        if (count > 0 && detectionCollider.radius <= pulses[0].maxRadius)
        {
            detectionCollider.radius += pulseSpeed * Time.deltaTime;

        }
        if (count > 0 && detectionCollider.radius >= pulses[0].maxRadius)
        {
            pulses.RemoveAt(0);
        }
        Shader.SetGlobalInt("_PulseCount", count);
        Shader.SetGlobalVectorArray("_PulseOrigins", pulseOrigins);
        Shader.SetGlobalFloatArray("_PulseTimes", pulseTimes);
        Shader.SetGlobalFloat("_PulseSpeed", pulseSpeed);
        Shader.SetGlobalFloatArray("_PulseRadii", pulseRadii);
        Shader.SetGlobalFloatArray("_PulseFades", pulseFades);
        Shader.SetGlobalFloatArray("_PulseWidths", pulseWidths);
    }
}