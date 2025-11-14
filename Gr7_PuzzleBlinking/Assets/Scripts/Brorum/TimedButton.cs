    using UnityEngine;

/// <summary>
/// TimedButton: when the player stands inside the trigger and presses the interact key,
/// this will call the assigned BridgeController to make the bridge visible for a limited time.
/// </summary>
public class TimedButton : MonoBehaviour
{
	[Header("Bridge")]
	[Tooltip("Reference to the BridgeController to trigger. If empty, Reset() will attempt to auto-find one.")]
	[SerializeField] private BridgeController bridge;

	[Tooltip("If > 0, this overrides the BridgeController's default duration for this button press.")]
	[SerializeField] private float customDuration = -1f;


	public void activateBridge()
	{
        if (bridge != null)
        {
            bridge.ShowForDuration(customDuration);
        }
        else
        {
            Debug.LogWarning("TimedButton: No BridgeController assigned.");
        }
    }
}
