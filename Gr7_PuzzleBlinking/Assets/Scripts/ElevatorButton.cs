using UnityEngine;
using System.Collections;

public class ElevatorButton : Interactable
{
    [Header("Elevator")]
    public GameObject elevator;

    [Header("Button Visuals (Materials)")]
    public Renderer buttonRenderer;          // drag MeshRenderer here
    public Material idleMaterial;            // drag idle material here
    public Material pressedMaterial;         // drag pressed material here
    public float cooldownTime = 2f;

    [Header("Button Movement")]
    public Transform buttonTop;              // moving top part
    public float pressDepth = 0.02f;

    private bool isOnCooldown = false;
    private Vector3 initialTopLocalPos;

    private void Start()
    {
        if (!buttonRenderer)
            buttonRenderer = GetComponentInChildren<Renderer>();

        if (buttonTop)
            initialTopLocalPos = buttonTop.localPosition;

        // set initial material
        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;
    }

    public override void Interact()
    {
        if (isOnCooldown)
            return;

        Debug.Log("Elevator button pressed!");

        StartCoroutine(ButtonCooldownRoutine());

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

    private IEnumerator ButtonCooldownRoutine()
    {
        isOnCooldown = true;

        // pressed material
        if (buttonRenderer && pressedMaterial)
            buttonRenderer.material = pressedMaterial;

        // move top down
        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos + Vector3.down * pressDepth;

        yield return new WaitForSeconds(cooldownTime);

        // back to idle material
        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;

        // move top back up
        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos;

        isOnCooldown = false;
    }
}
