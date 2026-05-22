using System.Collections;
using UnityEngine;

public class doorTrigger : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    private AudioSource src;
    private AudioClip scrape;
    private float timer = 6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        src = GetComponent<AudioSource>();
        scrape = Resources.Load<AudioClip>("Audio/scrape");
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 5)
        {
            timer += Time.deltaTime;
            door1.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -45, 0), timer / scrape.length);
            door2.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0,0,0), Quaternion.Euler(0, 45, 0), timer / scrape.length);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer = 0;
            StartCoroutine(DoorOpening());

        }
    }
    private IEnumerator DoorOpening()
    {
        src.PlayOneShot(scrape);
        yield return new WaitForSeconds(scrape.length);
        Destroy(gameObject);
    }
}
