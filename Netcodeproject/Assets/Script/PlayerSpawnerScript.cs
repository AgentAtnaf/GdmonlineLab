using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnerScript : NetworkBehaviour
{
    public Loginmanager loginmanager;
    Movementscript movementscript;
    void Start()
    {
        movementscript = gameObject.GetComponent<Movementscript>();
    }
    private Vector3 Getrandomposition()
    {
        Vector3 randompos = new Vector3(Random.Range(-3, 3f), 1f, Random.Range(-3f, 3f));
        return randompos;
    }
    public void Respawn()
    {
        RespawnServerRpc();
    }
    [ServerRpc]
    void RespawnServerRpc()
    {
        Vector3 pos = Getrandomposition();
        RespawnClientRpc(pos);
    }
    [ClientRpc]
    void RespawnClientRpc(Vector3 spawnPos)
    {
        movementscript.enabled = false;
        transform.position = spawnPos;
        movementscript.enabled = true;
    }
}
