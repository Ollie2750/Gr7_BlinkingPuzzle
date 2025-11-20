    using UnityEngine;
    using System.Collections;

public class TimedButton : MonoBehaviour
{
    [Header("Bridge")]
    [SerializeField] private BridgeController bridge;

    [Header("Visual & animation")]
    public ButtonVisual visual;      // shared visual script

    public void activateBridge()
    {
        // play visual; if on cooldown, ignore
        if (visual != null && !visual.TryPress())
            return;

        if (bridge != null)
        {
            bridge.bounceBridge();
        }
        else
        {
            Debug.LogWarning("TimedButton: No BridgeController assigned.");
        }
    }
}
