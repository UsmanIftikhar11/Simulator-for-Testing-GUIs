using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotMovement : MonoBehaviour, PlayerControls.IRobotActions
{

    //two available cameras:
    public Camera firstPerson;
    public Camera spectator;

    //speeds;
    public float speed = 5f;    //initial speed of the robot
    public float rotationSpeed = 120f; // how fast the robot rotates
    public float alignSpeed = 8f;   //how fast the robot rotates toward surface

    //the force of the magnet
    public float stickForce = 30f;

    //the robots rigidbody
    private Rigidbody rb;

    //the input from the keyboard
    private Vector2 moveInput;

    // vectors:
    private Vector3 currentSurfaceNormal = Vector3.up; //the normal of the surface we are currently on
    private Vector3 surfaceForward = Vector3.forward; // a vector poionting forward, used as the yaw-reference

    //accumulated yaw around the currentSurfaceNormal
    private float yawDegrees = 0f;

    // the robot mesh is rotated 90 degrees, otherwise it will stand up
    private Quaternion meshOffset = Quaternion.Euler(90f, 0f, 0f);

    //keeping track on the last collided surface to avoid updating it every frame
    private Collider lastSurfaceCollider = null;

    // if the angle changes more than 5 degrees, we will update the surface normal
    private const float normalUpdateAngleThreshold = 5f;

    // preparation for the tools
    public bool plasmaTorchActive = false;
    public bool cleaningHeadActive = false;

    private PlayerControls controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        firstPerson.gameObject.SetActive(true);
        spectator.gameObject.SetActive(false);
        controls = new PlayerControls();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnEnable()
    {
        controls.Robot.SetCallbacks(this);
        controls.Robot.Enable();
    }

    void OnDisable()
    {
        controls.Robot.Disable();
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Exit pressed!");
            Application.Quit();
        }
    }

    public void OnToggleCamera(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        bool firstActive = firstPerson.gameObject.activeSelf;
        firstPerson.gameObject.SetActive(!firstActive);
        spectator.gameObject.SetActive(firstActive);

        Debug.Log("Camera toggled!");
    }

    public void OnCleaning(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            cleaningHeadActive = !cleaningHeadActive;
            Debug.Log(cleaningHeadActive ? "Cleaning enabled" : "Cleaning disabled");
        }
    }

    public void OnCutting(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            plasmaTorchActive = !plasmaTorchActive;
            Debug.Log(plasmaTorchActive ? "Cutting enabled" : "Cutting disabled");
        }
    }


    void Update()
    {
        //switching between the two cameras
        /*
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (firstPerson.gameObject.activeSelf)
            {
                firstPerson.gameObject.SetActive(false);
                spectator.gameObject.SetActive(true);
            }
            else
            {
                firstPerson.gameObject.SetActive(true);
                spectator.gameObject.SetActive(false);
            }
        }*/
        /*
        bool toggle =
        Keyboard.current.cKey.wasPressedThisFrame ||
        Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;

        if (toggle)
        {
            bool firstActive = firstPerson.gameObject.activeSelf;
            firstPerson.gameObject.SetActive(!firstActive);
            spectator.gameObject.SetActive(firstActive);
            Debug.Log("Camera toggled!");
        }*/

        // change speed with number keys
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            speed = 1f;
            Debug.Log("Speed: 1");
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            speed = 3f;
            Debug.Log("Speed: 2");
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            speed = 5f;
            Debug.Log("Speed: 3");
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            speed = 6f;
            Debug.Log("Speed: 4");
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            speed = 8f;
            Debug.Log("Speed: 5");
        }
    }

    void FixedUpdate()
    {


       
        //movement, updating of the robots position
        float moveForward = -moveInput.y * speed;
        Vector3 movement = transform.up * moveForward * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        //this simulates the force of the magnet on the robot
        rb.AddForce(-currentSurfaceNormal * stickForce, ForceMode.Acceleration);


        // updating how many degrees the robot has turned around the surface normal since last surface commit
        yawDegrees += moveInput.x * rotationSpeed * Time.fixedDeltaTime;
        Quaternion yawRot = Quaternion.AngleAxis(yawDegrees, currentSurfaceNormal);
        Vector3 desiredForwardOnSurface = (yawRot * surfaceForward).normalized;

        //if something goes wrong
        if (desiredForwardOnSurface.sqrMagnitude < 0.001f)
        {
            desiredForwardOnSurface = Vector3.ProjectOnPlane(transform.forward, currentSurfaceNormal).normalized;
            if (desiredForwardOnSurface.sqrMagnitude < 0.001f) desiredForwardOnSurface = Vector3.Cross(currentSurfaceNormal, transform.right).normalized;
        }

        // desired rotation so that object's forward = desiredForwardOnSurface, object's up = currentSurfaceNormal
        Quaternion desiredRotation = Quaternion.LookRotation(desiredForwardOnSurface, currentSurfaceNormal);

        // compensate for the mesh offset
        Quaternion targetRbRotation = desiredRotation * Quaternion.Inverse(meshOffset);

        // interpolates the current rotation to the target rotation
        Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRbRotation, Mathf.Clamp01(alignSpeed * Time.fixedDeltaTime));
        rb.MoveRotation(newRotation);
    }

    // is called by OnCollisionEnter. Handles corners and edges of the walls
    private Vector3 BestContactNormal(Collision collision, Vector3 referenceUp)
    {
        Vector3 best = collision.contacts.Length > 0 ? collision.contacts[0].normal : referenceUp;
        float bestDot = Vector3.Dot(referenceUp, best);
        foreach (var c in collision.contacts)
        {
            float d = Vector3.Dot(referenceUp, c.normal);
            if (d > bestDot)
            {
                bestDot = d;
                best = c.normal;
            }
        }
        return best;
    }

    // when we collide with a new wall: get the best contact normal, and adjust to it
    private void OnCollisionEnter(Collision collision)
    {
        // choose best contact normal
        Vector3 newNormal = BestContactNormal(collision, currentSurfaceNormal);
        CommitNewSurface(collision.collider, newNormal);
    }

    // checks if we have collided with a new surface or not, and only changes normal if it is changed noticably 
    private void OnCollisionStay(Collision collision)
    {
        // If we touched a different collider, treat it as entering a new surface
        if (collision.collider != lastSurfaceCollider)
        {
            Vector3 newNormal = BestContactNormal(collision, currentSurfaceNormal);
            CommitNewSurface(collision.collider, newNormal);
            return;
        }

        // Otherwise, only update the normal if it changed noticeably (prevents per-frame jitter/work)
        Vector3 best = BestContactNormal(collision, currentSurfaceNormal);
        float angle = Vector3.Angle(best, currentSurfaceNormal);
        if (angle > normalUpdateAngleThreshold)
        {
            CommitNewSurface(collision.collider, best);
        }
    }

    // this if or when the crawler leaves a surface
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == lastSurfaceCollider)
        {
            lastSurfaceCollider = null;
            // default back to world-up when leaving surfaces; you can change behavior here
            currentSurfaceNormal = Vector3.up;

            // reproject the current transform.forward onto horizontal plane so we keep controlling yaw sensibly
            surfaceForward = Vector3.ProjectOnPlane(transform.forward, currentSurfaceNormal).normalized;
            yawDegrees = 0f;
        }
    }

    // everything that happens when entering a new surface.
    private void CommitNewSurface(Collider newCollider, Vector3 newNormal)
    {
        Vector3 oldNormal = currentSurfaceNormal;
        Vector3 oldForward = surfaceForward;

        lastSurfaceCollider = newCollider;
        currentSurfaceNormal = newNormal.normalized;

        // Rotation that aligns oldNormal to newNormal
        Quaternion align = Quaternion.FromToRotation(oldNormal, currentSurfaceNormal);

        // Rotate the old tangent direction using the surface-to-surface rotation
        Vector3 rotatedForward = align * oldForward;

        // Project onto new tangent plane to clean it
        Vector3 candidate = Vector3.ProjectOnPlane(rotatedForward, currentSurfaceNormal);

        if (candidate.sqrMagnitude < 0.001f)
        {
            // Fallback if tangent collapses: choose any stable perpendicular
            candidate = Vector3.Cross(currentSurfaceNormal, Vector3.up);
            if (candidate.sqrMagnitude < 0.001f)
                candidate = Vector3.Cross(currentSurfaceNormal, Vector3.right);
        }

        candidate.Normalize();
        surfaceForward = candidate;

        yawDegrees = 0f;
    }
}
