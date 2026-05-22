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
        if (other.CompareTag("Pulse"))
        {
            if (canEmit)
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
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
