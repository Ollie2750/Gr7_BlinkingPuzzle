using UnityEngine;

public class ExitBehavior : MonoBehaviour

{
    public GameObject targetObject; // Det GameObject, hvis materiale skal ændres
    public Material newMaterial; // Materiale der skal bruges, når spilleren går ind i triggeren
    private Renderer targetRenderer;

    private void Start()
    {
        // Hent Renderer-komponenten fra det angivne targetObject
        if (targetObject != null)
        {
            targetRenderer = targetObject.GetComponent<Renderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Tjek om det er spilleren, der går ind i triggeren
        if (other.CompareTag("Player") && targetRenderer != null && newMaterial != null)
        {
            targetRenderer.material = newMaterial;
        }
    }
}
