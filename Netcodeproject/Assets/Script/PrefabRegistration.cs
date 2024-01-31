// using UnityEngine;
// using Unity.Netcode;

// public class PrefabRegistration : MonoBehaviour
// {
//     public GameObject[] prefabs;
//     public uint[] prefabHashes;

//     void Start()
//     {
//         RegisterPrefabs();
//     }

//     void RegisterPrefabs()
//     {
//         if (prefabs.Length != prefabHashes.Length)
//         {
//             Debug.LogError("Prefab array length does not match prefabHashes array length");
//             return;
//         }

//         for (int i = 0; i < prefabs.Length; i++)
//         {
//             GameObject prefab = prefabs[i];
//             uint prefabHash = prefabHashes[i];

//             if (prefab != null)
//             {
//                 NetworkManager.Singleton.NetworkConfig.NetworkPrefabs.Add(new NetworkPrefab(prefab, prefabHash));
//                 Debug.Log($"Prefab {prefab.name} registered with hash {prefabHash}");
//             }
//             else
//             {
//                 Debug.LogError($"Prefab at index {i} is null");
//             }
//         }
//     }
// }
