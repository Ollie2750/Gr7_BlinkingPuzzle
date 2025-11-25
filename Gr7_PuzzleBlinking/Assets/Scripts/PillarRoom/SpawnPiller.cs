using Unity.Netcode;
using UnityEngine;

public class SpawnPiller : Interactable
{
    [SerializeField] private GameObject piller;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private AudioClip clickSound;
    [SerializeField][Range(0,1)] private float volume;

    public override void Interact(bool isHost)
    {
        piller.transform.localPosition = spawnPosition;
        piller.transform.localRotation = Quaternion.identity;
        SoundManager.Instance.PlaySoundClip(clickSound, transform, volume);

        GetComponent<ButtonVisual>().TryPress(isHost);
    }
}