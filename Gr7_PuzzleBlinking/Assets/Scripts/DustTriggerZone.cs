using UnityEngine;

public class DustTriggerZone : MonoBehaviour
{
    public ParticleSystem dustParticles;     
    public float delayBeforeStop = 5f;       

    private bool playerInside = false;       
    private float exitTimer = 0f;

    private void Start()
    {
        // Make sure dust is NOT playing at start
        dustParticles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            dustParticles.Play();          // Start dust ONLY when entering
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            exitTimer = Time.time;         // Start countdown
        }
    }

    private void Update()
    {
        // Only stop if player has been outside for more than delayBeforeStop
        if (!playerInside && Time.time > exitTimer + delayBeforeStop)
        {
            dustParticles.Stop();
        }
    }
}
