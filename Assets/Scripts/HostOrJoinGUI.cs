using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class HostOrJoinGUI : MonoBehaviour
{
	[SerializeField] private HostGUI hostGui;
	// [SerializeField] private JoinGUI joinGui;

	public string playerName;
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2f - Screen.width/4f, 20, Screen.width/2f, 300));
		
		if (GUILayout.Button("Host"))
		{
			hostGui.enabled = true;
			hostGui.playerName = playerName;
			enabled = false;
		}

		if (GUILayout.Button("Join"))
		{
			Debug.Log("Load Client Scene");
		}

		GUILayout.BeginHorizontal();
		if (playerName == "")
		{
			GUILayout.Label("Type your name -->");
		}

		playerName = GUILayout.TextField(playerName, 22);
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}
}
