using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    private CharacterController controller;
    private InputSystem_Actions inputActions;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float currentSpeed;
    private Vector3 velocity;
    private Vector2 moveInput;

    [SerializeField] private float lookSensitivity = 0.25f;

    [SerializeField] private Transform playerCamera;
    private float xRotation = 0f;
    private bool isCrouching = false;
    
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
        ApplyGravity();
    }

    private void Move()
    {
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
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
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Sprint(bool sprint)
    {
        if (isCrouching || !controller.isGrounded) return;
        currentSpeed = sprint ? sprintSpeed : moveSpeed;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions.Player.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        inputActions.Player.Jump.performed += _ => Jump();
        inputActions.Player.Sprint.performed += _ => Sprint(true);
        inputActions.Player.Sprint.canceled += _ => Sprint(false);
    }
}
