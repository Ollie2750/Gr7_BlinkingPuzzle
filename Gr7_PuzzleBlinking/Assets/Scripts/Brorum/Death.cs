using Unity.VisualScripting;
using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] private Vector3 respawnLocation;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameObject player = collision.gameObject;
            player.GetComponent<CharacterController>().enabled = false;
            collision.gameObject.transform.position = respawnLocation;

            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
