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
    private AudioClip screech;
    private bool isGargling = false;
    private enemyState state;
    private float chaseTimeout = 5f;
    private float chaseTimer = 0f;
    private bool chaseStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gargle1 = Resources.Load<AudioClip>("Audio/monsterGargle1");
        gargle2 = Resources.Load<AudioClip>("Audio/monsterGargle2");
        gargle3 = Resources.Load<AudioClip>("Audio/monsterGargle3");
        screech = Resources.Load<AudioClip>("Audio/chaseScreech");
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
        if(!awake) return; // If the enemy is not awake, do not execute any of the AI behavior in the Update function
        if (state == enemyState.Chasing && !chaseStarted)
        {
            chaseStarted = true;
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(screech);
        }

        animator.SetFloat("moveSpeed", agent.velocity.magnitude);
      
        float distance = Vector3.Distance(player.position, transform.position);
        wanderTimer += Time.deltaTime;
      
        if (wanderTimer > wanderInterval && state != enemyState.Chasing) // If the wander timer has exceeded the wander interval and the enemy is not currently chasing the player, find a new random destination for the enemy to wander to
        {
            wanderTimer = 0;
            agent.destination = RandomNavSphere(transform.position, wanderRadius);
            agent.speed = 1f;
            ChangeState(enemyState.Wandering);
        }

        if (distance <= detectionRange) // If the player is within detection range and the enemy is not currently chasing the player, start chasing the player
        {
            chaseTimer = 0f;
            agent.destination = player.position;
            agent.speed = 2f;
            ChangeState(enemyState.Chasing);

        }
        else if (state == enemyState.Chasing && distance > detectionRange)  // If the enemy is currently chasing the player but the player has moved out of detection range, start the chase timeout timer
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseTimeout)
            {
                agent.ResetPath();
                ChangeState(enemyState.Idle);
                chaseTimer = 0f;
                chaseStarted = false;
            }
        }

        if (agent.destination == agent.transform.position && state != enemyState.Chasing) // If the enemy has reached its destination and is not currently chasing the player, change its state to idle
        {
            
            ChangeState(enemyState.Idle);
        }


        if(!isGargling) // If the enemy is not currently gargling, start the gargle coroutine to play a random gargle sound at random intervals
        {
            isGargling = true;
            StartCoroutine(Gargle());
        }
      
    }
   

    private void ChangeState(enemyState newState) // Function to change the enemy's state, which is used to control its behavior in the Update function
    {
        state = newState;
       
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist) // Function to find a random point on the NavMesh within a certain radius of the origin point, which is used for the enemy's wandering behavior
    {
        
        Vector3 randDirection = Random.insideUnitSphere * dist;
        
        NavMesh.SamplePosition(origin + randDirection, out NavMeshHit hit, dist, NavMesh.AllAreas);
        
        return hit.position;
    }

    public void OnTriggerEnter(Collider other) 
    {
        if (!awake && other.CompareTag("Pulse")) // If the enemy is hit by a pulse and is not already awake, start the wakeup sequence
        {
            StartCoroutine(WakeupSequence());
        }
      
        if (awake) {
            if (other.CompareTag("Pulse")) // If the enemy is hit by a pulse and is already awake, start chasing the player
            {
                state = enemyState.Chasing;
                agent.speed = 2f;
                agent.SetDestination(other.transform.position);
                chaseTimer = 0f;
            }
        }
    }

    private IEnumerator WakeupSequence() // Coroutine to handle the enemy's wakeup sequence, which plays an animation and then sets the awake flag to true after a delay
    {
        animator.SetTrigger("wakeup");
        yield return new WaitForSeconds(14.5f / 3f); 
        awake = true;
    }

    private IEnumerator Gargle() // Coroutine to play a random gargle sound at random intervals, which is used to add atmosphere to the enemy when it is awake
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

}

