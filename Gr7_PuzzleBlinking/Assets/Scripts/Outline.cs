using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    private Renderer rend;
    private Material[] originalMaterials;
    private Material outlineMaterial;

    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float outlineWidth = 1.03f;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterials = rend.sharedMaterials;

        outlineMaterial = new Material(Shader.Find("Outlined/Uniform"));
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_Outline", outlineWidth);

        DisableOutline();
    }

    public void EnableOutline()
    {
        var mats = new Material[originalMaterials.Length + 1];
        originalMaterials.CopyTo(mats, 0);
        mats[mats.Length - 1] = outlineMaterial;
        rend.materials = mats;
    }

    public void DisableOutline()
    {
        rend.materials = originalMaterials;
    }
}
