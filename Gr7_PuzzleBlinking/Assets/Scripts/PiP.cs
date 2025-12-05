using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;

public class PiP : NetworkBehaviour
{
    private GameObject[] players;
    [SerializeField] private RenderTexture pipHost;
    [SerializeField] private RenderTexture pipJoin;
    [SerializeField] private MonoBehaviour[] localOnlyScripts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Test")
        {
            GetPip();
        }

    }
    public void GetPip()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2)
        {
            //DisableLocalOnlyScripts();
            foreach (GameObject p in players)
            {
                if (p != gameObject)
                {
                    if (IsOwner)
                    {
                        Camera cam = p.GetComponentInChildren<Camera>();
                        cam.targetTexture = pipHost;
                    }

                    if (!IsOwner)
                    {
                        Camera cam = p.GetComponentInChildren<Camera>();
                        cam.targetTexture = pipJoin;
                    }
                }
            }
        }
    }

    private void DisableLocalOnlyScripts()
    {
        foreach (var script in localOnlyScripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        var characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }
}
