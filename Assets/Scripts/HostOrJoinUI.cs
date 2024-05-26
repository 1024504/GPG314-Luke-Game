using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostOrJoinUI : MonoBehaviour
{
	[SerializeField] private UIManager uiManager;
	[SerializeField] private HostUI hostUI;
	[SerializeField] private JoinUI joinUI;

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/2f - Screen.width/4f, 20, Screen.width/2f, 300));
		
		if (GUILayout.Button("Host"))
		{
			Host();
		}
		
		uiManager.transport.ConnectionData.Address = GUILayout.TextField(uiManager.transport.ConnectionData.Address, 22);

		if (uiManager.transport.ConnectionData.Address != "")
		{
			if (GUILayout.Button("Join"))
			{
				Join();
			}
		}

		GUILayout.BeginHorizontal();
		if (uiManager.PlayerName == "")
		{
			GUILayout.Label("Type your name -->");
		}

		uiManager.PlayerName = GUILayout.TextField(uiManager.PlayerName, 22);
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}

	private void Host()
	{
		NetworkManager.Singleton.StartHost();
		hostUI.enabled = true;
		enabled = false;
	}

	private void Join()
	{
		NetworkManager.Singleton.StartClient();
		joinUI.enabled = true;
		enabled = false;
	}
}
