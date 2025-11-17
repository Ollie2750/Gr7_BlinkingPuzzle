using UnityEngine;

public class PlayerAbilitiesSwitch : MonoBehaviour
{
    ClientNetworkTransform clientTransform;
    TimeFreezeAbility timeFreeze;
    TimeTravel timeTravel;

    void Start()
    {
        clientTransform = GetComponent<ClientNetworkTransform>();
        timeFreeze = GetComponent<TimeFreezeAbility>();
        timeTravel = GetComponent<TimeTravel>();

        if (clientTransform.IsOwnedByServer)
        {
            Debug.Log("Player 1 Joined");
            timeFreeze.enabled = true;
        }
        else
        {
            Debug.Log("Player 2 Joined");
            timeTravel.enabled = true;
        }
    }

    void Update()
    {
        
    }
}
