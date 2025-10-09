using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameObject platformMove;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            platformMove.GetComponent<Animator>().Play("PlatMove_Animation");
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            platformMove.GetComponent<Animator>().Play("PlatMoveDown_Animation");
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
        }

    }
}
