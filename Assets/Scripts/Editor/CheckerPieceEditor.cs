using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckerPiece))]
public class SpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Kick Piece"))
		{
			CheckerPiece piece = target as CheckerPiece;
			if (piece != null) piece.GetKicked(piece.testAngle);
		}
	}
}
