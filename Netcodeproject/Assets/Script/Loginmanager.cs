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
    public TMP_Dropdown skinDropdown;
    private Dictionary<int, uint> dropdownToPrefabHashMap = new Dictionary<int, uint>();
    public uint selectedPrefabHash;
    private string hostPassword;
    public Transform[] spawnPoints;
    private bool isApproveConnection = false;
    public GameObject loginpanel;
    public GameObject leavebutton;

    public GameObject Scorepannel;

    [Command("set-approve")]
    public bool SetIsApproveConnection()
    {
        isApproveConnection = !isApproveConnection;
        return isApproveConnection;
    }
    void Start()
    {
        PopulateDropdownOptions();
        PopulateDropdownToPrefabHashMap();
        selectedPrefabHash = 1217761731;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        loginpanel.SetActive(true);
        leavebutton.SetActive(false);
        Scorepannel.SetActive(false);
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
        Scorepannel.SetActive(false);
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
            Scorepannel.SetActive(true);
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
            // string Clientname = userNameInputField.text;
            string hostDatapassword = hostPassword;  // stored host password
            string[] data = clientData.Split(',');

            // Check the values
            Debug.Log("Client data: " + clientData);
            Debug.Log("Host data: " + hostDatapassword);

            isApproved = ApproveConnection(clientData, hostDatapassword, hostData);
            response.PlayerPrefabHash = uint.Parse(data[2]);
        }
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            response.PlayerPrefabHash = selectedPrefabHash;
        }
        Debug.Log("selectedPrefabHash = " + selectedPrefabHash);
        response.Approved = isApproved;
        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        // response.PlayerPrefabHash = null;
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

        // Combine user name, password, and selectedPrefabHash into a single string
        string combinedData = userName + "," + clientPassword + "," + selectedPrefabHash.ToString();

        // Set the client's data
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(combinedData);

        NetworkManager.Singleton.StartClient();
        Debug.Log("start client " + combinedData);
    }

    private void PopulateDropdownOptions()
    {
        // Clear existing options
        skinDropdown.ClearOptions();

        // Replace this with your actual dropdown options
        List<string> dropdownOptions = new List<string> { "Red", "Green", "Blue", "Skin" };

        // Add options to the dropdown
        skinDropdown.AddOptions(dropdownOptions);
    }
    private void PopulateDropdownToPrefabHashMap()
    {
        // Replace these values with your actual prefab hashes
        dropdownToPrefabHashMap.Add(0,1217761731);
        dropdownToPrefabHashMap.Add(1,2044945768);
        dropdownToPrefabHashMap.Add(2,1580094032);
        dropdownToPrefabHashMap.Add(3,4120045818);

        // Add more mappings as needed
    }
     public void OnDropdownValueChanged()
    {
        // Get the selected index from the dropdown
        int selectedIndex = skinDropdown.value;

        // Check if the selected index is within the bounds of the mapping
        if (dropdownToPrefabHashMap.ContainsKey(selectedIndex))
        {
            // Assign the selected prefab hash to a field for later use
            selectedPrefabHash = dropdownToPrefabHashMap[selectedIndex];
        }
        else
        {
            // Handle an invalid index if needed
            Debug.Log("Invalid dropdown index");
        }
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

    public void setSpawnLocation(ulong clientId, NetworkManager.ConnectionApprovalResponse response)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Randomly select a spawn point from the array
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = randomSpawnPoint.position;
            spawnRot = randomSpawnPoint.rotation;
        }
        else
        {
            switch (NetworkManager.Singleton.ConnectedClients.Count)
            {
                default:
                    // Handle cases where the player count is greater than 3
                    Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    spawnPos = randomSpawnPoint.position;
                    spawnRot = randomSpawnPoint.rotation;
                    spawnRot = Quaternion.Euler(0f, 0f, 0f);
                    break;
            }
        }
        response.Position = spawnPos;
        response.Rotation = spawnRot;
    }
}
