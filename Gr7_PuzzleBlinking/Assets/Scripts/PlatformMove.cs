using UnityEngine;

public class PlatformMove : MonoBehaviour
{

    public bool moving;
    [SerializeField] float speed;
    [SerializeField] int startpoint;
    [SerializeField] Transform[] points;

    int i;

    
    void Start()
    {
        transform.position = points[startpoint].position;
        i = startpoint;
    }

    void Update()
    {
        if(moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, points[i].position) < 0.01f)
            {
                moving = false;
                transform.position = points[i].position;
            }
        }
    }
    public void Activate()
    {
        if (moving) { return; }

        i++;

        if (i == points.Length)
        {
            i = 0;
        }

        moving = true;
    }
}
