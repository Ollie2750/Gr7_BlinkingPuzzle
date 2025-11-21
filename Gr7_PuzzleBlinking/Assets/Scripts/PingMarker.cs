using UnityEngine;
using UnityEngine.UI;

public class PingMarker : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinScale = 0.8f;
    [SerializeField] private float pulseMaxScale = 1.2f;
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Components - Auto-detected or Assign Manually")]
    [SerializeField] private Image uiImage;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Renderer meshRenderer;

    private float spawnTime;
    private Vector3 baseScale;
    private Color baseColor;
    private Material materialInstance;

    void Start()
    {
        spawnTime = Time.time;

        // Auto-find components if not assigned
        if (uiImage == null)
        {
            uiImage = GetComponentInChildren<Image>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<Renderer>();
        }

        // Determine which component we're using
        if (uiImage != null)
        {
            Debug.Log("✓ PingMarker: Using UI Image");
            baseScale = uiImage.transform.localScale;
            baseColor = uiImage.color;

            // Ensure canvas is set up correctly
            Canvas canvas = uiImage.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Debug.Log($"  Canvas Render Mode: {canvas.renderMode}");
                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    Debug.LogWarning("⚠ Canvas should be in WorldSpace mode!");
                }
            }
        }
        else if (spriteRenderer != null)
        {
            Debug.Log("✓ PingMarker: Using SpriteRenderer");
            baseScale = spriteRenderer.transform.localScale;
            baseColor = spriteRenderer.color;
        }
        else if (meshRenderer != null)
        {
            Debug.Log("✓ PingMarker: Using MeshRenderer");
            baseScale = meshRenderer.transform.localScale;
            materialInstance = new Material(meshRenderer.material);
            meshRenderer.material = materialInstance;
            baseColor = materialInstance.color;
        }
        else
        {
            Debug.LogWarning("⚠ PingMarker: No renderer found! Using transform only.");
            baseScale = transform.localScale;
            baseColor = Color.white;
        }

        Debug.Log($"  Base Scale: {baseScale}");
        Debug.Log($"  Position: {transform.position}");
    }

    void Update()
    {
        float age = Time.time - spawnTime;

        if (age >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        float pulsePhase = Mathf.Sin(age * pulseSpeed * Mathf.PI);
        float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, (pulsePhase + 1f) / 2f);

        float lifetimeProgress = age / lifetime;
        float alpha = 1f;
        if (lifetimeProgress > 0.7f)
        {
            float fadeProgress = (lifetimeProgress - 0.7f) / 0.3f;
            alpha = fadeOutCurve.Evaluate(fadeProgress);
        }

        if (uiImage != null)
        {
            uiImage.transform.localScale = baseScale * scale;
            Color c = baseColor;
            c.a = alpha;
            uiImage.color = c;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.transform.localScale = baseScale * scale;
            Color c = baseColor;
            c.a = alpha;
            spriteRenderer.color = c;
        }
        else if (meshRenderer != null && materialInstance != null)
        {
            meshRenderer.transform.localScale = baseScale * scale;
            Color c = baseColor;
            c.a = alpha;
            materialInstance.color = c;
        }
    }

    public void SetColor(Color color)
    {
        baseColor = color;

        if (uiImage != null)
        {
            uiImage.color = color;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
        else if (materialInstance != null)
        {
            materialInstance.color = color;
        }
    }

    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
    }

    void OnDestroy()
    {
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }
}