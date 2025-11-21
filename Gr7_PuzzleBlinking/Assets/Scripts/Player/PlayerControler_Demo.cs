using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerControler_Demo : MonoBehaviour

{
    private Vector2 moveInput2D;
    private Vector3 moveInput;
    private Rigidbody rb;

    public float moveSpeed = 5f;

    public float velocity = 10f;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue context)
    {
        moveInput2D = context.Get<Vector2>();

        moveInput = new Vector3(moveInput2D.x, 0, moveInput2D.y);

    }
    private void FixedUpdate()
    {
        // Apply movement in FixedUpdate for physics-based movement
        Vector3 movement = moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    

}
