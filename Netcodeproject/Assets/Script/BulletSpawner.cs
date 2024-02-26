using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletSpawner : NetworkBehaviour
{
    public GameObject projectilePrefab; // Change the type to your projectile prefab
    private List<GameObject> spawnedProjectiles = new List<GameObject>();

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0)) // 0 corresponds to the left mouse button (Mouse 1)
        {
            SpawnProjectileServerRpc();
        }
    }

    [ServerRpc]
    void SpawnProjectileServerRpc()
    {
        // Calculate direction towards the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
        }
        else
        {
            // Use a default direction if the ray doesn't hit anything
            targetPosition = ray.GetPoint(50f);
        }

        Vector3 spawnPos = transform.position + (transform.forward * 2f) + (transform.up * 3.5f);
        Quaternion spawnRot = transform.rotation;
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, spawnRot);
        spawnedProjectiles.Add(projectile);
        projectile.GetComponent<BulletScript>().bulletSpawner = this;
        projectile.GetComponent<NetworkObject>().Spawn();

        // Calculate direction towards the mouse position
        Vector3 launchDirection = (targetPosition - spawnPos).normalized;

        // Add code here to launch the projectile towards the mouse position
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            float launchForce = 100f; // Adjust the launch force as needed
            projectileRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc(ulong networkObjId)
    {
        GameObject obj = FindSpawnedProjectile(networkObjId);
        if (obj == null) return;
        obj.GetComponent<NetworkObject>().Despawn();
        spawnedProjectiles.Remove(obj);
        Destroy(obj);
    }

    private GameObject FindSpawnedProjectile(ulong networkObjId)
    {
        foreach (GameObject projectile in spawnedProjectiles)
        {
            ulong projectileId = projectile.GetComponent<NetworkObject>().NetworkObjectId;
            if (projectileId == networkObjId) { return projectile; }
        }
        return null;
    }
}
