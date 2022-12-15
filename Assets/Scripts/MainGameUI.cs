using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainGameUI : NetworkBehaviour
{
	[SerializeField] private GameManager gm;

	[ClientRpc]
	private void DisableUIClientRpc()
	{
		enabled = false;
	}
	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width / 2f - Screen.width / 4f, 20, Screen.width / 2f, 300));
		if (!IsServer)
		{
			GUILayout.Label("Waiting for host to start game.");
		}
		else
		{
			if (GUILayout.Button("Start Game"))
			{
				gm.SpawnPlayers();
				gm.SpawnPieces();
				DisableUIClientRpc();
				enabled = false;
			}
		}
		GUILayout.EndArea();
	}
}
