using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovementScript : MonoBehaviour
{
    private CharacterController controller;
    private InputSystem_Actions inputActions;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float airControlMultiplier = 0.5f;
    [SerializeField] private float currentSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float headBumpCheckDistance = 0.1f;

    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivity = 0.25f;
    [SerializeField] private Transform playerCamera;

    [Header("Interaction")]
    [SerializeField] private float interactDistance = 3;

    private Vector3 velocity;
    private Vector2 moveInput;
    private float xRotation = 0f;
    private bool isCrouching = false;
    private Interactable currentInteractable;

    // Coyote time variables
    private float coyoteTimeCounter;
    private bool wasGroundedLastFrame;

    // Jump buffer variables
    private float jumpBufferCounter;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        currentSpeed = moveSpeed;
        standingHeight = controller.height;
    }

    private void Update()
    {
        UpdateCoyoteTime();
        UpdateJumpBuffer();
        HandleJump();
    }

    private void FixedUpdate()
    {
        Move();
        ApplyGravity();
        HandleHover();
        HandleCrouchTransition();
    }

    private void UpdateCoyoteTime()
    {
        if (controller.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            wasGroundedLastFrame = true;
        }
        else
        {
            if (wasGroundedLastFrame)
            {
                coyoteTimeCounter = coyoteTime;
                wasGroundedLastFrame = false;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }
    }

    private void UpdateJumpBuffer()
    {
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        // Check if we should jump (either from buffer or coyote time)
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            PerformJump();
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }
    }

    private void Move()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        // Apply air control if not grounded
        float speedMultiplier = controller.isGrounded ? 1f : airControlMultiplier;
        
        controller.Move(moveDirection * currentSpeed * speedMultiplier * Time.deltaTime);
    }

    private void Look(Vector2 input)
    {
        if (Time.timeScale == 0) return;
        float mouseX = input.x * lookSensitivity;
        float mouseY = input.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Check for head bump and stop upward velocity
        if (velocity.y > 0 && CheckHeadBump())
        {
            velocity.y = 0f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool CheckHeadBump()
    {
        // Cast a ray upward from the top of the character controller
        Vector3 rayStart = transform.position + Vector3.up * (controller.height / 2);
        float rayDistance = headBumpCheckDistance;

        // Debug visualization
        Debug.DrawRay(rayStart, Vector3.up * rayDistance, Color.blue);

        return Physics.Raycast(rayStart, Vector3.up, rayDistance);
    }

    private void PerformJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void OnJumpInput()
    {
        // Set jump buffer when jump is pressed
        jumpBufferCounter = jumpBufferTime;
    }

    private void Sprint(bool sprint)
    {
        if (isCrouching || !controller.isGrounded) return;
        currentSpeed = sprint ? sprintSpeed : moveSpeed;
    }

    private void Crouch(bool crouch)
    {
        isCrouching = crouch;
        
        // Update speed based on crouch state
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            // Check if there's enough space to stand up
            if (CanStandUp())
            {
                currentSpeed = moveSpeed;
            }
            else
            {
                // Force crouch if can't stand up
                isCrouching = true;
            }
        }
    }

    private bool CanStandUp()
    {
        // Raycast upward to check if there's space to stand
        float checkDistance = standingHeight - crouchHeight;
        Vector3 rayStart = transform.position + Vector3.up * (controller.height / 2);
        
        return !Physics.Raycast(rayStart, Vector3.up, checkDistance);
    }

    private void HandleCrouchTransition()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        
        if (Mathf.Abs(controller.height - targetHeight) > 0.01f)
        {
            float previousHeight = controller.height;
            controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
            
            // Adjust position to keep feet on ground
            float heightDifference = controller.height - previousHeight;
            controller.Move(Vector3.up * (heightDifference / 2));
        }
    }

    private void HandleHover()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(playerCamera.position, playerCamera.forward * interactDistance, Color.red);

        Interactable newInteractable = null;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            newInteractable = hit.collider.GetComponent<Interactable>();
        }

        if (currentInteractable != null && currentInteractable.Equals(null))
        {
            currentInteractable = null;
        }

        if (newInteractable != currentInteractable)
        {
            if (currentInteractable != null && !currentInteractable.Equals(null))
                currentInteractable.OnHoverExit();

            currentInteractable = newInteractable;

            if (currentInteractable != null)
                currentInteractable.OnHoverEnter();
        }
    }

    private void Interaction()
    {
        Debug.Log("Interact pressed");
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        inputActions.Player.Jump.performed += _ => OnJumpInput();
        inputActions.Player.Sprint.performed += _ => Sprint(true);
        inputActions.Player.Sprint.canceled += _ => Sprint(false);
        inputActions.Player.Interact.performed += _ => Interaction();
        inputActions.Player.Crouch.performed += _ => Crouch(true);
        inputActions.Player.Crouch.canceled += _ => Crouch(false);
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
