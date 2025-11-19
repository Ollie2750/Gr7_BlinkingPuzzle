using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light[] lights; // Array af lys, der skal styres
    public float minTime = 0.5f;
    public float maxTime = 1.5f;
    public float flickerSpeed = 0.1f;
    private float flickerTimer = 0f;
    private float lastFlicker = 0f;

    public GameObject lightBulb; // Reference til pæren
    private Material bulbMaterial; // Materialet på pæren
    public Color bulbEmissionColor = Color.yellow; // Emissionsfarve for pæren

    void Start()
    {
        // Hent materialet fra pæren
        if (lightBulb != null)
        {
            Renderer bulbRenderer = lightBulb.GetComponent<Renderer>();
            if (bulbRenderer != null)
            {
                bulbMaterial = bulbRenderer.material;
            }
        }
    }

    void Update()
    {
        if (flickerTimer <= 0f)
        {
            SetLightsEnabled(false);
            flickerTimer = Random.Range(minTime, maxTime);
            lastFlicker = flickerSpeed;
        }
        else
        {
            flickerTimer -= Time.deltaTime;
            lastFlicker -= Time.deltaTime;
        }

        if (lastFlicker <= 0f)
        {
            SetLightsEnabled(true);
        }

        // Opdater pærens emission baseret på lysets tilstand
        if (bulbMaterial != null)
        {
            float intensity = AreLightsEnabled() ? lights[0].intensity : 0f;
            bulbMaterial.SetColor("_EmissionColor", bulbEmissionColor * intensity);
        }
    }

    private void SetLightsEnabled(bool enabled)
    {
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.enabled = enabled;
            }
        }
    }

    private bool AreLightsEnabled()
    {
        // Tjek om det første lys i arrayet er tændt
        return lights.Length > 0 && lights[0].enabled;
    }
}