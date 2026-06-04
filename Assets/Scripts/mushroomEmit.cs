using System.Collections;
using UnityEngine;

public class mushroomEmit : MonoBehaviour
{
    public SonarPulseManager pulseManager;
    private AudioSource src;
    private AudioClip pulseSound;
    private bool canEmit = true;
    public float radius = 5f;

    void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Collided with: " + other.gameObject.name);
        if (other.CompareTag("Pulse")) // Check if the colliding object has the "Pulse" tag
        {
            if (canEmit) // Check if the mushroom can emit a pulse
            {
                canEmit = false;
                pulseManager.EmitPulse(transform.position, radius);
                src.PlayOneShot(pulseSound);
                Debug.Log("Mushroom hit by pulse!");
                StartCoroutine(PulseCooldown());
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        src = GetComponent<AudioSource>();
        pulseSound = Resources.Load<AudioClip>("Audio/sonarPulse");
        pulseManager = SonarPulseManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PulseCooldown()
    {
        yield return new WaitForSeconds(5f);
        canEmit = true;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius); // Draw a wireframe sphere to visualize the pulse radius in the editor
    }
}
