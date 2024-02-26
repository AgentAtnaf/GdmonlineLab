using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bombscirpt : NetworkBehaviour
{
    public Bombspawner bombspawner;
    public GameObject effectPrefab;
    private void OnCollisionEnter(Collision other) {
        if(!IsOwner)return;
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Bullet")
        {
            ulong networkObjId = GetComponent<NetworkObject>().NetworkObjectId;
            SpawnEffect();
            bombspawner.DestrouServerRpc(networkObjId);
        }
    }
    private void SpawnEffect()
    {
        GameObject effect = Instantiate(effectPrefab, this.transform.position, Quaternion.identity);
        effect.GetComponent<NetworkObject>().Spawn();
    }
}
