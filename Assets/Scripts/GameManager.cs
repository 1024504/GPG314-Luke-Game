using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
	[SerializeField] private GameObject whitePieces;
    [SerializeField] private GameObject blackPieces;

    private NetworkManager _networkManager;

    public bool piecesSpawned;

    void Start()
    {
	    _networkManager = NetworkManager.Singleton;
	    _networkManager.OnServerStarted += SpawnPieces;
    }

    void Update()
    {
        if (!IsServer) return;
        if (Input.GetKeyDown(KeyCode.Space) && !piecesSpawned)
        {
	        piecesSpawned = true;
            SpawnPieces();
        }
    }

    void SpawnPieces()
    {
	    if (!IsServer) return;
	    GameObject go = Instantiate(whitePieces);
	    go.GetComponent<NetworkObject>().Spawn();
	    
	    go = Instantiate(blackPieces);
	    go.GetComponent<NetworkObject>().Spawn();
    }
}
