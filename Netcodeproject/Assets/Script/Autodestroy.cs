using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Autodestroy : NetworkBehaviour
{
    public float delay = 1f;
    private ParticleSystem ps;
    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    public void Update()
    {
        if(!IsOwner) return;
        if(ps && !ps.IsAlive())
        {
            DestroyPbject();
        }
    }
    void DestroyPbject()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject, delay);
    }
}
