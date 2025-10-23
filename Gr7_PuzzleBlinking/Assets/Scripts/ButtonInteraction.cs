using UnityEngine;

public class ButtonInteraction : Interactable
{
    [SerializeField] private Light Light;
    
    public override void Interact()
    {
        Light.enabled = !Light.enabled;
        Debug.Log("Button Hit");
    }
}
