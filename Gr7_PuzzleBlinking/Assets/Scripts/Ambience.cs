using UnityEngine;

public class Ambience : MonoBehaviour
{
    [SerializeField] private AudioClip pastAmbience;
    [SerializeField] private AudioClip futureAmbience;
    [SerializeField] private AudioSource empty;

    private void Awake()
    {
        {
            if (transform.position.x < 25)
            {
                empty.clip = pastAmbience;
                empty.Play();
            }
            else
            {
                empty.clip = futureAmbience;
                empty.Play();
            }
        }
    }
    private void Update()
    {
        if (transform.position.x < 25)
        {
            empty.clip = pastAmbience;
        }
        else
        {
            empty.clip = futureAmbience;
        }
    }
}
