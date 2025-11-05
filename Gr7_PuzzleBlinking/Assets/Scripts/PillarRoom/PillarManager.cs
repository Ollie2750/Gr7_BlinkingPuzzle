using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Numerics;

public class PillarManager : MonoBehaviour
{
    [Header("Room Bounds (BoxCollider matching floor area)")]
    public BoxCollider roomBounds;

    [Header("Spawn Settings")]
    public float dropHeight = 3f;           //how high above floor to spawn
    public float edgeMargin = 0.6f;         //keep away from walls
    public float minSpacing = 0.9f;         //avoid overlap with other pillars
    public int maxTries = 20;


    //track spawned pillars to keep spacing
    readonly List<Transform> spawned = new();

    public GameObject SpawnPillar(GameObject pillarPrefab)
    {
        if (!roomBounds || !pillarPrefab) { Debug.LogWarning("PillarManager not set up"); return null; }

        UnityEngine.Vector3 pos;
        if (!TryPickSpot(out pos)) return null;

        //spawn upright above the spot
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject pillar = Instantiate(pillarPrefab, pos + UnityEngine.Vector3.up * dropHeight, rot);
        spawned.Add(pillar.transform);

        //wake physics & make it fall straight down while staying upright
        var drop = pillar.GetComponent<PillarDrop>();
        if (drop) drop.Kick(1f);

        return pillar;
    }

   public void Despawn(GameObject pillar)
    {
        if (!pillar) return;
        spawned.Remove(pillar.transform);
        Destroy(pillar);
    }

    //same-spot respawn helper
    public GameObject SpawnAtSameXZ(GameObject pillarPrefab, UnityEngine.Vector3 oldXZ)
    {
        if (!roomBounds || !pillarPrefab) return null;

        Bounds b = roomBounds.bounds;
        float xmin = b.min.x + edgeMargin, xmax = b.max.x - edgeMargin;
        float zmin = b.min.z + edgeMargin, zmax = b.max.z - edgeMargin;

        UnityEngine.Vector3 target = new UnityEngine.Vector3(
            Mathf.Clamp(oldXZ.x, xmin, xmax),
            b.min.y,
            Mathf.Clamp(oldXZ.z, zmin, zmax)
        );

        // keep spacing from other pillars; if too close, fall back to random
        foreach (var t in spawned)
        {
            if (!t) continue;
            if (UnityEngine.Vector2.Distance(new UnityEngine.Vector2(target.x, target.z),
                                 new UnityEngine.Vector2(t.position.x, t.position.z)) < minSpacing)
            {
                return SpawnPillar(pillarPrefab);
            }
        }

        var rot = UnityEngine.Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        var go  = Instantiate(pillarPrefab, target + UnityEngine.Vector3.up * dropHeight, rot);
        spawned.Add(go.transform);
        var drop = go.GetComponent<PillarDrop>();
        if (drop) drop.Kick(1f);
        return go;
    }
    bool TryPickSpot(out UnityEngine.Vector3 spot)
    {
        Bounds b = roomBounds.bounds;
        float xmin = b.min.x + edgeMargin, xmax = b.max.x - edgeMargin;
        float zmin = b.min.z + edgeMargin, zmax = b.max.z - edgeMargin;

        for (int i = 0; i < maxTries; i++)
        {
            UnityEngine.Vector3 candidate = new(Random.Range(xmin, xmax), b.center.y, Random.Range(zmin, zmax));
            if (IsFree(candidate)) { spot = candidate; return true; }
        }

        //fallback: center
        spot = new UnityEngine.Vector3(Mathf.Clamp(b.center.x, xmin, xmax), b.center.y, Mathf.Clamp(b.center.z, zmin, zmax));
        return true;
    }

    bool IsFree(UnityEngine.Vector3 p)
    {
        //ensure spacing from all spawned pillars (XZ distance)
        foreach (var t in spawned)
        {
            if (!t) continue;
            UnityEngine.Vector2 a = new UnityEngine.Vector2(p.x, p.z);
            UnityEngine.Vector2 b = new UnityEngine.Vector2(t.position.x, t.position.z);
            if (UnityEngine.Vector2.Distance(a, b) < minSpacing) return false;
        }
        return true;
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.P))
    {
        Debug.Log("PillarManager: DEBUG spawn via 'P' key");
        // drag your prefab into this slot in the inspector for a minute, or cache it some other way
        // or expose a public GameObject debugPrefab;
        // SpawnPillar(debugPrefab);
    }
}
}
