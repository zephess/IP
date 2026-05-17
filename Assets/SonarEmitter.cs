
using UnityEngine;

public class SonarEmitter : MonoBehaviour
{
    public float pulseInterval = 3f;
    public bool automatic = true;
    public Material pulseMaterial;
    private Color lerpedColor;
    public float dangerDistance = 20f;
    private GameObject[] enemies;
    public AudioSource src;
    public AudioClip pulseSound;
    private float defaultPulseInterval = 3f;
    private float timer = 0f;
    void Start()
    {
        
        Debug.Log(src.gameObject.name);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        pulseSound = Resources.Load<AudioClip>("Audio/sonarPulse");
        //if (automatic)
        //    InvokeRepeating(nameof(EmitPulse), 0f, defaultPulseInterval);
    }

    void Update()
    {
        //enemies = GameObject.FindGameObjectsWithTag("Enemy");
        timer += Time.deltaTime;
        if(automatic && timer >= pulseInterval)
        {
            EmitPulse();
            timer = 0f;
        }
        //Debug.Log(pulseInterval);
        if (automatic)
        {
            pulseInterval = GetPulseInterval();
        }
        if (!automatic && Input.GetKeyDown(KeyCode.Q))
        {
            EmitPulse();
        }
        if (GetDistanceToClosest() < 5f)
        {
            pulseMaterial.SetColor("_LineColor", Color.red);
        }
        if(GetDistanceToClosest() <= dangerDistance)
        {
            lerpedColor = Color.Lerp(Color.red, Color.aquamarine, GetDistanceToClosest()/dangerDistance);
            pulseMaterial.SetColor("_LineColor", lerpedColor);
        }
        else
        {
            pulseMaterial.SetColor("_LineColor", Color.aquamarine);
        }

    }

    void EmitPulse()
    {
        if (SonarPulseManager.Instance != null)
        {
            SonarPulseManager.Instance.EmitPulse(transform.position);
        }
        if (src != null)
        {
            src.PlayOneShot(pulseSound);
        }
    }

    float GetDistanceToClosest()
    {
        float closestDistance = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(pos, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        return closestDistance;
    }

    float GetPulseInterval()
    {
        float distance = GetDistanceToClosest();
        float t = Mathf.InverseLerp(5f, dangerDistance, distance);
        float interval = Mathf.Lerp(1f, defaultPulseInterval, t);
        return interval;
    }
}
