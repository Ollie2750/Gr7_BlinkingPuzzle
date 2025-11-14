using System.Collections;
using UnityEngine;

public class TimeTravelAbility : MonoBehaviour
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
    [SerializeField] private Transform oldMapCenter;  // Center of the old/past map
    [SerializeField] private Transform newMapCenter;  // Center of the new/future map

    [Header("Starting Positions")]
    [SerializeField] private bool player1StartsInOldMap = true;  // Which map Player 1 starts in
    [SerializeField] private bool player2StartsInOldMap = false; // Which map Player 2 starts in (opposite by default)

    private bool player1IsInOldMap;  // Track which map Player 1 is in
    private bool player2IsInOldMap;  // Track which map Player 2 is in
    private bool isOnCooldown = false;

    void Start()
    {
        player1IsInOldMap = player1StartsInOldMap;
        player2IsInOldMap = player2StartsInOldMap;

        Debug.Log($"Player 1 spawned in: {(player1IsInOldMap ? "Old Map" : "New Map")}");
        Debug.Log($"Player 2 spawned in: {(player2IsInOldMap ? "Old Map" : "New Map")}");
    }

    void Update()
    {
        if (Input.GetKeyDown(travelKey) && !isOnCooldown)
        {
            ActivateTimeTravel();
        }
    }

    void ActivateTimeTravel()
    {
        if (oldMapCenter == null || newMapCenter == null)
        {
            Debug.LogError("Map centers not assigned! Please set Old Map Center and New Map Center.");
            return;
        }

        if (player1Transform == null || player2Transform == null)
        {
            Debug.LogError("Player transforms not assigned!");
            return;
        }

        Debug.Log($"BEFORE: Player 1 at {player1Transform.position}, Player 2 at {player2Transform.position}");

        // ===== PLAYER 1 TELEPORTATION =====
        TeleportPlayer(
            player1Transform,
            player1Rigidbody,
            player1CharController,
            player1IsInOldMap,
            out player1IsInOldMap
        );

        // ===== PLAYER 2 TELEPORTATION =====
        TeleportPlayer(
            player2Transform,
            player2Rigidbody,
            player2CharController,
            player2IsInOldMap,
            out player2IsInOldMap
        );

        Debug.Log($"AFTER: Player 1 at {player1Transform.position}, Player 2 at {player2Transform.position}");
        Debug.Log($"Time Travel activated! Player 1 → {(player1IsInOldMap ? "Old Map" : "New Map")}, Player 2 → {(player2IsInOldMap ? "Old Map" : "New Map")}");

        // Start cooldown
        StartCoroutine(Cooldown());
    }

    void TeleportPlayer(Transform playerTransform, Rigidbody playerRigidbody, CharacterController charController, bool isInOldMap, out bool newMapStatus)
    {
        // Determine source and destination based on current location
        Transform sourceMapCenter = isInOldMap ? oldMapCenter : newMapCenter;
        Transform destMapCenter = isInOldMap ? newMapCenter : oldMapCenter;

        // ===== POSITION CALCULATION: Convert relative position between maps =====

        // Get player's offset from source map center
        Vector3 playerOffset = playerTransform.position - sourceMapCenter.position;

        // Calculate new position in destination map (same relative position)
        Vector3 newPlayerPos = destMapCenter.position + playerOffset;

        // ===== MOMENTUM PRESERVATION: Store velocity before teleport =====
        Vector3 playerVelocity = Vector3.zero;
        Vector3 playerAngularVelocity = Vector3.zero;
        bool isKinematic = false;

        if (playerRigidbody != null)
        {
            isKinematic = playerRigidbody.isKinematic;

            // Only store velocity if not kinematic
            if (!isKinematic)
            {
                playerVelocity = playerRigidbody.linearVelocity;
                playerAngularVelocity = playerRigidbody.angularVelocity;
            }
        }

        // ===== TELEPORTATION: Move player to destination map =====

        // If using Character Controller, disable it temporarily for teleport
        if (charController != null && charController.enabled)
        {
            charController.enabled = false;
            playerTransform.position = newPlayerPos;
            charController.enabled = true;
        }
        else
        {
            // Standard transform teleport
            playerTransform.position = newPlayerPos;
        }

        // ===== MOMENTUM RESTORATION: Reapply velocity after teleport =====
        if (playerRigidbody != null && !isKinematic)
        {
            // Only restore velocity if not kinematic
            playerRigidbody.linearVelocity = playerVelocity;
            playerRigidbody.angularVelocity = playerAngularVelocity;
        }

        // Toggle which map the player is in
        newMapStatus = !isInOldMap;
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
        if (oldMapCenter != null)
        {
            Gizmos.color = new Color(0.6f, 0.4f, 0.2f, 0.5f); // Brown for old map
            Gizmos.DrawWireSphere(oldMapCenter.position, 2f);
            Gizmos.DrawWireCube(oldMapCenter.position, new Vector3(20f, 0.5f, 20f));
        }

        if (newMapCenter != null)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.5f); // Cyan for new map
            Gizmos.DrawWireSphere(newMapCenter.position, 2f);
            Gizmos.DrawWireCube(newMapCenter.position, new Vector3(20f, 0.5f, 20f));
        }

        // Draw arrow between maps
        if (oldMapCenter != null && newMapCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(oldMapCenter.position, newMapCenter.position);
        }

        // Show starting positions
        if (player1Transform != null)
        {
            Transform startMap = player1StartsInOldMap ? oldMapCenter : newMapCenter;
            if (startMap != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(player1Transform.position, startMap.position);
            }
        }

        if (player2Transform != null)
        {
            Transform startMap = player2StartsInOldMap ? oldMapCenter : newMapCenter;
            if (startMap != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(player2Transform.position, startMap.position);
            }
        }
    }
}