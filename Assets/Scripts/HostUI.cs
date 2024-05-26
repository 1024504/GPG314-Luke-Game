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
		GUILayout.BeginArea(new Rect(Screen.width/2f - Screen.width/2.5f, 20, Screen.width*2/2.5f, 300));//1
		
		if (uiManager.selectedScene != "")
		{
			if (GUILayout.Button("Start Game"))
            {
	            StartGame();
            }
		}
		else
		{
			GUILayout.Label("Select a level");
		}

		GUILayout.Space(20);

		GUILayout.BeginVertical();//2
		
		GUILayout.BeginHorizontal();//3
		
		if (uiManager.PlayerName == "")
		{
			GUILayout.Label("Type your name -->");
		}

		uiManager.PlayerName = GUILayout.TextField(uiManager.PlayerName, 22);
		
		GUILayout.EndHorizontal();//3
		
		GUILayout.BeginHorizontal();//4
		
		GUILayout.BeginVertical();//5
		
		GUILayout.BeginHorizontal();//6

		GUILayout.Label("Level: ");

		if (uiManager.selectedScene == "")
		{
			GUILayout.Label("None");
		}
		else
		{
			GUILayout.Label(uiManager.selectedScene);
		}
		
		GUILayout.EndHorizontal();//6
		
		foreach (string sceneName in uiManager.scenes)
		{
			if (GUILayout.Button(sceneName)) uiManager.selectedScene = sceneName;
		}
		
		GUILayout.EndVertical();//5
		
		GUILayout.BeginVertical();//7

		foreach (KeyValuePair<ulong, string> playerName in uiManager.PlayerNames)
		{
			GUILayout.Label(playerName.Value);
		}
		
		GUILayout.EndVertical();//7
		
		GUILayout.EndHorizontal();//4
		
		GUILayout.Space(20);

		if (GUILayout.Button("Back"))
		{
			Back();
		}

		GUILayout.EndVertical();//2

		GUILayout.EndArea();//1
	}

	private void StartGame()
	{
		NetworkManager.Singleton.SceneManager.LoadScene(uiManager.selectedScene,LoadSceneMode.Additive);
		enabled = false;
		uiManager.StartGameClientRpc();
	}

	private void Back()
	{
		NetworkManager.Singleton.Shutdown();
		hostOrJoinUI.enabled = true;
		enabled = false;
	}
}
