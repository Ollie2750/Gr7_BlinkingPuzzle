using System.Data;
using UnityEngine;

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

    //Called immediatly after spawning to wake physics
    public void Kick(float downImpulse = 1f)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.down * downImpulse, ForceMode.Impulse);
    }
}
