using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DoorScript : MonoBehaviour
{
    private Transform target;
    private float doorDistance = 3;
    private float speed = 3.5f;
    private float slideDistance = 1;
    private Vector3 closed;
    private Vector3 open;
    [SerializeField] private bool reversed;

    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;
    private bool isOpen;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        closed = transform.position;

        if (reversed)
        {
            open = transform.position + transform.up * slideDistance;

        }
        else
        {
            open = transform.position + transform.up * slideDistance * -1;
        }
    }

    void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= doorDistance)
        {
            if (!isOpen) // Only play sound when transitioning to open
            {
                if (doorOpenSound != null)
                {
                    SoundManager.Instance.PlaySoundClip(doorOpenSound, transform, 0.5f);
                }
                isOpen = true;
            }
            transform.position = Vector3.Lerp(transform.position, open, Time.deltaTime * speed);
        }
        else
        {
            if (isOpen) // Only play sound when transitioning to closed
            {
                //if (doorCloseSound != null)
                //{
                //    SoundManager.Instance.PlaySoundClip(doorCloseSound, transform, 0.5f);
                //}
                isOpen = false;
            }
            transform.position = Vector3.Lerp(transform.position, closed, Time.deltaTime * speed);
        }
    }
}