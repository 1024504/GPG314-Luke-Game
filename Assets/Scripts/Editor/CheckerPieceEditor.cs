using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckerPiece))]
public class CheckerPieceEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button(/*"Kick Piece"*/ "Uncomment code for function"))
		{
			CheckerPiece piece = target as CheckerPiece;
			// if (piece != null) piece.GetKicked(piece.testAngle);
		}
	}
}
