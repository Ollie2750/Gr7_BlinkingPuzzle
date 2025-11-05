using UnityEngine;

// Ensure a Rigidbody + Collider exist on the pillar prefab
[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class PillarDrop : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        //keep it upright, and player can push
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    //Called immediatly after spawning to wake physics so it starts falling
    public void Kick(float downImpulse = 1f)
    {
        rb.linearVelocity = UnityEngine.Vector3.zero;
        rb.angularVelocity = UnityEngine.Vector3.zero;
        rb.AddForce(UnityEngine.Vector3.down * downImpulse, ForceMode.Impulse);
    }
}
