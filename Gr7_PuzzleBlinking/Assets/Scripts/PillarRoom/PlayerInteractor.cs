using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;                    // assign your player camera (defaults to main camera)
    public KeyCode useKey = KeyCode.E;
    public float useDistance = 4f;        // how far you can “use” a button
    public bool showDebugRay = false;     // Draws a yellow ray for a moment when pressing E

    public UIPromt uiPrompt;
    private Interactable currentInteractable;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
    // Debug ray so you can see where you're aiming
    if (showDebugRay)
        Debug.DrawRay(cam.transform.position, cam.transform.forward * useDistance, Color.yellow);

    Ray ray = new Ray(cam.transform.position, cam.transform.forward);
    bool canInteract = false;

    ButtonPad pad = null;
    TimedButton timedButton = null;
    Interactable interactable = null;

    if (Physics.Raycast(ray, out RaycastHit hit, useDistance))
    {
        // Only interact with objects tagged "Button"
        if (hit.collider.CompareTag("Button"))
        {
            pad = hit.collider.GetComponentInParent<ButtonPad>();
            timedButton = hit.collider.GetComponentInParent<TimedButton>();
            interactable = hit.collider.GetComponentInParent<Interactable>();

            if (pad != null || timedButton != null || interactable != null)
            {
                canInteract = true;
            }
        }
    }

    // -------- HANDLE HOVER for Interactable system --------
    if (interactable != currentInteractable)
    {
        // Stop hovering the old one
        if (currentInteractable != null)
            currentInteractable.OnHoverExit();

        // Start hovering the new one
        if (interactable != null)
            interactable.OnHoverEnter();

        currentInteractable = interactable;
    }

    // -------- UI Prompt --------
    if (uiPrompt != null)
    {
        if (canInteract)
            uiPrompt.Show("Press E to interact");
        else
            uiPrompt.Hide();
    }

    // -------- Handle interaction when pressing E --------
    if (canInteract && Input.GetKeyDown(useKey))
    {
        // First priority: Interactable system
        if (interactable != null)
        {
            interactable.Interact();
            return;
        }

        // Legacy systems
        if (pad != null)
            pad.Activate();
    }
}
}
