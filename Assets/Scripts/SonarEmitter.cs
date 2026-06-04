
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public Volume vol;
    public float cooldown = 1f;
    void Start()
    {
        
        Debug.Log(src.gameObject.name);
        pulseSound = Resources.Load<AudioClip>("Audio/sonarPulse");
      
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Update the list of enemies every frame
        if (cooldown >= 0f) // Reduce cooldown timer if it's above 0
        {
            cooldown -= Time.deltaTime;
        }
       
        timer += Time.deltaTime;

        if(automatic && timer >= pulseInterval) // If automatic mode is enabled and the timer has exceeded the pulse interval, emit a pulse
        {
            EmitPulse();
            timer = 0f;
        }
       
        if (automatic)
        {
            pulseInterval = GetPulseInterval();
        }

        if (!automatic && Input.GetKeyDown(KeyCode.Q) && cooldown <= 0) // If manual mode is enabled and the Q key is pressed, emit a pulse
        {
            EmitPulse();
            cooldown = 1f;
        }
        if (GetDistanceToClosest() < 5f) // If the closest enemy is within 5 units, change the pulse color to red
        {
            pulseMaterial.SetColor("_LineColor", Color.red);
        }
        if(GetDistanceToClosest() <= dangerDistance) // If the closest enemy is within the danger distance, adjust visual effects based on proximity
        {
            vol.profile.TryGet(out ChromaticAberration crmab);
            if (crmab != null)
            {
                //Debug.Log(crmab.intensity.value);
                crmab.intensity.value = Mathf.Lerp(1.0f, 0.0f, GetDistanceToClosest()/dangerDistance);
            }
            vol.profile.TryGet(out Bloom bloom);
            if (bloom != null)
            {
                bloom.tint.value = Color.Lerp(Color.red, Color.aquamarine, GetDistanceToClosest() / dangerDistance);
            }
            lerpedColor = Color.Lerp(Color.red, Color.aquamarine, GetDistanceToClosest()/dangerDistance); // Lerp the pulse color from red to aquamarine based on the distance to the closest enemy
            pulseMaterial.SetColor("_LineColor", lerpedColor); // Set the pulse color to the lerped color
        }
        else
        {
            pulseMaterial.SetColor("_LineColor", Color.aquamarine); // Set the pulse color to aquamarine if no enemies are within the danger distance
        }

    }

    public void EmitPulse() // Function to emit a sonar pulse, which also plays a sound effect
    {
        if (SonarPulseManager.Instance != null )
        {
            SonarPulseManager.Instance.EmitPulse(transform.position); // Emit a pulse from the SonarPulseManager at the position of this emitter

        }
        if (src != null)
        {
            src.PlayOneShot(pulseSound);

        }
    }

    float GetDistanceToClosest() // Function to calculate the distance to the closest enemy, which is used for adjusting pulse intervals and visual effects
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

    float GetPulseInterval() // Function to calculate the pulse interval based on the distance to the closest enemy, which creates a dynamic pulse rate that increases as enemies get closer
    {
        float distance = GetDistanceToClosest();
        float t = Mathf.InverseLerp(5f, dangerDistance, distance);
        float interval = Mathf.Lerp(1f, defaultPulseInterval, t);
        return interval;
    }

    public void DisableEmitter() // Function to disable the sonar emitter
    {
        this.enabled = false;
    }

    public void EnableEmitter() // Function to enable the sonar emitter
    {
        this.enabled = true;
    }
}
