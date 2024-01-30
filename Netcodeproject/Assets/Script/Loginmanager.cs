using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QFSW.QC;
using Unity.Netcode;

public class Loginmanager : MonoBehaviour
{
    public TMP_InputField userNameInputField;
    public TMP_InputField SkinIDInputField;
    public TMP_InputField passwordInputField;
    public List<uint>Alternativeprefab;
    private string hostPassword;
    private bool isApproveConnection = false;
    public GameObject loginpanel;
    public GameObject leavebutton;

    [Command("set-approve")]
    public bool SetIsApproveConnection()
    {
        isApproveConnection = !isApproveConnection;
        return isApproveConnection;
    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        loginpanel.SetActive(true);
        leavebutton.SetActive(false);
    }

    public void Leave()
    {
        Debug.Log("leave");
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        loginpanel.SetActive(true);
        leavebutton.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    private void HandleServerStarted()
    {
        Debug.Log("HandleServerstarted");
    }

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log("HandlClientconnected Client ID " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginpanel.SetActive(false);
            leavebutton.SetActive(true);
        }
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        Debug.Log("HandlClientDisconnected Client ID " + clientId);
        if (NetworkManager.Singleton.IsHost) { }
        else if (NetworkManager.Singleton.IsClient) { Leave(); }

    }

    public void Host()
    {
        hostPassword = passwordInputField.text;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        Debug.Log("start host " + hostPassword);
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;

        var connectionData = request.Payload;
        int byteLength = connectionData.Length;
        bool isApproved = false;
        if (byteLength > 0)
        {
            string clientData = System.Text.Encoding.ASCII.GetString(connectionData, 0, byteLength);
            string hostData = userNameInputField.text;
            string hostDatapassword = hostPassword;  // stored host password

            // Check the values
            Debug.Log("Client data: " + clientData);
            Debug.Log("Host data: " + hostDatapassword);

            isApproved = ApproveConnection(clientData, hostDatapassword, hostData);
        }
        response.Approved = isApproved;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = Alternativeprefab[SkinIdindex];

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        setSpawnLocation(clientId, response);
        NetworkLog.LogInfoServer("Spawnpos of " + clientId + " is " + response.Position.ToString());

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Some reason for not approving the client";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    public void Client()
    {
        string userName = userNameInputField.text;
        string clientPassword = passwordInputField.text;
        string SkinId =  SkinIDInputField.text;
        string[] inputField = { userName,clientPassword,SkinId}

        // Combine user name and password into a single string
        string combinedData = userName + "," + clientPassword;

        // Set the client's data
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(combinedData);

        NetworkManager.Singleton.StartClient();
        Debug.Log("start client " + combinedData);
    }

    public bool ApproveConnection(string clientData, string hostPassword, string hostName)
    {
        // Split the combined data into user name and password
        string[] data = clientData.Split(',');

        // Check if the password is correct and the name is different from the host's name
        bool isPasswordApprove = string.Equals(data[1].Trim(), hostPassword.Trim());
        bool isNameApprove = !System.String.Equals(data[0].Trim(), hostName.Trim());

        return isPasswordApprove && isNameApprove;
    }

    private void setSpawnLocation(ulong clientId, NetworkManager.ConnectionApprovalResponse response)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            spawnPos = new Vector3(-1f, 2f, 3f);
            spawnRot = Quaternion.Euler(0f, 123f, 0f);
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                case 1:
                    spawnPos = new Vector3(0f, 0f, 0f);
                    spawnRot = Quaternion.Euler(0f, 110f, 0f);
                    break;
                case 2:
                    spawnPos = new Vector3(2f, 4f, 3f);
                    spawnRot = Quaternion.Euler(1f, 120f, 3f);
                    break;
            }
        }
        response.Position = spawnPos;
        response.Rotation = spawnRot;
    }
}
