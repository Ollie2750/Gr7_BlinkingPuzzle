using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class PiP : MonoBehaviour
{
    private GameObject[] players;
    [SerializeField] private RenderTexture pip;
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2)
        {
            foreach (GameObject p in players)
            {
                if (p != gameObject)
                {
                    Camera cam = p.GetComponentInChildren<Camera>();
                    cam.targetTexture = pip; 
                }
            }
        }
    }
}
