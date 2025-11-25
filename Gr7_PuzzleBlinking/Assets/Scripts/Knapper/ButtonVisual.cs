using UnityEngine;
using System.Collections;

public class ButtonVisual : MonoBehaviour
{
    [Header("Visuals (Materials)")]
    public Renderer buttonRenderer;      // drag MeshRenderer here
    public Material idleMaterial;        // drag idle material
    public Material pressedMaterial;     // drag pressed material
    public Material errorMaterial;
    public float cooldownTime = 2f;

    [Header("Movement")]
    public Transform buttonTop;          // moving top part
    public float pressDepth = 0.02f;     // how far it moves down

    bool isOnCooldown = false;
    Vector3 initialTopLocalPos;

    void Start()
    {
        if (!buttonRenderer)
            buttonRenderer = GetComponentInChildren<Renderer>();

        if (buttonTop)
            initialTopLocalPos = buttonTop.localPosition;

        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;
    }

    /// <summary>
    /// Try to play press animation. Returns true if it actually pressed
    /// (not on cooldown), false if ignored.
    /// </summary>
    public bool TryPress(bool isHost)
    {
        if (isOnCooldown)
            return false;
        if (isHost)
        {
            StartCoroutine(PressRoutine());
            return true;
        }
        StartCoroutine(ErrorRoutine());
        return true;

    }

    IEnumerator PressRoutine()
    {
        isOnCooldown = true;

        // pressed look
        if (buttonRenderer && pressedMaterial)
            buttonRenderer.material = pressedMaterial;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos + Vector3.down * pressDepth;

        // wait
        yield return new WaitForSeconds(cooldownTime);

        // back to idle
        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos;

        isOnCooldown = false;
    }

    IEnumerator ErrorRoutine()
    {
        isOnCooldown = true;

        // pressed look
        if (buttonRenderer && pressedMaterial)
            buttonRenderer.material = pressedMaterial;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos + Vector3.down * pressDepth;

        // wait
        yield return new WaitForSeconds(cooldownTime);

        // back to idle
        if (buttonRenderer && idleMaterial)
            buttonRenderer.material = idleMaterial;

        if (buttonTop)
            buttonTop.localPosition = initialTopLocalPos;

        isOnCooldown = false;
    }
}
