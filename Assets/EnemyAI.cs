using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class EnemyAI : MonoBehaviour
{
    public enum enemyState
    {
        Idle, Wandering, Chasing
    };

    private NavMeshAgent agent;
    private Transform player;
    private float detectionRange = 10f;
    public float wanderRadius = 5f;
    public float wanderInterval = 10f;
    private float wanderTimer;
    //private bool isInvestigating = false;
    private Renderer rend;
    public LayerMask enemyMask;
    private Animator animator;
    private bool awake = false;
    private AudioSource audioSource;
    private AudioClip gargle1;
    private AudioClip gargle2;
    private AudioClip gargle3;
    private bool isGargling = false;
    private enemyState state;
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
        
        state = enemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("moveSpeed", agent.velocity.magnitude);
        Debug.Log(state);
        float distance = Vector3.Distance(player.position, transform.position);
        wanderTimer += Time.deltaTime;

        if (wanderTimer > wanderInterval && state != enemyState.Chasing)
        {
            wanderTimer = 0;
            agent.destination = RandomNavSphere(transform.position, wanderRadius);
            agent.speed = 1f;
            ChangeState(enemyState.Wandering);
        }

        if (distance <= detectionRange)
        {
            agent.destination = player.position;
            agent.speed = 3f;
            ChangeState(enemyState.Chasing);

        }
        else if (state == enemyState.Chasing && distance > detectionRange) 
        {
            agent.ResetPath();
            ChangeState(enemyState.Idle);
        }

        if (agent.destination == agent.transform.position && state != enemyState.Chasing)
        {
            
            ChangeState(enemyState.Idle);
        }


        if(!isGargling)
        {
            isGargling = true;
            StartCoroutine(Gargle());
        }
        
    }

   
    private void ChangeState(enemyState newState)
    {
        state = newState;
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
                //isInvestigating = true;
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

