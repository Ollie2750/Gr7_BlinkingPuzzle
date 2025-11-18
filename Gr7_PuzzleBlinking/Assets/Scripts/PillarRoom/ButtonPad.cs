using UnityEngine;

// A floor/wall button. When the player looks at it and presses E (via PlayerInteractor),
// we call Activate(): despawn the current pillar (if any) and spawn a new one.
public class ButtonPad : MonoBehaviour
{
    public PillarManager manager;                                                // Spawner that knows the room bounds + spacing rules
    public GameObject pillarPrefab;                                              // Prefab to instantiate when this button is activated
    public KeyCode useKey = KeyCode.E;                                           // (Unused here—interaction happens via PlayerInteractor calling Activate())

    [Tooltip("Spawn the new pillar at the old pillar's XZ if possible.")]
    public bool respawnAtSamePlace = false;

    [Header("Fixed spawn (per button/pillar)")]
    [Tooltip("If true, this button will always spawn its pillar at this fixed point.")]
    public bool useFixedSpawnPoint = false;

    [Tooltip("World-space point where THIS button's pillar should spawn (use an empty GameObject here).")]
    public Transform fixedSpawnPoint;

    // keep track of the pillar this pad spawned last time
    // so we can despawn/replace it on subsequent presses.
    GameObject current;

    public void Activate()                                           // Called by PlayerInteractor when the player looks at this button and presses E.
    {
        if (!manager || !pillarPrefab) return;                       // Safety: if not wired in the Inspector, do nothing.

        Vector3 oldXZ = Vector3.zero;
        if (current) oldXZ = new Vector3(current.transform.position.x, 0f, current.transform.position.z);    // Remember the old XZ so we can optionally respawn at (roughly) the same place.(we dont really use that)

        if (current) manager.Despawn(current);                       // Remove prior pillar (if any) from the scene and manager’s tracking list

        if (useFixedSpawnPoint && fixedSpawnPoint != null)
        {
            // Always spawn at this button's dedicated spot
            current = manager.SpawnAtExactPosition(pillarPrefab, fixedSpawnPoint.position);
            return;
        }

        // Spawn a fresh pillar either at same XZ or at a random valid point
        current = respawnAtSamePlace
            ? manager.SpawnAtSameXZ(pillarPrefab, oldXZ)
            : manager.SpawnPillar(pillarPrefab);
    }
}
