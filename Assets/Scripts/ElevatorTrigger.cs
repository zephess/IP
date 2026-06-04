using System.Collections;

using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    private AudioSource src;
    private Rigidbody[] rbs;
    private Rigidbody rb2;
    private Rigidbody rb3;
    public bool crashes;
    public GameObject elevatorGrate;
    public GameObject elevatorGrate2;
    public GameObject elevator;
    private bool canMoveGrate = true;
    private bool hasElevatorMoved = false;
    private GameObject player;
    public float distanceToMove;
  

    private AudioClip elevatorFalling;
    private AudioClip elevatorAmbience;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        src = elevator.GetComponent<AudioSource>();
        rbs = elevator.GetComponentsInChildren<Rigidbody>();
        rb2 = elevatorGrate.GetComponent<Rigidbody>();
        rb3 = elevatorGrate2.GetComponent<Rigidbody>();
        elevatorFalling = Resources.Load<AudioClip>("Audio/elevatorFalling");
        elevatorAmbience = Resources.Load<AudioClip>("Audio/elevatorAmbience");
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canMoveGrate && other.tag.Equals("Player"))
        {
            other.transform.SetParent(elevator.transform); // Parent the player to the elevator
            other.gameObject.GetComponent<PlayerController>().DisableController(); // Disable the CharacterController to prevent movement issues
            player = other.gameObject; // Store reference to the player

            StartCoroutine(MoveGrate(elevatorGrate.transform, 0.9f, 4f)); 

        }
        
    }

    private IEnumerator MoveGrate(Transform target, float distance, float time) // Coroutine to move the grate, which also triggers the elevator movement after a delay
    {
        canMoveGrate = false;
        Vector3 startPos = target.position;
        Vector3 endPos = new Vector3(target.position.x, target.position.y + distance, target.position.z);
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            target.position = Vector3.Lerp(startPos, endPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.position = endPos; // Ensure it ends at the exact position
        if(!hasElevatorMoved)
        {
            hasElevatorMoved = true;
            src.Play(); // Play the elevator sound
            StartCoroutine(MoveElevator(elevator.transform, -distanceToMove, 24f)); 
        }
       
    }

    private IEnumerator MoveElevator(Transform target, float distance, float time)
    {
        yield return new WaitForSeconds(1f); // Delay before moving the elevator
        src.PlayOneShot(elevatorAmbience); // Play the elevator ambience sound
        Vector3 startPos = target.position;
        Vector3 endPos = new Vector3(target.position.x, target.position.y + distance, target.position.z);
        Renderer renderer = elevator.GetComponent<Renderer>();
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            target.position = Vector3.Lerp(startPos, endPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        target.position = endPos; // Ensure it ends at the exact position
       
        yield return new WaitForSeconds(1f); // Delay before moving the grate back down
        if (!crashes) // If not scripted to crash
        {
            StartCoroutine(MoveGrate(elevatorGrate2.transform, -0.9f, 4f)); // Move the grate back down after the elevator has moved
        }
        else
        {
            yield return new WaitForSeconds(8f);
            src.PlayOneShot(elevatorFalling); // Play the elevator falling sound
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = false; // Make all child Rigidbodies non-kinematic to allow them to be affected by physics
            }
            
        }
    }   
}
