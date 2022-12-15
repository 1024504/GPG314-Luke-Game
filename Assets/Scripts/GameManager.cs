using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Unity.Jobs;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;

public class GameManager : NetworkBehaviour
{
	private bool _checkingMoves = false;
	public bool movesAvailable = true;
	
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject whitePiecesPrefab;
    [SerializeField] private GameObject blackPiecesPrefab;

    private Transform _whitePieces;
    private Transform _blackPieces;
    private int _totalPieces;

    public int numberOfRanks = 8;
    public int numberOfFiles = 8;
    
    public float tileLength = 10f;
    public float tileWidth = 10f;

    private JobHandle _handles;
    public NativeArray<int> OccupancyArray;
    private NativeList<int> _ranks;
    private NativeList<int> _files;
    private NativeList<CheckerPiece.TeamColour> _teams;
    private NativeList<bool> _isKings;

    public CheckerPiece[] checkerPieces;

    public static GameManager Singleton { get; private set; }
    private NetworkManager _networkManager;

    private NativeArray<bool> _movesAvailable;

    private void Awake()
    {
	    Singleton = this;
    }

    private void Start()
    {
	    _networkManager = NetworkManager.Singleton;
        SpawnPlayers();
	    SpawnPieces();
        _networkManager.OnClientConnectedCallback += SpawnLatePlayer;
        checkerPieces = new CheckerPiece[OccupancyArray.Length];
    }

    private void OnEnable()
    {
	    //Using 1D array as 2D array, index = y*width+x
	    OccupancyArray = new NativeArray<int>(numberOfRanks*numberOfFiles, Allocator.Persistent);
    }

    private void OnDisable()
    {
	    OccupancyArray.Dispose();
	    _movesAvailable.Dispose();
	    _ranks.Dispose();
	    _files.Dispose();
	    _teams.Dispose();
	    _isKings.Dispose();
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
	    GameObject go = Instantiate(whitePiecesPrefab);
	    go.GetComponent<NetworkObject>().Spawn();
	    _whitePieces = go.transform;
	    
	    go = Instantiate(blackPiecesPrefab);
	    go.GetComponent<NetworkObject>().Spawn();
	    _blackPieces = go.transform;

	    _totalPieces = _whitePieces.childCount+_blackPieces.childCount;
	    _ranks = new NativeList<int>(_totalPieces, Allocator.Persistent);
	    _files = new NativeList<int>(_totalPieces, Allocator.Persistent);
	    _isKings = new NativeList<bool>(_totalPieces, Allocator.Persistent);
	    _teams = new NativeList<CheckerPiece.TeamColour>(_totalPieces, Allocator.Persistent);
	    _movesAvailable = new NativeArray<bool>(_totalPieces, Allocator.Persistent);
    }

    private void SpawnLatePlayer(ulong clientId)
    {
        GameObject go = Instantiate(player);
        go.GetComponent<NetworkObject>().Spawn();
    }

    //Check both teams have players;
    public bool CheckTeamsAlive()
    {
	    for (int i = 0; i < _whitePieces.childCount; i++)
	    {
		    if (_whitePieces.GetChild(i).gameObject.activeSelf) return true;
		    if (_blackPieces.GetChild(i).gameObject.activeSelf) return true;
	    }
	    return false;
    }

    //Check pieces have moves;
    public void CheckAvailableMoves()
    {
	    _ranks.Clear();
	    _files.Clear();
	    _teams.Clear();
	    _isKings.Clear();

	    CheckerPiece[] whitePieces = _whitePieces.GetComponentsInChildren<CheckerPiece>();
	    CheckerPiece[] blackPieces = _blackPieces.GetComponentsInChildren<CheckerPiece>();
	    _totalPieces = whitePieces.Length + blackPieces.Length;

	    for (int i = 0; i < _totalPieces; i++)
	    {
		    if (i < whitePieces.Length)
		    {
			    CheckerPiece piece = whitePieces[i].GetComponent<CheckerPiece>();
			    _ranks.Add(piece.rank);
			    _files.Add(piece.file);
			    _teams.Add(piece.team);
			    _isKings.Add(piece.IsKing);
		    }
		    else
		    {
			    CheckerPiece piece = blackPieces[i-whitePieces.Length].GetComponent<CheckerPiece>();
			    _ranks.Add(piece.rank);
			    _files.Add(piece.file);
			    _teams.Add(piece.team);
			    _isKings.Add(piece.IsKing);
		    }
	    }
	    
	    CheckAvailableMovesJob job = new CheckAvailableMovesJob
	    {
		    IsKings = _isKings,
		    Ranks = _ranks,
		    Files = _files,
		    Teams = _teams,
		    NumberOfFiles = numberOfFiles,
		    NumberOfRanks = numberOfRanks,
		    OccupancyArray = OccupancyArray,
		    HasMoves = _movesAvailable
	    };

	    _checkingMoves = true;
	    _handles = job.Schedule(_totalPieces, 24);
    }

    private void LateUpdate()
    {
	    if (!_checkingMoves) return;
	    _checkingMoves = false;
	    _handles.Complete();
	    bool moveAvailable = false;
	    for (int i = 0; i < _totalPieces; i++)
	    {
		    if (_movesAvailable[i])
		    {
			    moveAvailable = true;
			    //end game
			    break;
		    }
	    }
	    movesAvailable = moveAvailable; // could be redundant
    }

    private void OnDrawGizmosSelected()
    {
	    for (int i=0; i<OccupancyArray.Length; i++)
	    {
		    if (OccupancyArray[i] == 0)
		    {
			    Gizmos.color = new Color(1,0,0,0.5f);
			    Gizmos.DrawCube(new Vector3(i%8*tileWidth-3.5f*tileWidth, 1, (i-i%8)*tileLength/8-3.5f*tileLength), new Vector3(tileWidth,1,tileLength));
		    }
		    else if (OccupancyArray[i] == 1)
		    {
			    Gizmos.color = new Color(0,1,0,0.5f);
			    Gizmos.DrawCube(new Vector3(i%8*tileWidth-3.5f*tileWidth, 1, (i-i%8)*tileLength/8-3.5f*tileLength), new Vector3(tileWidth,1,tileLength));
		    }
		    else
		    {
			    Gizmos.color = new Color(1,0,1,0.5f);
			    Gizmos.DrawCube(new Vector3(i%8*tileWidth-3.5f*tileWidth, 1, (i-i%8)*tileLength/8-3.5f*tileLength), new Vector3(tileWidth,1,tileLength));
		    }
	    }
    }
}
