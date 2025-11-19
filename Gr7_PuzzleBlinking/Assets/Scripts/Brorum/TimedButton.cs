    using UnityEngine;
    using System.Collections;

public class TimedButton : MonoBehaviour
{
    [Header("Bridge Logic")]
    [SerializeField] private BridgeController bridge;

    [Header("Button Visuals")]
    public Renderer buttonRenderer;            // Assign in inspector
    public Color idleColor = Color.red;
    public Color pressedColor = Color.green;
    public float cooldownTime = 2f;

    [Header("Button Movement")]
    public Transform buttonTop;                // Assign the moving part
    public float pressDepth = 0.02f;

    private bool isOnCooldown = false;
    private Vector3 initialTopLocalPos;

    private void Start()
    {
        // If user does not assign a renderer, try find one
        if (!buttonRenderer)
            buttonRenderer = GetComponentInChildren<Renderer>();

        // Set initial color
        if (buttonRenderer)
            buttonRenderer.material.color = idleColor;

        // Store starting top position
        if (buttonTop)
            initialTopLocalPos = buttonTop.localPosition;
    }

    public void activateBridge()
    {
        // Don’t activate twice during cooldown
        if (isOnCooldown)
            return;

        // Start visuals + cooldown
        StartCoroutine(ButtonCooldownRoutine());

        // Original logic (unchanged)
        if (bridge != null)
        {
            bridge.bounceBridge();
        }
        else
        {
            Debug.LogWarning("TimedButton: No BridgeController assigned.");
        }
    }

    private IEnumerator ButtonCooldownRoutine()
    {
        isOnCooldown = true;

        // Change to green
        if (buttonRenderer)
            buttonRenderer.material.color = pressedColor;

        // Move top down
        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos + Vector3.down * pressDepth;

        // Wait during cooldown
        yield return new WaitForSeconds(cooldownTime);

        // Back to red
        if (buttonRenderer)
            buttonRenderer.material.color = idleColor;

        // Move top back up
        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos;

        isOnCooldown = false;
    }
}
