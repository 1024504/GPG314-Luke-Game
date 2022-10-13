using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckerPiece : NetworkBehaviour, IKickable
{
	///for testing
	public float testAngle;

	public enum TeamColour
	{
		White,
		Black
	}

	public float jumpHeight = 1f;
	public TeamColour team;
	private bool isKing;

	private bool IsKing
	{
		get => isKing;
		set
		{
			ChangeKingStateClientRpc(value);
			isKing = value;
		}
	}

	private bool isMoving;
	
	[SerializeField] private GameObject KingView;
	private Transform _transform;
	private OccupationStatus _targetOccupation;

	private void Start()
	{
		_transform = transform;
	}

	[ClientRpc]
	public void GetTakenClientRpc()
	{
		gameObject.SetActive(false);
	}

	public void GetKicked(float angle)
	{
		if (isMoving) return;
		if (!IsKing && IsMovingBackwards(angle)) return;
		Vector3 target = GetTarget(angle, _transform.position);
		if (IsOutOfBounds(target)) return;
		_targetOccupation = CheckOccupationStatus(target);
		if (!_targetOccupation.IsOccupied)
		{
			StartCoroutine(MoveInArc(target, null));
		}
		else if (_targetOccupation.IsOpponent)
		{
			target = GetTarget(angle, target);
			if (IsOutOfBounds(target)) return;
			CheckerPiece opponentPiece = _targetOccupation.Opponent;
			_targetOccupation = CheckOccupationStatus(target);
			if (!_targetOccupation.IsOccupied) StartCoroutine(MoveInArc(target, opponentPiece));
		}
	}
	
	private void CheckContinuingConquer()
	{
		List<float> possibleAngles = new();
		float angle;
		Vector3 enemyTarget;
		Vector3 moveTarget;
		
		for (int i = 0; i < 4; i++)
		{
			if (i>1 && !IsKing) break;
			angle = -45+i*90;
			if (team == TeamColour.Black) angle += 180;
			enemyTarget = GetTarget(angle, _transform.position);
			moveTarget = GetTarget(angle, enemyTarget);
			if (IsOutOfBounds(moveTarget)) continue;
			if (CheckOccupationStatus(moveTarget).IsOccupied) continue;
			OccupationStatus targetOccupation = CheckOccupationStatus(enemyTarget);
			if (targetOccupation.IsOccupied && targetOccupation.IsOpponent) possibleAngles.Add(angle);
		}

		if (possibleAngles.Count == 0) return;
		angle = possibleAngles[Random.Range(0, possibleAngles.Count)];
		enemyTarget = GetTarget(angle, _transform.position);
		moveTarget = GetTarget(angle, enemyTarget);
		StartCoroutine(MoveInArc(moveTarget,CheckOccupationStatus(enemyTarget).Opponent));
	}

	private Vector3 GetTarget(float angle, Vector3 position)
    {
	    float xDiff = 10 * Mathf.Sign(Mathf.Sin(Mathf.Deg2Rad*angle));
	    float zDiff = 10 * Mathf.Sign(Mathf.Cos(Mathf.Deg2Rad*angle));
	    
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
    
    private bool IsEndOfBoard(Vector3 target) => target.z + 10 > 40 || target.z - 10 < -40;

	    private OccupationStatus CheckOccupationStatus(Vector3 target)
    {
	    OccupationStatus res = new OccupationStatus();

	    foreach (Collider t in Physics.OverlapBox(target, new Vector3(0.1f, 0.1f, 0.1f)))
	    {
		    CheckerPiece overlap = t.GetComponent<CheckerPiece>();
		    if (overlap == null) continue;
		    res.IsOccupied = true;
			res.IsOpponent = overlap.team != team;
			res.Opponent = overlap;
	    }
	    return res;
    }

    private IEnumerator MoveInArc(Vector3 target, CheckerPiece opponent)
    {
	    isMoving = true;
	    Vector3 position = _transform.position;
	    Vector3 lateralStep = (target - position) / 100f;
	    float lateralDistance = Vector3.Distance(position, target);
	    float lateralStepDistance = lateralDistance/100f;
	    
	    for (int i=0; i<100; i++)
	    {
		    float height = -jumpHeight*4/Mathf.Pow(lateralDistance,2)*i*lateralStepDistance*(i*lateralStepDistance-lateralDistance);
		    MovePieceClientRpc(new Vector3(position.x+i*lateralStep.x,position.y+height,position.z+i*lateralStep.z));
		    if (i == 49 && opponent != null) opponent.GetTakenClientRpc();
		    yield return new WaitForSeconds(0.005f);
	    }

	    MovePieceClientRpc(target);
	    isMoving = false;
	    if (IsEndOfBoard(target)) IsKing = true;
	    if (opponent != null) CheckContinuingConquer();
    }

    [ClientRpc]
    private void ChangeKingStateClientRpc(bool state)
    {
	    KingView.SetActive(state);
    }

    [ClientRpc]
    private void MovePieceClientRpc(Vector3 target) => _transform.position = target;

    private struct OccupationStatus
    {
	    public bool IsOccupied;
	    public bool IsOpponent;
	    public CheckerPiece Opponent;
    }
}
