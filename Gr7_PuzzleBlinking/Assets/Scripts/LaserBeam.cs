using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;

public class LaserBeam : MonoBehaviour
{
    private LineRenderer myLine;
    [SerializeField] private Material beamMaterial;
    

    private void Start()
    {
        myLine = GetComponent<LineRenderer>();
        DrawLine(Vector3.zero, new Color(1f, 0f, 0f));
    }

    void DrawLine(Vector3 end, Color color)
    {   
        Vector3 start = gameObject.transform.position;
        myLine.material = beamMaterial; //new Material(Shader.Find("Transparent/Diffuse"));
        myLine.startColor = color;
        myLine.endColor = color;
        myLine.startWidth = 0.1f;
        myLine.endWidth = 0.1f;
        myLine.SetPosition(0, start);
        myLine.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }

}
