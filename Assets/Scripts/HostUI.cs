using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostUI : MonoBehaviour
{
	[SerializeField] private UIManager uiManager;
	[SerializeField] private HostOrJoinUI hostOrJoinUI;

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2f - Screen.width/2.5f, 20, Screen.width*2/2.5f, 300));
		if (uiManager.selectedScene != "")
		{
			if (GUILayout.Button("Start Game"))
            {
	            NetworkManager.Singleton.SceneManager.LoadScene(uiManager.selectedScene,LoadSceneMode.Additive);
                enabled = false;
                uiManager.StartGameClientRpc();
            }
		}
		else
		{
			GUILayout.Label("Select a level");
		}

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
		
		GUILayout.BeginVertical();
		
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
		
		GUILayout.EndHorizontal();
		
		foreach (string sceneName in uiManager.scenes)
		{
			if (GUILayout.Button(sceneName)) uiManager.selectedScene = sceneName;
		}
		
		GUILayout.EndVertical();
		
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
