using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HostGUI : MonoBehaviour
{
	[SerializeField] private HostOrJoinGUI hostOrJoinGui;
	public string playerName;
	public int sceneCount;
	public List<string> scenes;
    private NetworkManager _networkManager;

	public string selectedScene;

	public List<string> playerNames = new ();
	
	private void OnEnable()
    {
        _networkManager = NetworkManager.Singleton;
		sceneCount = SceneManager.sceneCountInBuildSettings;
		scenes = new(sceneCount);
        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName == "Lobby") continue;
			scenes.Add(sceneName);
		}
		playerNames.Add(playerName);
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2f - Screen.width/2.5f, 20, Screen.width*2/2.5f, 300));
		if (selectedScene != "")
		{
			if (GUILayout.Button("Start Game"))
            {
                _networkManager.SceneManager.LoadScene(selectedScene,LoadSceneMode.Additive);
            }
		}
		else
		{
			GUILayout.Label("Select a level");
		}

		GUILayout.Space(20);

		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		
		if (playerName == "")
		{
			GUILayout.Label(" Enter your name");
		}
		
		playerName = GUILayout.TextField(playerName, 22);

		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();

		GUILayout.Label("Level: ");

		if (selectedScene == "")
		{
			GUILayout.Label("None");
		}
		else
		{
			GUILayout.Label(selectedScene);
		}
		
		GUILayout.EndHorizontal();
		
		foreach (string sceneName in scenes)
		{
			if (GUILayout.Button(sceneName)) selectedScene = sceneName;
		}
		
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical();

		foreach (string pName in playerNames)
		{
			GUILayout.Label(pName);
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(20);

		if (GUILayout.Button("Back"))
		{
			hostOrJoinGui.enabled = true;
			hostOrJoinGui.playerName = playerName;
			enabled = false;
		}

		GUILayout.EndVertical();

		GUILayout.EndArea();
	}
}
