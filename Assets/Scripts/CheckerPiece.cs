using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPiece : MonoBehaviour, IKickable
{
	///for testing
	public float testAngle;

	public enum TeamColour
	{
		White,
		Black
	}

	public TeamColour team;
	public bool isKing;
	private Transform _transform;
	private bool[] _targetOccupation;

	private void Start()
	{
		_transform = transform;
	}

	public void GetKicked(float angle)
	{
		if (!isKing && IsMovingBackwards(angle)) return;
		Vector3 target = GetTarget(angle);
		if (IsOutOfBounds(target)) return;
		_targetOccupation = IsOccupiedByEnemy(target);
		if (!_targetOccupation[0])
		{
			MoveSingle(target);
		}
		else if (_targetOccupation[1])
		{
			target += target - _transform.position;
			if (IsOutOfBounds(target)) return;
			_targetOccupation = IsOccupiedByEnemy(target);
			if (!_targetOccupation[0]) MoveDouble(target);
		}
		else return;
		
		// Check for continued conquering
		
		// Check for King (Need to account for moving backwards.)


		//get target
		//check edge of board
		//check if target occupied
		//	move
		//		check if end of board
		//			king
		//	check if occupied by enemy
		//		check next square for edge of board
		//		check next square for occupied
		//		move over piece
		//			call destroy method

		// separate the view so that I can instantly teleport pieces to prevent two tiles moving to the same tile
	}

	private Vector3 GetTarget(float angle)
    {
	    float xDiff = 10 * Mathf.Sign(Mathf.Sin(Mathf.Deg2Rad*angle));
	    float zDiff = 10 * Mathf.Sign(Mathf.Cos(Mathf.Deg2Rad*angle));

	    Vector3 position = _transform.position;
	    Vector3 target = new Vector3(position.x + xDiff, position.y, position.z + zDiff);
	    
	    return target;
    }

	private bool IsMovingBackwards(float angle)
	{
		float zDiff = 10 * Mathf.Sign(Mathf.Cos(Mathf.Deg2Rad*angle));
		if (team == TeamColour.White) return zDiff < 0;
		return zDiff > 0;
	}

    private bool IsOutOfBounds(Vector3 target)
    {
	    if (target.x > 40)	return true;
	    if (target.x < -40)	return true;
	    if (target.z > 40)	return true;
	    return target.z < -40;
    }

    private bool[] IsOccupiedByEnemy(Vector3 target)
    {
	    bool[] res = new bool[2]
	    {
		    false, // Is the space occupied?
		    false  // Is it occupied by an enemy?
	    };
	    
	    foreach (Collider t in Physics.OverlapBox(target, new Vector3(0.1f, 0.1f, 0.1f)))
	    {
		    CheckerPiece overlap = t.GetComponent<CheckerPiece>();
		    if (overlap == null) continue;
		    res[0] = true;
			res[1] = overlap.team != team;
	    }
	    return res;
    }

    
    
    private void MoveSingle(Vector3 target)
    {
	    Debug.Log("Move 1");
	    _transform.position = target;
    }

    private void MoveDouble(Vector3 target)
    {
	    Debug.Log("Move 2");
	    _transform.position = target;
    }

    private void King()
    {
	    
    }
}
