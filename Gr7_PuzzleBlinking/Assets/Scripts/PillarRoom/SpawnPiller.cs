using UnityEngine;
using Unity.Netcode;

public class SpawnPiller : Interactable
{
    [SerializeField] private GameObject piller;
    [SerializeField] private Vector3 spawnPosition;

    public override void Interact()
    {
        // Request the server to move the pillar
        if (NetworkManager.Singleton != null)
        {
            // Get the NetworkObject component from the pillar
            NetworkObject pillerNetworkObject = piller.GetComponent<NetworkObject>();

            if (pillerNetworkObject != null)
            {
                // Call the server RPC to move the pillar
                RequestMovePillerServerRpc(pillerNetworkObject.NetworkObjectId);
            }
            else
            {
                Debug.LogError("Pillar does not have a NetworkObject component!");
            }
        }
        else
        {
            // Fallback for single-player or testing
            piller.transform.localPosition = spawnPosition;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestMovePillerServerRpc(ulong pillerNetworkId)
    {
        // Find the pillar's NetworkObject
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(pillerNetworkId, out NetworkObject pillerNetObj))
        {
            // Move the pillar on the server
            pillerNetObj.transform.localPosition = spawnPosition;
        }
    }
}