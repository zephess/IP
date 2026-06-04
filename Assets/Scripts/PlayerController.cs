using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    
    public GameObject sonarPulsePrefab;
    public float cooldownTime = 1.2f;
    private float cooldownTimer = 0f;

    public GameObject pulsePrefab;
    private Rigidbody rb;
    private CharacterController charC;
    public float jumpForce;
    [SerializeField]
    private Transform orientation;
    public float speed = 10f;
    private float xRotation = 0f;
    public float mouseSensitivity = 100f;
    private Transform cameraTransform;
    private float pulseCooldown = 2f;
    private float maxSpeed = 5f;
    private Collider col;
    private float gravity = -9.8f;
    private bool cameraBobInProgress = false;
    private Transform cameraOrigin;
    private float timer = 0f;
    public float bobSpeed = 6f;
    public float bobAmount = 0.0005f;
    private float verticalVelocity = 0f;
    private bool isEnabled = true;
    private AudioSource src;
    public AudioClip footstep;
    private CharacterController controller;
    private AudioClip elevatorCrash;
    private AudioClip sonarSizzle;
    public GameObject enemyPrefab;
    private CanvasGroup cnv;
    
    // Start is called before the first frame update
    void Start()
    {
        src = GetComponent<AudioSource>();
        col = GetComponent<Collider>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        cameraOrigin = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        footstep = Resources.Load<AudioClip>("Audio/footsteps");
        elevatorCrash = Resources.Load<AudioClip>("Audio/elevatorCrash");
        sonarSizzle = Resources.Load<AudioClip>("Audio/sonarSizzle");
        cnv = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {   
        timer += Time.deltaTime;
        HandleLook();
    }

    private void FixedUpdate()
    {
        if (!isEnabled) return; // If the controller is disabled, skip the movement code

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        move = transform.TransformDirection(move);


        bool grounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f);
        Vector3 groundNormal = grounded ? hit.normal : Vector3.up;

        Vector3 slopeMove = Vector3.ProjectOnPlane(move, groundNormal);

        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.fixedDeltaTime;
        }

        Vector3 finalMove = slopeMove * speed + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.fixedDeltaTime);
        if(controller.velocity.magnitude > 0.1f && grounded)
        {
            if (timer >= footstep.length)
            {
                src.pitch = Random.Range(0.8f, 1.2f);
                src.PlayOneShot(footstep);
                timer = 0f;
            }
        }
    }


   


    private void HandleLook() // Handles the player's looking around with the mouse, including clamping the vertical rotation and rotating the player horizontally based on mouse input
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; 
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("ControllerEnable"))
        {
            
            Debug.Log("Entered elevator trigger");
            transform.SetParent(null); // Unparent the player from the elevator
            EnableController();
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Reset the player's rotation to be upright
        }
        if (other.tag.Equals("JumpSequence1")) // If the player enters the trigger for the first jumpscare sequence, disable the trigger and start the jumpscare sequence
        {
            other.enabled = false;
            StartCoroutine(JumpscareSequence1());
        }
        if (other.CompareTag("Enemy") || other.CompareTag("Hazard")) // If the player collides with an enemy or hazard, start the game over sequence
        {
            Debug.Log("collided with enemy");
            StartCoroutine(GameOver());
        }
    }

    public void DisableController() // Disables the character controller and sets the isEnabled flag to false
    {
        controller.enabled = false;
        isEnabled = false;
    }

    public void EnableController() // Enables the character controller and sets the isEnabled flag to true
    {
        controller.enabled = true;
        isEnabled = true;
    }

    private IEnumerator JumpscareSequence1()    // Jumpscare sequence that spawns an enemy and plays a sonar pulse effect, then destroys the enemy after a short time
    {
        ParticleSystem sys = gameObject.GetComponentInChildren<ParticleSystem>();
        DisableController(); 
        SonarEmitter emitter = GetComponentInChildren<SonarEmitter>(); 
        if (emitter != null)
        {
            emitter.DisableEmitter();
        }
        sys.Play();
        src.PlayOneShot(sonarSizzle);
        yield return new WaitForSeconds(sys.main.duration);
        src.Stop();
        GameObject enemyInst = Instantiate(enemyPrefab, transform.position + transform.forward  - transform.up, Quaternion.identity);
        enemyInst.transform.LookAt(transform);

        emitter.EnableEmitter();
        
        emitter.EmitPulse();
        src.PlayOneShot(Resources.Load<AudioClip>("Audio/chaseScreech"), 1.4f);
        emitter.DisableEmitter();
        
        yield return new WaitForSeconds(1f);
        Destroy(enemyInst);
        emitter.EnableEmitter();
        EnableController();

    }
    private IEnumerator GameOver() // Game over sequence that plays a sound effect, shows a game over screen, and then returns to the main menu after a short delay
    {
        src.PlayOneShot(Resources.Load<AudioClip>("Audio/grunt"));
        cnv.alpha = 1f;
        DisableController();
        
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("menu");
        

    }
}
