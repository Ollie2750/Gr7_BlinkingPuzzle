using UnityEngine;

public class PlayerPusher : MonoBehaviour
{
  [Tooltip("How strong the shove is.")]
    public float pushPower = 3.5f;

    [Tooltip("Only push along the ground, not upward.")]
    public bool horizontalOnly = true;

  // Only runs if your player uses a CharacterController.
  // It adds an impulse to any non-kinematic Rigidbody we collide with.
  void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.rigidbody;
        if (!rb || rb.isKinematic) return;

        // Direction is your move direction; use hit.moveDirection if you like
        Vector3 force = hit.moveDirection;
        if (horizontalOnly) force.y = 0f;

        rb.AddForce(force.normalized * pushPower, ForceMode.Impulse);
    }
}
