using UnityEngine;
using System.Collections.Generic;

public class SonarPulseManager : MonoBehaviour
{
    public static SonarPulseManager Instance;

    struct Pulse
    {
        public Vector3 origin;
        public float startTime;
    }

    List<Pulse> pulses = new List<Pulse>();

    public float pulseSpeed = 15f;
    public int maxPulses = 10;
    public SphereCollider detectionCollider;


    Vector4[] pulseOrigins;
    float[] pulseTimes;

    void Awake()
    {
        Instance = this;

        pulseOrigins = new Vector4[maxPulses];
        pulseTimes = new float[maxPulses];
    }

    public void EmitPulse(Vector3 origin)
    {
        if (pulses.Count >= maxPulses)
            pulses.RemoveAt(0);

        Pulse newPulse;
        newPulse.origin = origin;
        newPulse.startTime = Time.time;
        detectionCollider.transform.position = origin;
        detectionCollider.radius = 0.1f;
        pulses.Add(newPulse);
    }

    void Update()
    {
        int count = pulses.Count;

        for (int i = 0; i < count; i++)
        {
            pulseOrigins[i] = pulses[i].origin;
            pulseTimes[i] = pulses[i].startTime;
        }
        if(detectionCollider.radius < 20f)
        {
            detectionCollider.radius += pulseSpeed * Time.deltaTime;
        }
        Shader.SetGlobalInt("_PulseCount", count);
        Shader.SetGlobalVectorArray("_PulseOrigins", pulseOrigins);
        Shader.SetGlobalFloatArray("_PulseTimes", pulseTimes);
        Shader.SetGlobalFloat("_PulseSpeed", pulseSpeed);
    }
}