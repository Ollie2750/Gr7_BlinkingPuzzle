    using UnityEngine;
    using System.Collections;

public class TimedButton : Interactable
{
    [Header("Bridge")]
    [SerializeField] private BridgeController bridge;


    public override void Interact(bool isHost)
    {
        if (bridge != null)
        {
            bridge.bounceBridge();
        }
        else
        {
            Debug.LogWarning("TimedButton: No BridgeController assigned.");
        }
    }
}
