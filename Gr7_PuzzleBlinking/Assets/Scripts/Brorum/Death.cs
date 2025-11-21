using Unity.VisualScripting;
using UnityEngine;

public class Death : MonoBehaviour
{
    [SerializeField] private Vector3 respawnLocation;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = respawnLocation; 
        }
    }
}
