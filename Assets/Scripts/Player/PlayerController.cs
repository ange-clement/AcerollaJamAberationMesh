using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input system
    public InputActionAsset actions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction lookActionController;
    private InputAction nextToolAction;
    private InputAction prevToolAction;

    public float playerSpeed = 10.0f;
    public float jumpHeight = 2.0f;
    public float groundDrag = 0.1f;
    public float lookSens = 10f;
    public float lookSensController = 100f;

    public float coyoteTime = 0.3f;
    public float firstFrameJumpTime = 0.2f;

    public bool grounded;
    public bool canJump;
    public Vector3 playerVelocity = Vector3.zero;
    public float gravityValue = -20f;
    public Vector3 gravityVector = new Vector3(0f, 1f, 0f);
    public bool applyGravity = true;


    public Transform lookObject;
    public Transform shootOrigin;
    public InteractionManager interactionManager;

    private CharacterController controller;

    private float coyoteTimeLeft = 0.0f;
    private float firstFrameJumpTimeLeft = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Input handling
        moveAction = actions.FindActionMap("Gameplay").FindAction("Move");
        lookAction = actions.FindActionMap("Gameplay").FindAction("Look");
        lookActionController = actions.FindActionMap("Gameplay").FindAction("LookController");
        nextToolAction = actions.FindActionMap("Gameplay").FindAction("NextTool");
        prevToolAction = actions.FindActionMap("Gameplay").FindAction("PrevTool");

        actions.FindActionMap("Gameplay").FindAction("Jump").performed += OnJump;
        actions.FindActionMap("Gameplay").FindAction("ActOn").performed += OnActOn;
        actions.FindActionMap("Gameplay").FindAction("ActOff").performed += OnActOff;

        actions.FindActionMap("Gameplay").FindAction("SetTool1").performed += OnSetTool1;
        actions.FindActionMap("Gameplay").FindAction("SetTool2").performed += OnSetTool2;

        // components
        controller = GetComponent<CharacterController>();

        lookObject.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Process input
        if (nextToolAction.ReadValue<float>() != 0)
        {
            interactionManager.NextTool();
        }
        if (prevToolAction.ReadValue<float>() != 0)
        {
            interactionManager.PreviousTool();
        }
        Vector2 lookVectorInputController = lookActionController.ReadValue<Vector2>();
        Vector2 lookVectorInputKeyboard = lookAction.ReadValue<Vector2>();
        Vector2 lookVectorInput;
        if (lookVectorInputController.sqrMagnitude > lookVectorInputKeyboard.sqrMagnitude)
        {
            lookVectorInput = lookVectorInputController * lookSensController * Time.deltaTime;
        }
        else
        {
            lookVectorInput = lookVectorInputKeyboard * lookSens;
        }
        transform.Rotate(Vector3.up, lookVectorInput.x);

        float pitchAngle = lookObject.localEulerAngles.x;
        if (pitchAngle > 180)
            pitchAngle -= 360;

        pitchAngle = Mathf.Clamp(pitchAngle - lookVectorInput.y, -90.0f, 90.0f);

        lookObject.localRotation = Quaternion.Euler(pitchAngle, 0f, 0f);


        Vector2 moveVectorInput = moveAction.ReadValue<Vector2>() * playerSpeed;
        Vector3 moveVector = transform.forward * moveVectorInput.y + transform.right * moveVectorInput.x;


        // Process state
        grounded = controller.isGrounded;

        if (!applyGravity || (grounded && playerVelocity.y < 0))
        {
            playerVelocity.y = 0f;
        }
        
        if (applyGravity && !grounded)
        {
            playerVelocity += gravityVector * (gravityValue * Time.deltaTime);
        }

        if (grounded)
        {
            playerVelocity = playerVelocity - playerVelocity * groundDrag;

            float minVelToStop = 0.1f;
            if (playerVelocity.x * playerVelocity.x + playerVelocity.z * playerVelocity.z < minVelToStop)
            {
                playerVelocity.x = 0;
                playerVelocity.z = 0;
            }
        }

        // Apply speed (with collision)
        CollisionFlags collisionFlags = controller.Move((playerVelocity + moveVector) * Time.deltaTime);
        if (collisionFlags != CollisionFlags.None)
        {
            OnCollision(collisionFlags);
        }

        if (Vector3.Dot(playerVelocity, moveVector) < 0)
        {
            playerVelocity += moveVector * Time.deltaTime;
        }

        // Update cooldowns
        if (grounded)
        {
            canJump = true;
            coyoteTimeLeft = coyoteTime;
            if (firstFrameJumpTimeLeft > 0)
            {
                OnJump();
                firstFrameJumpTimeLeft = 0;
            }
        }
        else
        {
            coyoteTimeLeft -= Time.deltaTime;
            firstFrameJumpTimeLeft -= Time.deltaTime;
        }

        if (coyoteTimeLeft < 0)
        {
            canJump = false;
        }
    }

    private void OnJump(InputAction.CallbackContext ignored)
    {
        OnJump();
    }
    private void OnJump()
    {
        firstFrameJumpTimeLeft = firstFrameJumpTime;
        if (canJump)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            canJump = false;
        }
    }

    private void OnActOn(InputAction.CallbackContext context)
    {
        interactionManager.startPos = shootOrigin;
        //interactionManager.gameObject.SetActive(true);
        interactionManager.enabled = true;
    }
    private void OnActOff(InputAction.CallbackContext context)
    {
        //interactionManager.gameObject.SetActive(false);
        interactionManager.enabled = false;
    }

    private void OnSetTool1(InputAction.CallbackContext context)
    {
        interactionManager.SwitchTool(0);
    }
    private void OnSetTool2(InputAction.CallbackContext context)
    {
        interactionManager.SwitchTool(1);
    }

    private void OnCollision(CollisionFlags flags)
    {

    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Player : collision with " + collision.gameObject.name);
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Player : trigger with " + other.gameObject.name);
    //}
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    Debug.Log("Player : Controller collision with " + hit.gameObject.name);
    //}

    void OnEnable()
    {
        actions.FindActionMap("Gameplay").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("Gameplay").Disable();
    }
}
