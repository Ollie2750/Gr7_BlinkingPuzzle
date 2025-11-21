using Unity.Netcode;
using UnityEngine;

public class SpawnPiller : Interactable
{
    [SerializeField] private GameObject piller;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private AudioClip clickSound;
    [SerializeField][Range(0,1)] private float volume;

    public override void Interact()
    {
        piller.transform.localPosition = spawnPosition;
        SoundManager.Instance.PlaySoundClip(clickSound, transform, volume);

        GetComponent<ButtonVisual>().TryPress();
    }
}