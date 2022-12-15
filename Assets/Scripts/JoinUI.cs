using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class JoinUI : MonoBehaviour
{
	[SerializeField] private UIManager uiManager;
	[SerializeField] private HostOrJoinUI hostOrJoinUI;

	void OnGUI()
	{
		if (!uiManager.IsSpawned) return;
		GUILayout.BeginArea(new Rect(Screen.width/2f - Screen.width/2.5f, 20, Screen.width*2/2.5f, 300));

		GUILayout.Space(20);

		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		if (uiManager.PlayerName == "")
		{
			GUILayout.Label("Type your name -->");
		}

		uiManager.PlayerName = GUILayout.TextField(uiManager.PlayerName, 22);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();

		GUILayout.Label("Level: ");

		if (uiManager.selectedScene == "")
		{
			GUILayout.Label("None");
		}
		else
		{
			GUILayout.Label(uiManager.selectedScene);
		}
		
		GUILayout.BeginVertical();

		foreach (KeyValuePair<ulong, string> playerName in uiManager.PlayerNames)
		{
			GUILayout.Label(playerName.Value);
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);

		if (GUILayout.Button("Back"))
		{
			NetworkManager.Singleton.Shutdown();
			hostOrJoinUI.enabled = true;
			enabled = false;
		}

		GUILayout.EndVertical();

		GUILayout.EndArea();
	}
}
