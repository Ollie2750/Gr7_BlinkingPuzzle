using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;                    // assign your player camera (defaults to main camera)
    public KeyCode useKey = KeyCode.E;
    public float useDistance = 4f;        // how far you can “use” a button
    public bool showDebugRay = false;     // Draws a yellow ray for a moment when pressing E

    public UIPromt uiPrompt;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        // Draw debug ray if enabled
        if (showDebugRay)
            Debug.DrawRay(cam.transform.position, cam.transform.forward * useDistance, Color.yellow);

        // Always raycast straight ahead from the camera
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool canInteract = false;

        ButtonPad pad = null;
        TimedButton button = null;

        if (Physics.Raycast(ray, out RaycastHit hit, useDistance))
        {
            // Check if we are looking at a ButtonPad
            pad = hit.collider.GetComponentInParent<ButtonPad>();

            // Or a TimedButton
            button = hit.collider.GetComponentInParent<TimedButton>();

            if (pad != null || button != null)
            {
                canInteract = true;
            }
        }

        // --- Handle UI prompt ---
        if (uiPrompt != null)
        {
            if (canInteract)
            {
                uiPrompt.Show("Press E to interact");
            }
            else
            {
                uiPrompt.Hide();
            }
        }

        // --- Handle actual interaction when E is pressed ---
        if (canInteract && Input.GetKeyDown(useKey))
        {
            if (pad != null)
            {
                pad.Activate();
            }

            if (button != null)
            {
                button.activateBridge();
            }
        }
    }
}
