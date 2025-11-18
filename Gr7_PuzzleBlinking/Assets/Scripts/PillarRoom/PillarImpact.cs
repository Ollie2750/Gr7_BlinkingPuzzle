using UnityEngine;

public class PillarImpact : MonoBehaviour
{
    private AudioSource audioSource;
    private float lastPlayTime = -999f;
    private Vector3 spawnPosition;
    private float spawnTime = -999f;

    public float soundCooldown = 0.5f;           // Prevent rapid repeated sounds
    public float minFallDistance = 0.1f;         // Pillar must fall at least this far before sound plays

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        lastPlayTime = -999f;  // Reset so first impact can play sound
        spawnPosition = transform.position;
        spawnTime = Time.time;
        
        // Ensure Play On Awake is disabled so we control when sound plays
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float timeSinceLastSound = Time.time - lastPlayTime;
        float distanceFallen = spawnPosition.y - transform.position.y;

        // Play sound only if:
        // 1. Pillar has fallen far enough from spawn (not at spawn height)
        // 2. Enough time since last sound
        if (distanceFallen >= minFallDistance && timeSinceLastSound >= soundCooldown)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
                lastPlayTime = Time.time;
            }
        }
    }
}
