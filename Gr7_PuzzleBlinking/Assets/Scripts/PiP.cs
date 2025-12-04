using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class PiP : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private RawImage pipDisplayImage; // Drag the RawImage here
    private ClientNetworkTransform clientNetworkTransform;

    [SerializeField] private MonoBehaviour[] localOnlyScripts; // Drag your mouse look / controller scripts here

    private RenderTexture myPipTexture;
    private bool pipSetupComplete = false;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (!IsOwner)
        {
            // This is a REMOTE player
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
            }

            DisableLocalOnlyScripts();
            return;
        }

        // This is the LOCAL player
        SetupPiP();

        // Start coroutine to keep trying until PiP is connected
        StartCoroutine(KeepTryingToConnectPiP());
    }

    private void DisableLocalOnlyScripts()
    {
        foreach (var script in localOnlyScripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        var characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }

    private void SetupPiP()
    {
        myPipTexture = new RenderTexture(512, 512, 16);
        myPipTexture.name = $"PiP_Texture_{OwnerClientId}";

        if (pipDisplayImage != null)
        {
            pipDisplayImage.texture = myPipTexture;
            Debug.Log($"Local player {OwnerClientId}: PiP texture created and assigned to UI");
        }
    }

    private IEnumerator KeepTryingToConnectPiP()
    {
        // Keep trying every 0.5 seconds until we successfully connect
        while (!pipSetupComplete)
        {
            yield return new WaitForSeconds(0.5f);
            TryConnectToRemotePlayer();
        }
    }

    private void TryConnectToRemotePlayer()
    {
        if (pipSetupComplete) return;

        // Find ALL NetworkObjects in the scene
        var allNetworkObjects = FindObjectsOfType<NetworkObject>();

        Debug.Log($"Local player {OwnerClientId}: Searching for remote player. Found {allNetworkObjects.Length} network objects");

        foreach (var netObj in allNetworkObjects)
        {
            // Skip if this is our own object
            if (netObj.OwnerClientId == OwnerClientId) continue;

            // Try to get PlayerPiP component
            PiP remotePiP = netObj.GetComponent<PiP>();
            if (remotePiP == null) continue;

            // Found a remote player!
            if (remotePiP.playerCamera != null && myPipTexture != null)
            {
                remotePiP.playerCamera.enabled = true;
                remotePiP.playerCamera.targetTexture = myPipTexture;
                remotePiP.playerCamera.depth = -10;

                pipSetupComplete = true;

                Debug.Log($"SUCCESS! Local player {OwnerClientId}: Connected to remote player {remotePiP.OwnerClientId}'s camera");
                return;
            }
        }

        Debug.Log($"Local player {OwnerClientId}: No remote player found yet, will keep trying...");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        CleanupRenderTexture();
    }

    private void CleanupRenderTexture()
    {
        if (myPipTexture != null)
        {
            myPipTexture.Release();
            Destroy(myPipTexture);
        }
    }
}