using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject whitePieces;
    [SerializeField] private GameObject blackPieces;

    private NetworkManager _networkManager;

    private void Start()
    {
	    _networkManager = NetworkManager.Singleton;
        SpawnPlayers();
	    SpawnPieces();
        _networkManager.OnClientConnectedCallback += SpawnLatePlayer;
    }
    
    private void SpawnPlayers()
    {
        if (!IsServer) return;
        
        foreach (ulong id in _networkManager.ConnectedClientsIds)
        {
            GameObject go = Instantiate(player);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(id);
            go.GetComponent<IControllable>().AssignToClientEntityClientRpc(id);
        }
    }

    private void SpawnPieces()
    {
	    if (!IsServer) return;
	    GameObject go = Instantiate(whitePieces);
	    go.GetComponent<NetworkObject>().Spawn();
	    
	    go = Instantiate(blackPieces);
	    go.GetComponent<NetworkObject>().Spawn();
    }

    private void SpawnLatePlayer(ulong clientId)
    {
        GameObject go = Instantiate(player);
        go.GetComponent<NetworkObject>().Spawn();
    }
}
