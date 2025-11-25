using UnityEngine;
using UnityEngine.Rendering;

public class ButtonSpawner : Interactable
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject spawner;
    [SerializeField] private AudioClip press;
    [SerializeField] private float volume = 0.8f;

    public override void Interact(bool isHost)
    {
        Debug.Log("Spawner button pressed!");
        Instantiate(boxPrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y - 5, spawner.transform.position.z), Quaternion.identity);
        SoundManager.Instance.PlaySoundClip(press, gameObject.transform, volume);
    }
}
