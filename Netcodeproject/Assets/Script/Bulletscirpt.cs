using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletScript : NetworkBehaviour
{
    public BulletSpawner bulletSpawner;
    public float lifetime = 5.0f; // Adjust the lifetime as needed
    private float timer;

    private void Start()
    {
        timer = 0f;
    }

    private void Update()
    {
        // If the bullet has a limited lifetime, count down the timer
        if (IsOwner && lifetime > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                // Destroy the bullet if it has exceeded its lifetime
                ulong networkObjId = GetComponent<NetworkObject>().NetworkObjectId;
                bulletSpawner.DestroyProjectileServerRpc(networkObjId);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsOwner) return;
        if (other.gameObject.tag == "Player" )
        {
            ulong networkObjId = GetComponent<NetworkObject>().NetworkObjectId;
            Debug.Log("Hit.");
            bulletSpawner.DestroyProjectileServerRpc(networkObjId);
        }
    }
}
