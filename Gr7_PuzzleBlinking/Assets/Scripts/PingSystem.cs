using UnityEngine;
using UnityEngine.InputSystem;

public class PingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject pingMarkerPrefab;
    [SerializeField] private InputSystem_Actions inputActions;

    [Header("Ping Settings")]
    [SerializeField] private float maxPingDistance = 100f;
    [SerializeField] private float pingOffset = 0.1f;
    [SerializeField] private LayerMask pingableLayers = ~0;
    [SerializeField] private Color defaultPingColor = Color.cyan;

    [Header("Audio")]
    [SerializeField] private AudioClip pingPlaceSound;
    [SerializeField] private AudioClip pingFailSound;
    private AudioSource audioSource;

    [Header("Cooldown")]
    [SerializeField] private float pingCooldown = 0.3f;
    private float lastPingTime = -999f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.MiddleClick.performed += OnPingInput;
    }

    void OnDisable()
    {
        inputActions.UI.MiddleClick.performed -= OnPingInput; // ✅ FIXED - Now properly unsubscribes
        inputActions.Disable();
    }

    private void OnPingInput(InputAction.CallbackContext context)
    {
        if (Time.time - lastPingTime < pingCooldown)
            return;

        PlacePing();
        lastPingTime = Time.time;
    }

    public void PlacePing()
    {
        if (playerCamera == null || pingMarkerPrefab == null)
        {
            Debug.LogWarning("PingSystem: Missing references!");
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        Debug.DrawRay(playerCamera.position, playerCamera.forward * maxPingDistance, Color.cyan, 5f); // Longer duration

        if (Physics.Raycast(ray, out hit, maxPingDistance, pingableLayers))
        {
            Vector3 pingPosition = hit.point + hit.normal * pingOffset;
            Quaternion pingRotation = Quaternion.LookRotation(hit.normal);

            GameObject pingMarker = Instantiate(pingMarkerPrefab, pingPosition, pingRotation);

            // DEBUG: Log everything
            Debug.Log($"✓ Ping spawned!");
            Debug.Log($"  Position: {pingPosition}");
            Debug.Log($"  Scale: {pingMarker.transform.localScale}");
            Debug.Log($"  Active: {pingMarker.activeSelf}");
            Debug.Log($"  Hit object: {hit.collider.name}");

            // DEBUG: Draw a sphere at ping location
            Debug.DrawRay(pingPosition, Vector3.up * 2f, Color.green, 5f);
            Debug.DrawRay(pingPosition, Vector3.right * 2f, Color.red, 5f);
            Debug.DrawRay(pingPosition, Vector3.forward * 2f, Color.blue, 5f);

            PingMarker marker = pingMarker.GetComponent<PingMarker>();
            if (marker != null)
            {
                marker.SetColor(defaultPingColor);
            }
            else
            {
                Debug.LogWarning("⚠ PingMarker component not found on prefab!");
            }

            if (pingPlaceSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pingPlaceSound);
            }
        }
        else
        {
            Debug.Log("❌ Raycast hit nothing");
            if (pingFailSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pingFailSound);
            }
        }
    }

    public void PlacePingAtPosition(Vector3 position, Vector3 normal, Color color)
    {
        Vector3 pingPosition = position + normal * pingOffset;
        Quaternion pingRotation = Quaternion.LookRotation(normal);

        GameObject pingMarker = Instantiate(pingMarkerPrefab, pingPosition, pingRotation);

        PingMarker marker = pingMarker.GetComponent<PingMarker>();
        if (marker != null)
        {
            marker.SetColor(color);
        }

        if (pingPlaceSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pingPlaceSound);
        }
    }
}