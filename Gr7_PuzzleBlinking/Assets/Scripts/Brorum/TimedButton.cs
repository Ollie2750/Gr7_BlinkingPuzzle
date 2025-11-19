    using UnityEngine;

public class TimedButton : MonoBehaviour
{
	[SerializeField] private BridgeController bridge;


	public void activateBridge()
	{
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
