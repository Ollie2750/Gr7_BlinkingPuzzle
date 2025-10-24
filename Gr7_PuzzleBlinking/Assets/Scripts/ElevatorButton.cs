using UnityEngine;

public class ElevatorButton : Interactable
{
   
    private bool isPressed = false;
    public GameObject elevator;
    private Animator animator;

    private void Awake()
    {
        animator = elevator.GetComponent<Animator>();
    }

    public override void Interact()
    {
        Debug.Log("Elevator button pressed!");
        ToggleButtonState();
    }

    private void ToggleButtonState()
    {
        isPressed = !isPressed;
        animator.SetBool("On/off", isPressed);
    }

    private void Unpress()
    {
        isPressed = false;
        animator.SetBool("On/off", isPressed);
    } 
    
}
