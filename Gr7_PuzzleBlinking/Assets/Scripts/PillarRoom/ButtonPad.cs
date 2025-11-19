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

    [Header("Button visuals")]
    public Renderer buttonRenderer;
    public Color idleColor = Color.red;
    public Color pressedColor = Color.green;
    public float cooldownTime = 2f;

    [Header("Button movement")]
    public Transform buttonTop;          // top piece that moves down
    public float pressDepth = 0.02f;     // how far it moves down in local Y

    bool isOnCooldown = false;
    Vector3 initialTopLocalPos;
    GameObject current;

    void Start()
    {
        if (!buttonRenderer)
            buttonRenderer = GetComponent<Renderer>();

        if (buttonRenderer)
            buttonRenderer.material.color = idleColor;

        if (buttonTop)
            initialTopLocalPos = buttonTop.localPosition;
    }

    // Called by PlayerInteractor
    public void Activate()
    {
        if (isOnCooldown || !manager || !pillarPrefab)
            return;

        StartCoroutine(ButtonCooldownRoutine());

        // remember old XZ before despawn (for respawnAtSamePlace)
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

    IEnumerator ButtonCooldownRoutine()
    {
        isOnCooldown = true;

        if (buttonRenderer)
            buttonRenderer.material.color = pressedColor;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos + Vector3.down * pressDepth;

        yield return new WaitForSeconds(cooldownTime);

        if (buttonRenderer)
            buttonRenderer.material.color = idleColor;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos;

        isOnCooldown = false;
    }

    // Interactable calls this
    public void Interact()
    {
        Activate();
    }
}
