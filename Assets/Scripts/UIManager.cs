using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : NetworkBehaviour
{
	[SerializeField] private JoinUI joinUI;
	
	private string _playerName = "player";

	public string PlayerName
	{
		get => _playerName;
		set
		{
			if (value == _playerName) return;
			_playerName = value;
			if(IsSpawned) RequestNameChangeServerRpc(_networkManager.LocalClientId, _playerName);
		}
	}
	
	public readonly Dictionary<ulong, string> PlayerNames = new ();
	public int sceneCount;
	public List<string> scenes;
	public string selectedScene;
	
	private NetworkManager _networkManager;
	public UnityTransport transport;

	private void Start()
	{
		_networkManager = NetworkManager.Singleton;
		transport = _networkManager.GetComponent<UnityTransport>();
		sceneCount = SceneManager.sceneCountInBuildSettings;
		scenes = new(sceneCount);
		for (int i = 0; i < sceneCount; i++)
		{
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
			if (sceneName == "Lobby") continue;
			scenes.Add(sceneName);
		}
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		_networkManager.OnClientDisconnectCallback += RemovePlayerName;
		_networkManager.OnClientDisconnectCallback += ReturnToLobby;
		RequestNameChangeServerRpc(_networkManager.LocalClientId, _playerName);
	}
	
	[ServerRpc (RequireOwnership = false)]
	private void RequestNameChangeServerRpc(ulong clientId, string pName)
	{
		if (!PlayerNames.ContainsKey(clientId))
		{
			PlayerNames.Add(clientId, pName);
			foreach (ulong id in _networkManager.ConnectedClientsIds)
			{
				PopulatePlayersClientRpc(id, PlayerNames[id]);
			}
			return;
		}
		if (PlayerNames[clientId] == pName) return;
		PlayerNames[clientId] = pName;
		foreach (ulong id in _networkManager.ConnectedClientsIds)
		{
			PopulatePlayersClientRpc(id, PlayerNames[id]);
		}
	}

	[ClientRpc]
	private void PopulatePlayersClientRpc(ulong clientId, string pName)
	{
		if (!PlayerNames.ContainsKey(clientId))
		{
			PlayerNames.Add(clientId, pName);
			return;
		}
		PlayerNames[clientId] = pName;
	}

	[ClientRpc]
	public void StartGameClientRpc()
	{
		joinUI.enabled = false;
	}

	private void RemovePlayerName(ulong clientId)
	{
		PlayerNames.Remove(clientId);
	}

	private void ReturnToLobby(ulong clientId)
	{
		if (joinUI.enabled)
		{
			joinUI.Back();
		}
	}
}
