using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TimeTravel : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode travelKey = KeyCode.Q;
    [SerializeField] private float cooldownDuration = 5f;

    [Header("Both Player References")]
    [SerializeField] private Transform player1Transform;
    [SerializeField] private Transform player2Transform;
    [SerializeField] private Rigidbody player1Rigidbody;
    [SerializeField] private Rigidbody player2Rigidbody;
    [SerializeField] private CharacterController player1CharController;
    [SerializeField] private CharacterController player2CharController;

    [Header("Map Location Settings")]
    [SerializeField] private Vector3 oldMapCenter;
    [SerializeField] private Vector3 newMapCenter;

    [Header("Starting Positions")]
    [SerializeField] private bool player1StartsInOldMap = true;
    [SerializeField] private bool player2StartsInOldMap = false;

    private bool player1IsInOldMap;
    private bool player2IsInOldMap;
    private bool player2LocationFixed = false;
    private bool isOnCooldown = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        player1IsInOldMap = player1StartsInOldMap;
        player2IsInOldMap = player2StartsInOldMap;

        // Player 2 (owner)
        player2Transform = transform;
        player2Rigidbody = GetComponent<Rigidbody>();
        player2CharController = GetComponent<CharacterController>();

        // Locate Player 1 (server-owned)
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            var netTransform = player.GetComponent<ClientNetworkTransform>();
            if (netTransform != null && netTransform.IsOwnedByServer)
            {
                player1Transform = player.transform;
                player1Rigidbody = player.GetComponent<Rigidbody>();
                player1CharController = player.GetComponent<CharacterController>();
                break;
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Fix Player 2 start position once
        if (!player2LocationFixed)
        {
            // Raise Player 2 slightly to prevent spawning below map
            float spawnYOffset = 1f;

            Vector3 startPos = (player2IsInOldMap ? oldMapCenter : newMapCenter)
                               + new Vector3(2, spawnYOffset, 5);

            // Teleport WITHOUT offset logic
            LocalTeleport(player2Transform, player2CharController, player2Rigidbody, startPos, Vector3.zero, Vector3.zero);

            UpdateMapStatusClientRpc(NetworkObjectId, player2IsInOldMap);
            player2LocationFixed = true;
        }

        if (Input.GetKeyDown(travelKey))
        {
            ActivateTimeTravel();
        }
    }

    public void ActivateTimeTravel()
    {
        if (!IsOwner) return;
        if (!isOnCooldown) return;

        Debug.Log("///////////////////// TIME TRAVEL USED ///////////////////////");

        // ==== PLAYER 1 (SERVER TELEPORTED) ====
        Vector3 player1NewPos = CalculateNewPosition(player1Transform.position, player1IsInOldMap, out bool player1NewMapStatus);

        Vector3 p1Vel = Vector3.zero;
        Vector3 p1AngVel = Vector3.zero;
        if (player1Rigidbody != null && !player1Rigidbody.isKinematic)
        {
            p1Vel = player1Rigidbody.linearVelocity;
            p1AngVel = player1Rigidbody.angularVelocity;
        }

        var player1NetObj = player1Transform.GetComponent<NetworkObject>();
        if (player1NetObj != null)
        {
            RequestTeleportServerRpc(player1NetObj.NetworkObjectId, player1NewPos, p1Vel, p1AngVel, player1NewMapStatus);
        }

        // ==== PLAYER 2 (LOCAL TELEPORT) ====
        Vector3 player2NewPos = CalculateNewPosition(player2Transform.position, player2IsInOldMap, out bool player2NewMapStatus);

        Vector3 p2Vel = Vector3.zero;
        Vector3 p2AngVel = Vector3.zero;
        if (player2Rigidbody != null && !player2Rigidbody.isKinematic)
        {
            p2Vel = player2Rigidbody.linearVelocity;
            p2AngVel = player2Rigidbody.angularVelocity;
        }

        LocalTeleport(player2Transform, player2CharController, player2Rigidbody, player2NewPos, p2Vel, p2AngVel);
        player2IsInOldMap = player2NewMapStatus;

        UpdateMapStatusClientRpc(NetworkObjectId, player2NewMapStatus);

        StartCoroutine(Cooldown());
    }

    // ===== LOCAL TELEPORT =====
    void LocalTeleport(Transform target, CharacterController controller, Rigidbody rigid, Vector3 pos, Vector3 vel, Vector3 angVel)
    {
        if (controller != null && controller.enabled)
        {
            controller.enabled = false;
            target.position = pos;
            controller.enabled = true;
        }
        else
        {
            target.position = pos;
        }

        if (rigid != null && !rigid.isKinematic)
        {
            rigid.linearVelocity = vel;
            rigid.angularVelocity = angVel;
        }
    }

    // ===== SERVER TELEPORT =====
    [ServerRpc(RequireOwnership = false)]
    void RequestTeleportServerRpc(ulong id, Vector3 pos, Vector3 vel, Vector3 angVel, bool newStatus)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out NetworkObject obj))
        {
            Transform t = obj.transform;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            CharacterController cc = obj.GetComponent<CharacterController>();

            if (cc != null && cc.enabled)
            {
                cc.enabled = false;
                t.position = pos;
                cc.enabled = true;
            }
            else
            {
                t.position = pos;
            }

            if (rb != null && !rb.isKinematic)
            {
                rb.linearVelocity = vel;
                rb.angularVelocity = angVel;
            }

            UpdateMapStatusClientRpc(id, newStatus);
        }
    }

    [ClientRpc]
    void UpdateMapStatusClientRpc(ulong id, bool newStatus)
    {
        if (id == NetworkObjectId)
            player2IsInOldMap = newStatus;
        else
            player1IsInOldMap = newStatus;
    }

    Vector3 CalculateNewPosition(Vector3 current, bool isOld, out bool newStatus)
    {
        Vector3 source = isOld ? oldMapCenter : newMapCenter;
        Vector3 dest = isOld ? newMapCenter : oldMapCenter;

        Vector3 offset = current - source;
        newStatus = !isOld;

        return dest + offset;
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }
}
