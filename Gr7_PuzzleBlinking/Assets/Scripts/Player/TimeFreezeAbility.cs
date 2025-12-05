using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TimeFreezeAbility : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode freezeKey = KeyCode.Q;
    [SerializeField] private float freezeDuration = 5f;
    [SerializeField] private float cooldownDuration = 5f;

    private bool isOnCooldown = false;
    private bool isFreezeActive = false;
    private float cooldownTimer = 0f;
    private List<RigidbodyState> frozenRigidbodies = new List<RigidbodyState>();

    // Struct to store rigidbody state
    private struct RigidbodyState
    {
        public Rigidbody rb;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public bool wasKinematic;
    }

    void Update()
    {
        if (Input.GetKeyDown(freezeKey))
        {
            ActivateFreeze();
        }
    }

    public void ActivateFreeze()
    {
        if (!IsOwner) return;

        if (!isOnCooldown && !isFreezeActive)
        {
            StartCoroutine(FreezeTime());
            StartCoroutine(Cooldown()); // Start cooldown immediately!
        }
    }

    IEnumerator FreezeTime()
    {
        isFreezeActive = true;

        // Find and freeze all rigidbodies except the player
        FreezeAllRigidbodies();

        // Wait for freeze duration
        yield return new WaitForSeconds(freezeDuration);

        // Unfreeze all rigidbodies
        UnfreezeAllRigidbodies();

        isFreezeActive = false;
        
        // Cooldown is already running separately
    }

    void FreezeAllRigidbodies()
    {
        if (!IsOwner) return;

        frozenRigidbodies.Clear();

        // Find all rigidbodies in the scene (using newer Unity API)
        Rigidbody[] allRigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        foreach (Rigidbody rb in allRigidbodies)
        {
            // Skip the player's rigidbody
            if (rb.gameObject.tag == "Player")
                continue;

            // ===== MOMENTUM STORAGE: Save current velocities before freezing =====
            RigidbodyState state = new RigidbodyState
            {
                rb = rb,
                velocity = rb.linearVelocity,
                angularVelocity = rb.angularVelocity,
                wasKinematic = rb.isKinematic
            };

            frozenRigidbodies.Add(state);

            // ===== FREEZE: Stop all movement =====
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Debug.Log($"Froze {frozenRigidbodies.Count} rigidbodies for {freezeDuration} seconds");
    }

    void UnfreezeAllRigidbodies()
    {
        if (!IsOwner) return;

        foreach (RigidbodyState state in frozenRigidbodies)
        {
            if (state.rb != null)
            {
                // ===== MOMENTUM RESTORATION: Reapply stored velocities =====
                state.rb.isKinematic = state.wasKinematic;
                state.rb.linearVelocity = state.velocity;
                state.rb.angularVelocity = state.angularVelocity;
            }
        }

        frozenRigidbodies.Clear();
        Debug.Log("Unfroze all rigidbodies");
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        float totalDuration = cooldownDuration + freezeDuration; // Total cooldown time
        cooldownTimer = totalDuration;
        
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        
        cooldownTimer = 0f;
        isOnCooldown = false;
        Debug.Log("Time freeze ability ready!");
    }

    // Get cooldown progress for UI (0 = ready, >0 = on cooldown)
    public float GetCooldownProgress()
    {
        if (!isOnCooldown) return 0f;
        float totalDuration = cooldownDuration + freezeDuration;
        return Mathf.Clamp01(cooldownTimer / totalDuration); // Divide by total time
    }

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public bool IsFreezeActive()
    {
        return isFreezeActive;
    }
}