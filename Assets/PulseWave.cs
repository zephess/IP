using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PulseWave : MonoBehaviour
{
    private DecalProjector projector;
    private float currentFade = 10f;
    public float fadeOutSpeed = 1f;
    public float expansionSpeed = 20f;
    private float maxSize = 100f;
    private float currentSize = 0.1f;
    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        projector = GetComponent<DecalProjector>();
        col = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentSize += expansionSpeed * Time.deltaTime;
        projector.size = new Vector3 (currentSize, currentSize, currentSize);
        col.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
        float progress = currentSize / maxSize;              
        currentFade = 1f - progress;                         
        projector.fadeFactor = Mathf.Clamp01(currentFade);

        // Destroy when done
        if (currentSize >= maxSize || currentFade <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void Initialise(float pulseSpeed)
    {
        
    }
    public void setMax(float max)
    {
        maxSize = max;
    }
    

    
}
