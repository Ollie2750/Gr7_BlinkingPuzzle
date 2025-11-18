using System.Collections;
using UnityEngine;

/// <summary>
/// Simple BridgeController: shows a bridge (by toggling renderers & colliders or SetActive)
/// for a limited interval when requested.
/// - Attach this to a GameObject that controls the bridge visuals/colliders (or set `toToggle`).
/// - By default it toggles renderers & colliders instead of SetActive so coroutines keep running.
/// </summary>
public class BridgeController : MonoBehaviour
{
	[Header("Timing")]
	[Tooltip("Default duration the bridge stays visible (seconds)")]
	[SerializeField] private float upDuration = 5f;

	[Header("Target")]
	[Tooltip("If set, this GameObject (and its children) will be toggled. If null, this.gameObject is used.")]
	[SerializeField] private GameObject toToggle;

	[Tooltip("If true, SetActive is used on `toToggle`. If false (default), child renderers & colliders are enabled/disabled.")]
	[SerializeField] private bool useSetActive = false;

	[Header("Behavior")]
	[Tooltip("If true, pressing the button again restarts the visible timer. If false, additional presses while visible do nothing.")]
	[SerializeField] private bool retriggerRestartsTimer = true;

	[Tooltip("If true the bridge starts hidden in Awake()")]
	[SerializeField] private bool startHidden = true;

	private Coroutine activeRoutine;
	private bool isUp;

	private void Awake()
	{
		if (toToggle == null) toToggle = gameObject;

		if (startHidden)
		{
			isUp = false;
			ApplyHidden();
		}
		else
		{
			isUp = true;
			ApplyShown();
		}
	}

	/// <summary>
	/// Show the bridge for `duration` seconds. If duration <= 0, uses the default upDuration.
	/// </summary>
	public void ShowForDuration(float duration = -1f)
	{
		if (duration <= 0f) duration = upDuration;

		if (activeRoutine != null)
		{
			if (retriggerRestartsTimer)
			{
				StopCoroutine(activeRoutine);
				activeRoutine = null;
			}
			else
			{
				// Already up and we don't retrigger
				return;
			}
		}

		activeRoutine = StartCoroutine(ShowRoutine(duration));
	}

	private IEnumerator ShowRoutine(float duration)
	{
		Show();
		yield return new WaitForSeconds(duration);
		Hide();
		activeRoutine = null;
	}

	/// <summary>
	/// Immediately show the bridge (no timer management).
	/// </summary>
	public void Show()
	{
		if (isUp) return;
		isUp = true;
		ApplyShown();
	}

	/// <summary>
	/// Immediately hide the bridge.
	/// </summary>
	public void Hide()
	{
		if (!isUp) return;
		isUp = false;
		ApplyHidden();
	}

	private void ApplyShown()
	{
		if (useSetActive)
		{
			// If you set `toToggle` to this.gameObject and use SetActive(true/false),
			// be aware that disabling this GameObject will stop coroutines on this component.
			toToggle.SetActive(true);
		}
		else
		{
			ToggleRenderersAndColliders(true);
		}
	}

	private void ApplyHidden()
	{
		if (useSetActive)
		{
			toToggle.SetActive(false);
		}
		else
		{
			ToggleRenderersAndColliders(false);
		}
	}

	private void ToggleRenderersAndColliders(bool enabled)
	{
		// Renderers (MeshRenderer, SpriteRenderer etc.)
		foreach (var r in toToggle.GetComponentsInChildren<Renderer>(true))
			r.enabled = enabled;

		// 3D Colliders
		foreach (var c in toToggle.GetComponentsInChildren<Collider>(true))
			c.enabled = enabled;

		// 2D Colliders
		foreach (var c2 in toToggle.GetComponentsInChildren<Collider2D>(true))
			c2.enabled = enabled;
	}

	/// <summary>
	/// Returns true if the bridge is currently visible/walkable.
	/// </summary>
	public bool IsUp => isUp;
}
