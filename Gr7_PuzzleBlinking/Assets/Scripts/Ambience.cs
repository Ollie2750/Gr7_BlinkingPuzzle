using UnityEngine;

public class Ambience : MonoBehaviour
{
    public AudioSource Past;
    public AudioSource Future;

    private float muted = 0f;
    private float playing = 0.1f;
    private void Awake()
    {
        {
            if (transform.position.x < 25)
            {
                Future.volume = muted;
                Past.volume = playing;
            }
            else
            {
                Future.volume = playing;
                Past.volume = muted;
            }
        }
    }
    private void Update()
    {
        if (transform.position.x < 25)
        {
            Future.volume = muted;
            Past.volume = playing;
        }
        else
        {
            Future.volume = playing;
            Past.volume = muted;
        }
    }
}
