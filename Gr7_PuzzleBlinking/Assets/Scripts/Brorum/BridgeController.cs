using System.Collections;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] private Rigidbody bridgeRb;

    [SerializeField] private float upForce;

    [SerializeField] private float buttomHeight;

    private float volume = 0.1f;
    [SerializeField] private AudioClip bridgeSound;


    public void bounceBridge()
    {
        if (bridgeRb.transform.position.y < buttomHeight)
        {
            SoundManager.Instance.PlaySoundClip(bridgeSound, transform, volume);
            bridgeRb.AddForce(new Vector3(0, upForce, 0));
        }
        
    }
}
