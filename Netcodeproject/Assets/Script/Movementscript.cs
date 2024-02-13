using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
public class Movementscript : NetworkBehaviour 
{
   public TMP_Text namePrefab;
   public TMP_Text namelabel;
   public TMP_InputField playerNameInput;
   public Material materialToChange;
   Rigidbody rb;
   public float speed = 5.0f;
   public float rotationspeed = 10.0f;
   private NetworkVariable<int> posX = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);
   private NetworkVariable<bool> isColorChanged = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);     
   private NetworkVariable<Color> materialColor = new NetworkVariable<Color>(Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);
   // private bool isColorChanged = false;
   private void FixedUpdate()
   {
        if(IsOwner)
         {
            float translation = Input.GetAxis("Vertical") * speed;
            float rotation = Input.GetAxis("Horizontal") * rotationspeed;
            translation *= Time.deltaTime;
            Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
            rb.MovePosition(rb.position + this.transform.forward * translation);
            rb.MoveRotation(rb.rotation * turn);
         }
   }
   public NetworkVariable<NetworkString> playerNameA = new NetworkVariable<NetworkString>(
      new NetworkString{ info = "Player"},
      NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);
   public NetworkVariable<NetworkString> playerNameB = new NetworkVariable<NetworkString>(
      new NetworkString{ info = "Player"},
      NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);
   private Loginmanager loginmanager;
   public override void OnNetworkSpawn()
   {
      GameObject canvas = GameObject.FindWithTag("Maincanvas");
      namelabel = Instantiate(namePrefab, Vector3.zero, Quaternion.identity) as TMP_Text;
      namelabel.transform.SetParent(canvas.transform);
      posX.OnValueChanged += (int previousValue, int Newvalue) =>
      {
         Debug.Log("Owner ID = " + OwnerClientId + " :Pos X =" + posX.Value);
      };
      // if(IsOwner)
      // {
      //    playerNameA.Value = new NetworkString { info = new FixedString32Bytes("Player1")};
      //    playerNameB.Value = new NetworkString { info = new FixedString32Bytes("Player2")};
      // }
      if(IsOwner)
      {
         loginmanager = GameObject.FindObjectOfType<Loginmanager>();
         if(loginmanager != null)
         {
            string name = loginmanager.userNameInputField.text;
            if(IsOwnedByServer){ playerNameA.Value = name;}
            else{playerNameB.Value = name;}
         }
      }
   }
      void Start()
   {
      rb = this.GetComponent<Rigidbody>();
   }
   public struct NetworkString : INetworkSerializable
   {
      public FixedString32Bytes info;

      public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
      {
         serializer.SerializeValue(ref info);
      }

      public override string ToString()
      {
         return info.ToString();
      }

      public static implicit operator NetworkString(string v) =>
         new NetworkString() { info = new FixedString32Bytes(v) };
   }
   void Update()
   {
        HandleKeyboardInput();
        Vector3 namelabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 4f, 0));
        namelabel.text = gameObject.name;
        namelabel.transform.position = namelabelPos;
        if(IsOwner)
        {
            posX.Value = (int)System.Math.Ceiling(transform.position.x);
            if(Input.GetKeyDown(KeyCode.K))
            {
               TestserverRpc("KO", new ServerRpcParams());
            }
            if(Input.GetKeyDown(KeyCode.L))
            {
               ClientRpcSendParams clientRpcSendParams = new ClientRpcSendParams {TargetClientIds = new List<ulong>{1}};
               ClientRpcParams clientRpcParams = new ClientRpcParams{ Send = clientRpcSendParams};
               TestClientRpc("Ok", clientRpcParams);
            }
        }
        Upadteplayerinfo();
        UpdateMaterialColor();
   }
   [ClientRpc]
   private void TestClientRpc(string msg, ClientRpcParams clientRpcParams)
   {
      Debug.Log("Msg from Server = " + msg);
   }
   [ServerRpc]
   private void TestserverRpc(string msg, ServerRpcParams serverRpcParams)
   {
      Debug.Log("Test srever rpc from client = +" + OwnerClientId);
   }
    private void UpdateMaterialColor()
    {
        if (materialToChange != null)
        {
            // Set the material color to the synchronized value
            materialToChange.color = materialColor.Value;
        }
        else
        {   
            Debug.LogError("Material reference is null. Please assign a material to 'materialToChange'.");
        }
    }

   private void Upadteplayerinfo()
   {
      if(IsOwnedByServer) { namelabel.text = playerNameA.Value.ToString();}
      else{namelabel.text = playerNameB.Value.ToString();}
   }

   public override void OnDestroy()
   {
      if(namelabel != null) Destroy(namelabel.gameObject);
      base.OnDestroy();
   }

   private void HandleKeyboardInput()
    {
         if (IsOwner)
         {
            if (Input.GetKeyDown(KeyCode.F))
            {
                  Debug.Log("Material change color");
                  ChangeMaterialColor();
            }
         }
    }
    private void ChangeMaterialColor()
    {
        if (materialToChange != null)
        {
            Color newColor;

            if (isColorChanged.Value)
            {
                newColor = Color.white;
            }
            else
            {
                newColor = Color.red;
            }
            materialToChange.color = newColor;
            materialColor.Value = newColor;

            // Toggle the state
            isColorChanged.Value = !isColorChanged.Value;
        }
        else
        {
            Debug.LogError("Material reference is null. Please assign a material to 'materialToChange'.");
        }
    }
}
