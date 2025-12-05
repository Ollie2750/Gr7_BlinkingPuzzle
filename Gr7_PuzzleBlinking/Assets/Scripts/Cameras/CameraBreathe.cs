using UnityEngine;

public class CameraBreathe : MonoBehaviour
{
    [Header("Vejrtrækning Indstillinger")]
    [Tooltip("Hvor meget kameraet bevæger sig")]
    [Range(0.01f, 1f)]
    public float breatheIntensity = 0.15f;
    
    [Tooltip("Hvor hurtigt karakteren trækker vejret")]
    [Range(0.1f, 3f)]
    public float breatheSpeed = 0.8f;
    
    [Tooltip("Hvor blød bevægelsen er")]
    [Range(0.01f, 0.5f)]
    public float breatheSmoothing = 0.1f;
    
    // Interne variabler
    private float time;
    private Vector3 currentOffset;
    private Vector3 targetOffset;
    private Vector3 originalPosition;
    
    void Start()
    {
        originalPosition = transform.localPosition;
        currentOffset = Vector3.zero;
        targetOffset = Vector3.zero;
        time = 0f;
    }
    
    void LateUpdate()
    {
        // Opdater tid
        time += Time.deltaTime * breatheSpeed;
        
        // Simuler vejrtrækning med sinus-bølge
        float breatheCycle = Mathf.Sin(time) * breatheIntensity;
        
        // Tilføj lidt variation i X og Z aksen for mere naturlig bevægelse
        targetOffset = new Vector3(
            Mathf.Sin(time * 0.5f) * breatheIntensity * 0.3f,
            breatheCycle,
            Mathf.Sin(time * 0.7f) * breatheIntensity * 0.2f
        );
        
        // Blød overgang til target offset
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, breatheSmoothing);
        
        // Anvend offset til kameraet
        transform.localPosition = originalPosition + currentOffset;
    }
}
