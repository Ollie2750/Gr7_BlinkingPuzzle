using UnityEngine;

public class ButtonSpawner : Interactable
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject spawner;

    public override void Interact()
    {
        Debug.Log("Spawner button pressed!");
        Instantiate(boxPrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, spawner.transform.position.z), Quaternion.identity);
        
    }
}
