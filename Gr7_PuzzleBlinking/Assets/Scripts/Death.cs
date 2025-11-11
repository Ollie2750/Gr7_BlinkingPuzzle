using Unity.VisualScripting;
using UnityEngine;

public class Death : MonoBehaviour
{
    public int maxHP = 100;
    public int playerHP;
    public Vector3 spawnLocation;

    private void Start()
    {
        playerHP = maxHP;
    }

    private void OnTriggerEnter(Collider target)
    {
        Debug.Log("Death triggered");
        if (target.tag == "Death")
        {
            playerHP = playerHP - maxHP;
        }
    }
    void Update()
    {
        if (playerHP <= 0)
        {
            gameObject.GetComponent<CharacterController>().enabled = false;
            transform.position = (spawnLocation);
            playerHP = maxHP;
            gameObject.GetComponent<CharacterController>().enabled = true;

        }
    }
}
