using UnityEngine;
using UnityEngine.Rendering;

public class ButtonSpawner : Interactable
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject spawner;
    [SerializeField] private AudioClip press;
    public float volume = 2;

    public override void Interact()
    {
        Debug.Log("Spawner button pressed!");
        Instantiate(boxPrefab, new Vector3(spawner.transform.position.x, spawner.transform.position.y, spawner.transform.position.z), Quaternion.identity);
        SoundManager.Instance.PlaySoundClip(press, gameObject.transform, volume);
    }
}
