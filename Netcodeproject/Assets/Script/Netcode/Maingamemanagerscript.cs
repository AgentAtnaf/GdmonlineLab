using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Maingamemanagerscript : MonoBehaviour
{
    public void Hostclick()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void Serverclick()
    {
        NetworkManager.Singleton.StartServer();
    }
    public void Clientlick()
    {
        NetworkManager.Singleton.StartClient();
    }
}
