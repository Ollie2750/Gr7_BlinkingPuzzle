using UnityEngine;

public class ButtonPad : MonoBehaviour
{
    public PillarManager manager;
    public GameObject pillarPrefab;
    public KeyCode useKey = KeyCode.E;

    [Tooltip("Spawn the new pillar at the old pillar's XZ if possible.")]
    public bool respawnAtSamePlace = false;

    // keep track of the pillar this pad spawned last time
    GameObject current;

    public void Activate()
    {
        if (!manager || !pillarPrefab) return;

        Vector3 oldXZ = Vector3.zero;
        if (current) oldXZ = new Vector3(current.transform.position.x, 0f, current.transform.position.z);

        if (current) manager.Despawn(current);

        current = respawnAtSamePlace
            ? manager.SpawnAtSameXZ(pillarPrefab, oldXZ)
            : manager.SpawnPillar(pillarPrefab);
    }
}
