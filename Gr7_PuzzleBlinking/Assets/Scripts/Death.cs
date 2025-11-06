using Unity.VisualScripting;
using UnityEngine;

public class Death : MonoBehaviour
{
    public int maxHP = 100;
    public int playerHP;

    private void Start()
    {
        playerHP = maxHP;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.tag == "Player")
        {
            playerHP = playerHP - maxHP;
        }
    }
    void Update()
    {
        
    }
}
