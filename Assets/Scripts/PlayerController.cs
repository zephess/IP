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
       // rb = GetComponent<Rigidbody>();
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
        //if (rb.linearVelocity.magnitude > 3f)
        //{
            //Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x , cameraOrigin.localPosition.y + Mathf.Sin(timer) * bobAmount, Camera.main.transform.localPosition.z);
       // }
        
            //Debug.Log(speed);

        


       // Debug.Log(rb.linearVelocity.magnitude);
        
        HandleLook();
    }

    private void FixedUpdate()
    {
        if (!isEnabled) return;

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
        if(controller.velocity.magnitude < 0.1f)
        {
            //src.Stop();
        }
    }


   


    private void HandleLook()
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
            //SonarPulseManager.Instance.pulseSpeed = 20f;
            transform.SetParent(null); // Unparent the player from the elevator
            EnableController();
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Reset the player's rotation to be upright
        }
        if (other.tag.Equals("JumpSequence1"))
        {
            other.enabled = false;
            StartCoroutine(JumpscareSequence1());
        }
        if (other.CompareTag("Enemy") || other.CompareTag("Hazard"))
        {
            Debug.Log("collided with enemy");
            StartCoroutine(GameOver());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

    }

    public void DisableController()
    {
        controller.enabled = false;
        isEnabled = false;
    }

    public void EnableController()
    {
        controller.enabled = true;
        isEnabled = true;
    }

    private IEnumerator JumpscareSequence1()
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
        //SonarPulseManager.Instance.pulseSpeed = 30f;
        emitter.EmitPulse();
        src.PlayOneShot(Resources.Load<AudioClip>("Audio/chaseScreech"), 1.4f);
        emitter.DisableEmitter();
        
        yield return new WaitForSeconds(1f);
        Destroy(enemyInst);
        emitter.EnableEmitter();
        EnableController();
       // SonarPulseManager.Instance.pulseSpeed = 10f;

    }
    private IEnumerator GameOver()
    {
        src.PlayOneShot(Resources.Load<AudioClip>("Audio/grunt"));
        cnv.alpha = 1f;
        DisableController();
        
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("menu");
        

    }
}
