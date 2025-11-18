using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEditor;

public class PlayerMovementMultiplayer : MonoBehaviour
{
    private CharacterController controller;
    private InputSystem_Actions inputActions;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float interactDistance = 3;
    private Vector3 velocity;
    private Vector2 moveInput;

    [SerializeField] private float lookSensitivity = 0.25f;

    [SerializeField] private Transform playerCamera;
    private float xRotation = 0f;
    private bool isCrouching = false;
    private Interactable currentInteractable;

    private ClientNetworkTransform _transform;
    private bool isOwner;

    void Awake()
    {
        _transform = gameObject.GetComponent<ClientNetworkTransform>();

        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        currentSpeed = moveSpeed;
    }

    private void Start()
    {
        isOwner = _transform.IsOwner;
    }

    private void FixedUpdate()
    {
        if (!isOwner) return;
        Move();
        ApplyGravity();
        HandleHover();
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
        inputActions.Player.Jump.performed += _ => Jump();
        inputActions.Player.Sprint.performed += _ => Sprint(true);
        inputActions.Player.Sprint.canceled += _ => Sprint(false);
        inputActions.Player.Interact.performed += _ => Interaction();
    }

}
