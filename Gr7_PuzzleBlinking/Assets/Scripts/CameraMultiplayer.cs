using UnityEngine;
using Unity.Netcode;
using UnityEditor;

public class CameraMultiplayer : MonoBehaviour
{
    [SerializeField] private GameObject myCam;
    private ClientNetworkTransform myNetworkTransform;

    void Awake()
    {
        myNetworkTransform = GetComponent<ClientNetworkTransform>();
    }

    void FixedUpdate()
    {
        ulong clientId = gameObject.GetComponent<NetworkObject>().OwnerClientId;
        gameObject.GetComponent<NetworkObject>().ChangeOwnership(clientId);

        if (!myNetworkTransform.IsOwner) return;

        if (!myCam.GetComponent<Camera>().enabled)
        {
            myCam.GetComponent<Camera>().enabled = true;
        }

        if(myCam.GetComponent<AudioListener>().enabled == false)
        {
            myCam.GetComponent<AudioListener>().enabled = true;
        }
        
    }
}
