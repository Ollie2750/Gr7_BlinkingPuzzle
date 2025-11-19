using UnityEngine;
using System.Collections;

public class ElevatorButton : Interactable
{
    [Header("Elevator")]
    public GameObject elevator;

    [Header("Visual & animation")]
    public ButtonVisual visual;      // shared visual script

    public override void Interact()
    {
        // play visual; if on cooldown, ignore
        if (visual != null && !visual.TryPress())
            return;

        Debug.Log("Elevator button pressed!");

        if (elevator != null)
        {
            var platform = elevator.GetComponent<PlatformMove>();
            if (platform != null)
            {
                platform.canMove = true;
            }
            else
            {
                Debug.LogWarning("ElevatorButton: Elevator has no PlatformMove component.");
            }
        }
        else
        {
            Debug.LogWarning("ElevatorButton: No elevator GameObject assigned.");
        }
    }
}
