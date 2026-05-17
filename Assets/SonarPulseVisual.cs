using UnityEngine;

public class SonarPulseVisual : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;

    float scale;

    void Update()
    {
        scale += speed * Time.deltaTime;
        transform.localScale = Vector3.one * scale;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
            Destroy(gameObject);
    }
}
