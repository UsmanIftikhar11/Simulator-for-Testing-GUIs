using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class RobotMovement : MonoBehaviour, PlayerControls.IRobotActions
{
    // two available cameras
    public Camera firstPerson;
    public Camera spectator;

    // speeds
    public float speed = 5f;          // initial speed of the robot
    public float rotationSpeed = 120f; // how fast the robot rotates
    public float alignSpeed = 2f;      // how fast the robot rotates toward surface

    // the force of the magnet
    public float stickForce = 30f;

    // the robot's rigidbody
    private Rigidbody rb;

    // input
    private Vector2 moveInput;
    private Vector2 rotateInput;

    // surface data
    private Vector3 currentSurfaceNormal = Vector3.up;   // normal of the surface we are currently on

    // the robot mesh is rotated 90 degrees, otherwise it will stand up
    private Quaternion meshOffset = Quaternion.Euler(90f, 0f, 0f);

    // if the angle changes more than 5 degrees, we update the surface normal
    private const float normalUpdateAngleThreshold = 5f;

    // tools
    public bool plasmaTorchActive = false;
    public bool cleaningHeadActive = false;

    private PlayerControls controls;
    private Quaternion orientation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        firstPerson.gameObject.SetActive(true);
        spectator.gameObject.SetActive(false);
        controls = new PlayerControls();
        orientation = rb.rotation;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (OnButtonClick.IsPaused)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (OnButtonClick.IsPaused)
        {
            moveInput = Vector2.zero;
            return;
        }

        rotateInput = context.ReadValue<Vector2>();
        Debug.Log("Rotate input: " + rotateInput);
    }

    public void SetInputEnabled(bool enabled)
    {
        if (enabled)
        {
            controls.Robot.Enable();
        }
        else
        {
            controls.Robot.Disable();
            moveInput = Vector2.zero;
            rotateInput = Vector2.zero;
        }
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
        if (!context.performed) return;

        string currentScene = SceneManager.GetActiveScene().name;

        // If we're in the main game, treat Exit as Pause instead of quitting
        if (currentScene == "ShipYard Demo")
        {
            Debug.Log("Exit pressed in game: pausing instead of quitting.");
            OnButtonClick pauseScript = FindFirstObjectByType<OnButtonClick>();

            if (pauseScript != null)
            {
                pauseScript.OnPauseButton();
            }
            else
            {
                Debug.LogWarning("Pause script not found when trying to pause from OnExit.");
            }
        }
        else
        {
            Debug.Log("Exit pressed outside gameplay: quitting.");
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
        if (OnButtonClick.IsPaused)
            return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "ShipYard Demo")
        {
            // CAMERA TOGGLE
            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                bool firstActive = firstPerson.gameObject.activeSelf;
                firstPerson.gameObject.SetActive(!firstActive);
                spectator.gameObject.SetActive(firstActive);
                Debug.Log("Camera toggled!");
            }

            // CLEANING HEAD
            if ((Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame))
            {
                cleaningHeadActive = !cleaningHeadActive;
                Debug.Log(cleaningHeadActive ? "Cleaning enabled" : "Cleaning disabled");
            }

            // CUTTING
            if ((Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame))
            {
                plasmaTorchActive = !plasmaTorchActive;
                Debug.Log(plasmaTorchActive ? "Cutting enabled" : "Cutting disabled");
            }

            // PAUSE GAME
            if ((Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame))
            {
                OnButtonClick pauseScript = FindFirstObjectByType<OnButtonClick>();
                if (pauseScript != null)
                    pauseScript.OnPauseButton();
            }
        }

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
        // the force of the magnet
        rb.AddForce(-currentSurfaceNormal * stickForce, ForceMode.Acceleration);

        // the rotation
        if (Mathf.Abs(rotateInput.x) > 0.001f)
        {
            Quaternion deltaYaw = Quaternion.AngleAxis(
                rotateInput.x * rotationSpeed * Time.fixedDeltaTime,
                currentSurfaceNormal
            );

            orientation = deltaYaw * orientation;
        }

        //aligning to surface normal
        Quaternion alignToSurface =
            Quaternion.FromToRotation(orientation * Vector3.up, currentSurfaceNormal);

        orientation = alignToSurface * orientation;

        // movement
        Vector3 forward = Vector3.ProjectOnPlane(
            orientation * Vector3.forward,
            currentSurfaceNormal
        ).normalized;

        Vector3 movement = forward * (moveInput.y * speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + movement);

        // applying the rotation
        Quaternion targetRotation = orientation * Quaternion.Inverse(meshOffset);
        rb.MoveRotation(
            Quaternion.Slerp(rb.rotation, targetRotation, alignSpeed * Time.fixedDeltaTime)
        );
    }

    // finds the best and most natural contact normal
    private Vector3 BestContactNormal(Collision collision)
    {
        Vector3 best = collision.contacts[0].normal;
        float bestDot = Vector3.Dot(currentSurfaceNormal, best);

        foreach (var c in collision.contacts)
        {
            float d = Vector3.Dot(currentSurfaceNormal, c.normal);
            if (d > bestDot)
            {
                bestDot = d;
                best = c.normal;
            }
        }

        return best.normalized;
    }

    // when entering a new collider
    private void OnCollisionEnter(Collision collision)
    {
        TryUpdateSurface(collision);
    }

    //while on a collider
    private void OnCollisionStay(Collision collision)
    {
        TryUpdateSurface(collision);
    }

    //updates the normal based on the current surfaces normal
    private void TryUpdateSurface(Collision collision)
    {
        Vector3 newNormal = BestContactNormal(collision);
        float angle = Vector3.Angle(currentSurfaceNormal, newNormal);

        if (angle < normalUpdateAngleThreshold)
            return;

        currentSurfaceNormal = newNormal;
    }

}
