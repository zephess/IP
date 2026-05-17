using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{

    private NavMeshAgent agent;
    private Transform player;
    private float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;
    private float wanderTimer;
    private bool isInvestigating = false;
    private Renderer rend;
    public LayerMask enemyMask;
    private Animator animator;
    private bool awake = false;
    private AudioSource audioSource;
    private AudioClip gargle1;
    private AudioClip gargle2;
    private AudioClip gargle3;
    private bool isGargling = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gargle1 = Resources.Load<AudioClip>("Audio/monsterGargle1");
        gargle2 = Resources.Load<AudioClip>("Audio/monsterGargle2");
        gargle3 = Resources.Load<AudioClip>("Audio/monsterGargle3");
        rend = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        wanderTimer = wanderInterval;
        agent.updatePosition = false;
        agent.updateRotation = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (awake)
        {
           // transform.position = agent.nextPosition;
            animator.SetFloat("moveSpeed", agent.velocity.magnitude);
            Debug.Log(agent.velocity.magnitude);
            Debug.DrawRay(transform.position, player.position - transform.position + Vector3.up, Color.red);
            Physics.Raycast(transform.position, player.position - transform.position + Vector3.up, out RaycastHit hit, Mathf.Infinity, enemyMask);
            //Debug.Log("Raycast hit: " + hit.collider?.gameObject.name);
            //if (rend.isVisible && hit.collider.gameObject.tag.Equals("Player"))
            //{
            //    agent.isStopped = true;
            //    Debug.Log("Enemy is visible to the player, stopping movement.");
            //}
            //else
            //{
            //    agent.isStopped = false;
            //    Debug.Log("Enemy is not visible to the player, resuming movement.");
            //}
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= detectionRange)
            {
                isInvestigating = false;
                agent.SetDestination(player.position);
                agent.speed = 2f;
                animator.SetBool("chasing", true);
                //animator.speed = 3f;
                return;
            }
            else agent.speed = 1.0f;
            animator.SetBool("chasing", false);
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= wanderInterval && !isInvestigating)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
                agent.SetDestination(newPos);
                wanderTimer = 0f;
            }
            if (isInvestigating)
            {
                if (Vector3.Distance(agent.destination, transform.position) < 1f)
                {
                    isInvestigating = false;
                    wanderTimer = 0f;
                }
            }
            if(!isGargling)
            {
                isGargling = true;
                StartCoroutine(Gargle());
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        
        Vector3 randDirection = Random.insideUnitSphere * dist;
        
        NavMesh.SamplePosition(origin + randDirection, out NavMeshHit hit, dist, NavMesh.AllAreas);
        
        return hit.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!awake && other.CompareTag("Pulse"))
        {
            StartCoroutine(WakeupSequence());
        }
        Debug.Log("Enemy hit by pulse!");
        if (awake) {
            if (other.CompareTag("Pulse"))
            {
                isInvestigating = true;
                agent.SetDestination(other.transform.position);
            }
        }
    }

    private IEnumerator WakeupSequence()
    {
        animator.SetTrigger("wakeup");
        //Debug.Log(animator.GetCurrentAnimatorStateInfo(1).length);
        yield return new WaitForSeconds(1.5f); 
        awake = true;
    }

    private IEnumerator Gargle()
    {
        
            
            int gargleIndex = Random.Range(1, 4);
            switch (gargleIndex)
            {
                case 1:
                    audioSource.PlayOneShot(gargle1);
                    break;
                case 2:
                    audioSource.PlayOneShot(gargle2);
                    break;
                case 3:
                    audioSource.PlayOneShot(gargle3);
                    break;
            }
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            isGargling = false;
        
    }

   
    //public void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Enemy hit by pulse!");
    //    if (collision.gameObject.CompareTag("Pulse"))
    //    {
    //        isInvestigating = true;
    //        agent.SetDestination(collision.transform.position);
    //        Destroy(collision.gameObject);
    //    }
    //}
}

