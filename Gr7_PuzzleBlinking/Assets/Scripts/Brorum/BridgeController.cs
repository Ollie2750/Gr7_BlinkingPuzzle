using System.Collections;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] private Rigidbody bridgeRb;

    [SerializeField] private float upForce;

    [SerializeField] private float buttomHeight;


    public void bounceBridge()
    {
        if (bridgeRb.transform.position.y < buttomHeight)
        {
            bridgeRb.AddForce(new Vector3(0, upForce, 0));
        }
        
    }
}
