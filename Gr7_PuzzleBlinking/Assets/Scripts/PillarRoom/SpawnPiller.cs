using Unity.Netcode;
using UnityEngine;

public class SpawnPiller : NetworkBehaviour
{
    [SerializeField] private GameObject piller;
    [SerializeField] private Vector3 spawnPosition;

    public void Interact()
    {
        if (!IsServer)
        {
            // Client requests the server to move the pillar
            RequestMovePillarServerRpc();
        }
        else
        {
            // Server moves the pillar directly
            MovePillar();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestMovePillarServerRpc()
    {
        // Server executes the movement
        MovePillar();
    }

    private void MovePillar()
    {
        // Move the pillar locally on the server
        piller.transform.localPosition = spawnPosition;

        // Notify all clients to update their pillar position
        UpdatePillarClientRpc(spawnPosition);
    }

    [ClientRpc]
    private void UpdatePillarClientRpc(Vector3 newPosition)
    {
        if (!IsServer)
        {
            // Only clients update (server already moved it)
            piller.transform.localPosition = newPosition;
        }
    }
}