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
    public Light flickerLight; // Reference to the light you want to flicker
    public Material sonarRevealMat; // Reference to the material you want to apply to the elevator after it moves
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
        //StartCoroutine(FlickerLight()); // Flicker the light for 10 seconds with a 0.5 second interval
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
            SonarPulseManager.Instance.pulseSpeed = 10f;
            StartCoroutine(MoveGrate(elevatorGrate.transform, 0.9f, 4f)); // Move the grate up by 3 units over 2 seconds

        }
        
    }

    private IEnumerator MoveGrate(Transform target, float distance, float time)
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
            StartCoroutine(MoveElevator(elevator.transform, -distanceToMove, 24f)); // Move the elevator down by 3 units over 3 seconds
        }
       
    }

    private IEnumerator MoveElevator(Transform target, float distance, float time)
    {
        yield return new WaitForSeconds(1f); // Optional delay before moving the elevator
        src.PlayOneShot(elevatorAmbience); // Play the elevator ambience sound
        Vector3 startPos = target.position;
        Vector3 endPos = new Vector3(target.position.x, target.position.y + distance, target.position.z);
        Renderer renderer = elevator.GetComponent<Renderer>();
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            target.position = Vector3.Lerp(startPos, endPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            if(elapsedTime > 2f)
            {
                if (renderer != null)
                {
                    renderer.material = sonarRevealMat; // Change the material of the elevator to the sonar reveal material
                }
                renderer = elevatorGrate.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = sonarRevealMat; // Change the material of the elevator grate to the sonar reveal material
                }
                renderer = elevatorGrate2.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = sonarRevealMat; // Change the material of the second elevator grate to the sonar reveal material
                }
                renderer = null;

            }
            yield return null;
        }
        target.position = endPos; // Ensure it ends at the exact position
        //player.transform.SetParent(null); // Unparent the player from the elevator
        yield return new WaitForSeconds(1f); // Optional delay before moving the grate back down
        if (!crashes)
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
            rb2.isKinematic = false; // Make the first grate's Rigidbody non-kinematic to allow it to be affected by physics
            rb3.isKinematic = false; // Make the second grate's Rigidbody non-kinematic to allow it to be affected by physics
            //rb.AddExplosionForce(500f, target.position + Vector3.right, 10f); // Apply an explosion force to the elevator to make it crash
        }
        //StartCoroutine(FlickerLight()); // Start flickering the light after the grate has moved
        
        //player.gameObject.GetComponent<PlayerController>().EnableController(); // Re-enable the CharacterController to allow player movement

        

    }

   
}
