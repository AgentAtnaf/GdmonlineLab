using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawnerScript : NetworkBehaviour
{
    public Loginmanager loginmanager;
    // Movementscript movementscript;
    public Behaviour[] scripts;
    private Renderer[] renderers;

    void Start()
    {
        // movementscript = gameObject.GetComponent<Movementscript>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void SetPlayerState(bool state)
    {
        foreach (var script in scripts) { script.enabled = state; }
        foreach (var renderer in renderers) { renderer.enabled = state; }
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 randomPos = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        return randomPos;
    }

    public void Respawn()
    {
        RespawnServerRpc();
    }

    [ServerRpc]
    void RespawnServerRpc()
    {
        Vector3 pos = GetRandomPosition();
        RespawnClientRpc(pos);
    }

    [ClientRpc]
    void RespawnClientRpc(Vector3 spawnPos)
    {
        // movementscript.enabled = false;
        // transform.position = spawnPos;
        // movementscript.enabled = true;
        StartCoroutine(RespawnCoroutine(spawnPos));
    }

    IEnumerator RespawnCoroutine(Vector3 spawnPos)
    {
        SetPlayerState(false);
        transform.position = spawnPos;
        yield return new WaitForSeconds(3f);
        SetPlayerState(true);
    }
}
