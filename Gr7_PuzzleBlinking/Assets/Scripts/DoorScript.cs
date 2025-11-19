using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DoorScript : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float doorDistance = 3;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float slideDistance = 1;
    private Vector3 closed;
    private Vector3 open;
    [SerializeField] private bool reversed;
    
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        closed = transform.position;

        if (reversed)
        {
            open = transform.position + transform.up * slideDistance;

        }
        else
        {
            open = transform.position + transform.up * slideDistance * -1;
        }
    }
    void Update()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= doorDistance) 
        {
            transform.position = Vector3.Lerp(transform.position, open, Time.deltaTime * speed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, closed, Time.deltaTime * speed);

        }
    }
}
