using UnityEngine;

public class SpawnPiller : Interactable
{
    [SerializeField] private GameObject piller;
    [SerializeField] private Vector3 spawnPosition;

    public override void Interact()
    {
        piller.transform.localPosition = spawnPosition;
    }
}
