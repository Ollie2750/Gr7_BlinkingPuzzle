    using UnityEngine;
    using System.Collections;

public class TimedButton : Interactable
{
    [Header("Bridge")]
    [SerializeField] private BridgeController bridge;

    private float volume = 0.2f;
    [SerializeField] private AudioClip TimedButtonSound;


    public override void Interact(bool isHost)
    {
        SoundManager.Instance.PlaySoundClip(TimedButtonSound, transform, volume);
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
