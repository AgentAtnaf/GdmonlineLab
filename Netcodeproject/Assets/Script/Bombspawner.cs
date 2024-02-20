using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bombspawner : NetworkBehaviour
{
   public GameObject bombprefabs;
   private List<GameObject> spawnedBomb = new List<GameObject>();
   void Update()
   {
     if(!IsOwner)return;
     if(Input.GetKeyDown(KeyCode.V))
     {
        SpawBombServerRpc();
     }
   }
   [ServerRpc]
   void SpawBombServerRpc()
   {
        Vector3 spawnPos = transform.position + (transform.forward * 3.5f) + (transform.up * 3.5f);
        Quaternion spawnRot = transform.rotation;
        GameObject bomb = Instantiate(bombprefabs, spawnPos, spawnRot);
        spawnedBomb.Add(bomb);
        bomb.GetComponent<Bombscirpt>().bombspawner = this;
        bomb.GetComponent<NetworkObject>().Spawn();

   }
   [ServerRpc(RequireOwnership = false)]
   public void DestrouServerRpc(ulong networkObjId)
   {
    GameObject obj = findSpawnedBomb(networkObjId);
    if(obj == null)return;
    obj.GetComponent<NetworkObject>().Despawn();
    spawnedBomb.Remove(obj); Destroy(obj);
   }
   private GameObject findSpawnedBomb(ulong networkObjId)
   {
    foreach(GameObject bomb in spawnedBomb)
    {
        ulong bombId = bomb.GetComponent<NetworkObject>().NetworkObjectId;
        if(bombId == networkObjId){return bomb;}
    }
    return null;
   }
}
