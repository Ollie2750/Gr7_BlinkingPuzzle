using UnityEngine;

public class PressurePlateBehavior : MonoBehaviour
{

    public GameObject elevator;
    private Animator animator;
    private void Awake()
    {
        animator = elevator.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
    
            animator.SetBool("On/off", false);
        }
    }

}
