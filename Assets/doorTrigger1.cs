using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class doorTrigger1 : MonoBehaviour
{
    public GameObject door1;
    public GameObject door2;
    private AudioSource src;
    private AudioClip slam;
    private float timer = 6f;
    public GameObject[] enemies;
    public SonarEmitter emitter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        src = GetComponent<AudioSource>();
        slam = Resources.Load<AudioClip>("Audio/slam");
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 5)
        {
            timer += Time.deltaTime;
            door1.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 45, 0), Quaternion.Euler(0, 0, 0), timer / 1f);
            door2.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0,-45,0), Quaternion.Euler(0, 0, 0), timer / 1f);
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            timer = 0;
            StartCoroutine(DoorSlam());
        }
    }
    private IEnumerator DoorSlam()
    {
        emitter.DisableEmitter();
        src.PlayOneShot(slam);
        yield return new WaitForSeconds(slam.length);
        foreach (GameObject en in enemies)
        {
            en.SetActive(true);

        }
        emitter.EnableEmitter();
        emitter.EmitPulse();
        emitter.DisableEmitter();
        yield return new WaitForSeconds(3f);
        emitter.EnableEmitter();
        emitter.EmitPulse();
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene("menu");

        Destroy(gameObject);
    }
}
