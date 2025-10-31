using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;

public class LaserBeam : MonoBehaviour
{
    private LineRenderer myLine;

    private void Start()
    {
        myLine = GetComponent<LineRenderer>();
        DrawLine(Vector3.zero);
    }

    void DrawLine(Vector3 end)
    {   
        Vector3 start = gameObject.transform.position;
        myLine.startWidth = 0.2f;
        myLine.endWidth = 0.2f;
        myLine.SetPosition(0, start);
        myLine.SetPosition(1, end);
    }

}
