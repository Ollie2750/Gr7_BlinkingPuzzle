using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource audioSourceObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Soundmanager instanced");
        }
    }

    public void PlaySoundClip(AudioClip audioclip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(audioSourceObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioclip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
