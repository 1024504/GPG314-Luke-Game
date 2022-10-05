using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPiece : MonoBehaviour, IKickable
{
	public enum TeamColour
	{
		White,
		Black
	}

	public TeamColour team;
	private Transform _transform;
	private Collider[] _overlaps;

	private void Start()
	{
		_transform = transform;
	}

	public void GetKicked(float angle)
    {
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
        
        
    }

    public Vector3 GetTarget(float angle)
    {
	    float xDiff = 10 * Mathf.Sign(Mathf.Sin(angle));
	    float zDiff = 10 * Mathf.Sign(Mathf.Cos(angle));

	    Vector3 position = _transform.position;
	    Vector3 target = new Vector3(position.x + xDiff, position.y, position.z + zDiff);
	    
	    return target;
    }

    public void CheckTile(Vector3 target)
    {
	    Physics.OverlapBoxNonAlloc(target, new Vector3(0.1f, 0.1f, 0.1f), _overlaps);
	    for (int i=0; i < _overlaps.Length; i++)
	    {
		    if (_overlaps[i].GetComponent<CheckerPiece>() != null)
		    {
			    break;
		    }
	    }
    }

    public void MoveTile(Vector3 target)
    {
	    
    }

    public void King()
    {
	    
    }
}
