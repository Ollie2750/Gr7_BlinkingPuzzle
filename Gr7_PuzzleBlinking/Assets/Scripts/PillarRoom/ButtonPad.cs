using UnityEngine;
using System.Collections;

// A floor/wall button. When the player looks at it and presses E (via PlayerInteractor),
// we call Activate(): despawn the current pillar (if any) and spawn a new one.
public class ButtonPad : MonoBehaviour
{
    [Header("Pillar spawning")]
    public PillarManager manager;
    public GameObject pillarPrefab;
    public bool respawnAtSamePlace = false;

    [Tooltip("If true, this button will always spawn its pillar at this fixed point.")]
    public bool useFixedSpawnPoint = false;
    public Transform fixedSpawnPoint;

    [Header("Visual & animation")]
    public ButtonVisual visual;          // reference to shared visual script

    private GameObject current;

    // Called by PlayerInteractor when the player presses E on this button
    public void Activate(bool isHost)
    {
        // Play visual & respect cooldown
        if (visual != null && !visual.TryPress(isHost))
            return;

        if (!manager || !pillarPrefab)
            return;

        // --- PILLAR LOGIC ---

        bool hadOldPos = current != null;
        Vector3 oldXZ = Vector3.zero;

        if (hadOldPos)
            oldXZ = new Vector3(current.transform.position.x, 0f, current.transform.position.z);

        if (current)
            manager.Despawn(current);

        if (useFixedSpawnPoint && fixedSpawnPoint)
        {
            current = manager.SpawnAtExactPosition(pillarPrefab, fixedSpawnPoint.position);
        }
        else if (respawnAtSamePlace && hadOldPos)
        {
            current = manager.SpawnAtSameXZ(pillarPrefab, oldXZ);
        }
        else
        {
            current = manager.SpawnPillar(pillarPrefab);
        }
    }

    // Interactable system calls this
    public void Interact(bool isHost)
    {
        Activate(isHost);
    }
}
