    using UnityEngine;

/// <summary>
/// TimedButton: when the player stands inside the trigger and presses the interact key,
/// this will call the assigned BridgeController to make the bridge visible for a limited time.
/// </summary>
public class TimedButton : MonoBehaviour
{
	[Header("Player Detection")]
	[SerializeField] private string playerTag = "Player";
	[SerializeField] private KeyCode interactKey = KeyCode.E;

	[Header("Bridge")]
	[Tooltip("Reference to the BridgeController to trigger. If empty, Reset() will attempt to auto-find one.")]
	[SerializeField] private BridgeController bridge;

	[Tooltip("If > 0, this overrides the BridgeController's default duration for this button press.")]
	[SerializeField] private float customDuration = -1f;

	[Header("UI")]
	[Tooltip("Optional prompt GameObject to show when player is in range")]
	[SerializeField] private GameObject promptUI;

	[Header("Input")]
	[Tooltip("Minimum seconds between allowed presses")]
	[SerializeField] private float pressCooldown = 0.5f;

	private bool playerInside;
	private float lastPressTime = -999f;

	private void Reset()
	{
		// Try to auto-assign a BridgeController in the scene for convenience.
		if (bridge == null)
		{
#if UNITY_2023_2_OR_NEWER
			bridge = UnityEngine.Object.FindAnyObjectByType<BridgeController>();
#else
			bridge = FindObjectOfType<BridgeController>();
#endif
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(playerTag))
		{
			playerInside = true;
			if (promptUI != null) promptUI.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(playerTag))
		{
			playerInside = false;
			if (promptUI != null) promptUI.SetActive(false);
		}
	}

	private void Update()
	{
		if (!playerInside) return;

		if (Input.GetKeyDown(interactKey) && Time.time - lastPressTime >= pressCooldown)
		{
			if (bridge != null)
			{
				bridge.ShowForDuration(customDuration);
				lastPressTime = Time.time;
			}
			else
			{
				Debug.LogWarning("TimedButton: No BridgeController assigned.");
			}
		}
	}
}
