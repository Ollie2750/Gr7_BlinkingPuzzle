using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public Camera cam;                    // assign your player camera (or left empty to auto-grab)
    public KeyCode useKey = KeyCode.E;
    public float useDistance = 4f;        // how far you can “use” a button
    public LayerMask interactMask = ~0;   // set to "Interactable" layer if you make one
    public bool showDebugRay = false;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (showDebugRay)
            Debug.DrawRay(cam.transform.position, cam.transform.forward * useDistance, Color.yellow);

        if (Input.GetKeyDown(useKey))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, useDistance, interactMask, QueryTriggerInteraction.Ignore))
            {
                // Button can be on the hit collider or a parent object
                var pad = hit.collider.GetComponentInParent<ButtonPad>();
                if (pad != null)
                {
                    pad.Activate();
                }
            }
        }
    }
}
