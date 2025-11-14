using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TimeTravelAbility : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode travelKey = KeyCode.Q;
    [SerializeField] private float cooldownDuration = 5f;

    [Header("Both Player References")]
    [SerializeField] private Transform player1Transform;  // Player with time travel ability
    [SerializeField] private Transform player2Transform;  // Player with time stop ability
    [SerializeField] private Rigidbody player1Rigidbody;  // Optional: for preserving momentum
    [SerializeField] private Rigidbody player2Rigidbody;  // Optional: for preserving momentum
    [SerializeField] private CharacterController player1CharController;  // If using Character Controller
    [SerializeField] private CharacterController player2CharController;  // If using Character Controller

    [Header("Map Location Settings")]
    [SerializeField] private Vector3 oldMapCenter;  // Center position of the old/past map
    [SerializeField] private Vector3 newMapCenter;  // Center position of the new/future map

    [Header("Starting Positions")]
    [SerializeField] private bool player1StartsInOldMap = true;  // Which map Player 1 starts in
    [SerializeField] private bool player2StartsInOldMap = false; // Which map Player 2 starts in (opposite by default)

    private bool player1IsInOldMap;  // Track which map Player 1 is in
    private bool player2IsInOldMap;  // Track which map Player 2 is in
    private bool isOnCooldown = false;

    private bool player2LocationFixed = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        player1IsInOldMap = player1StartsInOldMap;
        player2IsInOldMap = player2StartsInOldMap;

        Debug.Log($"Player 1 spawned in: {(player1IsInOldMap ? "Old Map" : "New Map")}");
        Debug.Log($"Player 2 spawned in: {(player2IsInOldMap ? "Old Map" : "New Map")}");

        player2Transform = transform;
        player2Rigidbody = GetComponent<Rigidbody>();
        player2CharController = GetComponent<CharacterController>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            var clientNetTransform = player.GetComponent<ClientNetworkTransform>();
            if (clientNetTransform != null && clientNetTransform.IsOwnedByServer)
            {
                player1Transform = player.transform;
                player1Rigidbody = player.GetComponent<Rigidbody>();
                player1CharController = player.GetComponent<CharacterController>();
                return;
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Only the owner of this script (Player 2) can activate

        if (!player2LocationFixed)
        {
            Vector3 newPos = CalculateNewPosition(
                player2Transform.position,
                player2IsInOldMap,
                out bool newMapStatus
            );

            // Request server to move Player 2 to starting position
            RequestTeleportServerRpc(NetworkObjectId, newPos, Vector3.zero, Vector3.zero, newMapStatus);
            player2LocationFixed = true;
        }

        if (Input.GetKeyDown(travelKey) && !isOnCooldown)
        {
            ActivateTimeTravel();
        }
    }

    void ActivateTimeTravel()
    {
        if (!IsOwner) return;

        if (player1Transform == null || player2Transform == null)
        {
            Debug.LogError("Player transforms not assigned!");
            return;
        }

        Debug.Log($"BEFORE: Player 1 at {player1Transform.position}, Player 2 at {player2Transform.position}");

        // ===== PLAYER 1 TELEPORTATION =====
        Vector3 player1NewPos = CalculateNewPosition(
            player1Transform.position,
            player1IsInOldMap,
            out bool player1NewMapStatus
        );

        Vector3 player1Velocity = Vector3.zero;
        Vector3 player1AngularVel = Vector3.zero;

        if (player1Rigidbody != null && !player1Rigidbody.isKinematic)
        {
            player1Velocity = player1Rigidbody.linearVelocity;
            player1AngularVel = player1Rigidbody.angularVelocity;
        }

        // ===== PLAYER 2 TELEPORTATION =====
        Vector3 player2NewPos = CalculateNewPosition(
            player2Transform.position,
            player2IsInOldMap,
            out bool player2NewMapStatus
        );

        Vector3 player2Velocity = Vector3.zero;
        Vector3 player2AngularVel = Vector3.zero;

        if (player2Rigidbody != null && !player2Rigidbody.isKinematic)
        {
            player2Velocity = player2Rigidbody.linearVelocity;
            player2AngularVel = player2Rigidbody.angularVelocity;
        }

        // Request server to teleport both players
        var player1NetObj = player1Transform.GetComponent<NetworkObject>();
        if (player1NetObj != null)
        {
            RequestTeleportServerRpc(player1NetObj.NetworkObjectId, player1NewPos, player1Velocity, player1AngularVel, player1NewMapStatus);
        }

        RequestTeleportServerRpc(NetworkObjectId, player2NewPos, player2Velocity, player2AngularVel, player2NewMapStatus);

        Debug.Log($"AFTER: Player 1 at {player1NewPos}, Player 2 at {player2NewPos}");
        Debug.Log($"Time Travel activated! Player 1 → {(player1NewMapStatus ? "Old Map" : "New Map")}, Player 2 → {(player2NewMapStatus ? "Old Map" : "New Map")}");

        // Start cooldown
        StartCoroutine(Cooldown());
    }

    Vector3 CalculateNewPosition(Vector3 currentPos, bool isInOldMap, out bool newMapStatus)
    {
        // Determine source and destination based on current location
        Vector3 sourceMapCenter = isInOldMap ? oldMapCenter : newMapCenter;
        Vector3 destMapCenter = isInOldMap ? newMapCenter : oldMapCenter;

        // Get player's offset from source map center
        Vector3 playerOffset = currentPos - sourceMapCenter;

        // Calculate new position in destination map (same relative position)
        Vector3 newPlayerPos = destMapCenter + playerOffset;

        // Toggle which map the player is in
        newMapStatus = !isInOldMap;

        return newPlayerPos;
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestTeleportServerRpc(ulong networkObjectId, Vector3 newPosition, Vector3 velocity, Vector3 angularVelocity, bool newMapStatus)
    {
        // Server executes the teleport for the specified player
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            Transform playerTransform = netObj.transform;
            Rigidbody playerRigidbody = netObj.GetComponent<Rigidbody>();
            CharacterController charController = netObj.GetComponent<CharacterController>();

            // Teleport the player
            if (charController != null && charController.enabled)
            {
                charController.enabled = false;
                playerTransform.position = newPosition;
                charController.enabled = true;
            }
            else
            {
                playerTransform.position = newPosition;
            }

            // Restore momentum
            if (playerRigidbody != null && !playerRigidbody.isKinematic)
            {
                playerRigidbody.linearVelocity = velocity;
                playerRigidbody.angularVelocity = angularVelocity;
            }

            // Update map status on all clients
            UpdateMapStatusClientRpc(networkObjectId, newMapStatus);
        }
    }

    [ClientRpc]
    void UpdateMapStatusClientRpc(ulong networkObjectId, bool newMapStatus)
    {
        // Update the map status for the correct player
        if (networkObjectId == NetworkObjectId)
        {
            player2IsInOldMap = newMapStatus;
        }
        else
        {
            player1IsInOldMap = newMapStatus;
        }
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
        Debug.Log("Time travel ability ready!");
    }

    // Get current map status for each player
    public bool IsPlayer1InOldMap()
    {
        return player1IsInOldMap;
    }

    public bool IsPlayer2InOldMap()
    {
        return player2IsInOldMap;
    }

    public string GetPlayer1MapName()
    {
        return player1IsInOldMap ? "Old Map (Past)" : "New Map (Future)";
    }

    public string GetPlayer2MapName()
    {
        return player2IsInOldMap ? "Old Map (Past)" : "New Map (Future)";
    }

    // Optional: Get cooldown progress for UI
    public float GetCooldownProgress()
    {
        return isOnCooldown ? 0f : 1f;
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    // Visualize the two map centers in the editor
    void OnDrawGizmos()
    {
        // Draw old map center
        Gizmos.color = new Color(0.6f, 0.4f, 0.2f, 0.5f); // Brown for old map
        Gizmos.DrawWireSphere(oldMapCenter, 2f);
        Gizmos.DrawWireCube(oldMapCenter, new Vector3(20f, 0.5f, 20f));

        // Draw new map center
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.5f); // Cyan for new map
        Gizmos.DrawWireSphere(newMapCenter, 2f);
        Gizmos.DrawWireCube(newMapCenter, new Vector3(20f, 0.5f, 20f));

        // Draw arrow between maps
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(oldMapCenter, newMapCenter);

        // Show starting positions
        if (player1Transform != null)
        {
            Vector3 startMap = player1StartsInOldMap ? oldMapCenter : newMapCenter;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(player1Transform.position, startMap);
        }

        if (player2Transform != null)
        {
            Vector3 startMap = player2StartsInOldMap ? oldMapCenter : newMapCenter;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(player2Transform.position, startMap);
        }
    }
}