using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        /*if (Input.GetKeyDown(KeyCode.Space) && IsServer)
        {
	        LoadSceneClientRpc("Test Scene 2");
        }*/
    }

    [ClientRpc]
    void LoadSceneClientRpc(string sceneName)
    {
	    _networkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    void SpawnPieces()
    {
	    if (!IsServer || piecesSpawned) return;
	    piecesSpawned = true;
	    GameObject go = Instantiate(whitePieces);
	    go.GetComponent<NetworkObject>().Spawn();
	    
	    go = Instantiate(blackPieces);
	    go.GetComponent<NetworkObject>().Spawn();
    }
}
