using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameObject platformMove;

    private Animator animator;

    private void Awake()
    {
        animator = platformMove.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetBool("IsPlayerOnPlatform", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetBool("IsPlayerOnPlatform", false);
        }
    }
}
