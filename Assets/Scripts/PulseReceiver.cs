using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseReceiver : MonoBehaviour
{
    public GameObject pulsePrefab;
    public float pulseCooldown = 4f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        pulseCooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(pulseCooldown <= 0)
        {
            {
                if (other.tag.Equals("Pulse"))
                {
                    pulseCooldown = 1.5f;
                    GameObject pulse = Instantiate(pulsePrefab, transform.position, Quaternion.Euler(90, 0, 0));
                    pulse.GetComponent<PulseWave>().setMax(20f);
                }
            }
            
        }
        Debug.Log("pulse");
       
    }

}
