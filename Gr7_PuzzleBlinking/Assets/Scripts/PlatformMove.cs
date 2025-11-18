using UnityEngine;

public class PlatformMove : MonoBehaviour
{

    public bool canMove;
    [SerializeField] float speed;
    [SerializeField] int startpoint;
    [SerializeField] Transform[] points;

    int i;
    bool reverse;

    
    void Start()
    {
        transform.position = points[startpoint].position;
        i = startpoint;
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, points[i].position)<0.01f)
        {
            canMove = false;
            if(i == points.Length - 1)
            {
                reverse = true;
                i--;
                return;
            }
            else if(i == 0)
            {
                reverse = false;
                i++;
                return;
            }

            if(reverse)
            {
                i--;

            }
            else
            {
                i++;
            }
            
        }
        
        if(canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
    }
}
