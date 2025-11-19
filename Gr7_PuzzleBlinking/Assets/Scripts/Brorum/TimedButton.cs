    using UnityEngine;
    using System.Collections;

public class TimedButton : MonoBehaviour
{
    [Header("Bridge Logic")]
    [SerializeField] private BridgeController bridge;

    [Header("Button Visuals (Materials)")]
    public Renderer buttonRenderer;
    public Material idleMaterial;
    public Material pressedMaterial;
    public float cooldownTime = 2f;

    [Header("Button Movement")]
    public Transform buttonTop;
    public float pressDepth = 0.02f;

    private bool isOnCooldown = false;
    private Vector3 initialTopLocalPos;

    private void Start()
    {
        if (!buttonRenderer)
            buttonRenderer = GetComponentInChildren<Renderer>();

        if (buttonTop)
            initialTopLocalPos = buttonTop.localPosition;

        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;
    }

    public void activateBridge()
    {
        if (isOnCooldown)
            return;

        StartCoroutine(ButtonCooldownRoutine());

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

        if (buttonRenderer && pressedMaterial)
            buttonRenderer.material = pressedMaterial;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos + Vector3.down * pressDepth;

        yield return new WaitForSeconds(cooldownTime);

        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos;

        isOnCooldown = false;
    }
}
