using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class TimeFreezeAbility : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode freezeKey = KeyCode.Space;
    [SerializeField] private float freezeDuration = 5f;
    [SerializeField] private float cooldownDuration = 5f;

    private bool isOnCooldown = false;
    private bool isFreezeActive = false;
    private List<RigidbodyState> frozenRigidbodies = new List<RigidbodyState>();
  
    private float volume = 0.1f;
    [SerializeField] private AudioClip freezeSound;
    private float clipLength = 5f;
    private float slowPitch = 0.5f;
    private float pitch = 1f;

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
            SoundManager.Instance.PlaySoundClip(freezeSound, transform, volume, clipLength);
            GetComponent<Ambience>().Past.pitch = slowPitch;
            GetComponent<Ambience>().Future.pitch = slowPitch;

        }
    }

    public void ActivateFreeze()
    {
        Debug.Log($"ActivateFreeze called. IsOwner: {IsOwner}, isOnCooldown: {isOnCooldown}, isFreezeActive: {isFreezeActive}");

        if (!IsOwner)
        {
            Debug.Log("ActivateFreeze aborted: not owner.");
            return;
        }

        if (!isOnCooldown && !isFreezeActive)
        {
            Debug.Log("Starting FreezeTime coroutine.");
            StartCoroutine(FreezeTime());
        }
        else
        {
            Debug.Log("Cannot start freeze: either on cooldown or already active.");
        }
        if (!IsOwner) return;

        if (!isOnCooldown && !isFreezeActive)
        {
            StartCoroutine(FreezeTime());
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
        GetComponent<Ambience>().Past.pitch = pitch;
        GetComponent<Ambience>().Future.pitch = pitch;

        isFreezeActive = false;

        // Start cooldown
        StartCoroutine(Cooldown());
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
                velocity = rb.linearVelocity,                   // Store linear momentum
                angularVelocity = rb.angularVelocity,     // Store rotational momentum
                wasKinematic = rb.isKinematic
            };

            frozenRigidbodies.Add(state);

            // ===== FREEZE: Stop all movement =====
            rb.linearVelocity = Vector3.zero;           // Clear linear momentum
            rb.angularVelocity = Vector3.zero;    // Clear rotational momentum
            rb.isKinematic = true;                // Disable physics calculations
        }

        Debug.Log($"Froze {frozenRigidbodies.Count} rigidbodies for {freezeDuration} seconds");
    }

    void UnfreezeAllRigidbodies()
    {
        if (GetComponent<ClientNetworkTransform>().IsOwner == false) return;

        foreach (RigidbodyState state in frozenRigidbodies)
        {
            if (state.rb != null)
            {
                // ===== MOMENTUM RESTORATION: Reapply stored velocities =====
                state.rb.isKinematic = state.wasKinematic;      // Restore physics state
                state.rb.linearVelocity = state.velocity;             // Restore linear momentum
                state.rb.angularVelocity = state.angularVelocity; // Restore rotational momentum
                // Objects will continue moving exactly as they were before freeze!
            }
        }

        frozenRigidbodies.Clear();
        Debug.Log("Unfroze all rigidbodies");
    }

    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
        Debug.Log("Time freeze ability ready!");
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

    public bool IsFreezeActive()
    {
        return isFreezeActive;
    }
}