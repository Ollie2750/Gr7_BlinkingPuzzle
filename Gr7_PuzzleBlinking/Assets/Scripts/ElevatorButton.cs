using UnityEngine;

public class ElevatorButton : Interactable
{
   
    private bool isPressed = false;
    public GameObject elevator;


    public override void Interact()
    {
        Debug.Log("Elevator button pressed!");
        elevator.GetComponent<PlatformMove>().canMove = true;
        
    }

 
    
}
