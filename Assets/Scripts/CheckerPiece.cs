using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class CheckerPiece : NetworkBehaviour, IKickable
{
	///for testing
	// public float testAngle;

	public enum TeamColour
	{
		White=1,
		Black=2
	}

	public int rank;
	public int file;
	private float _tileLength;
	private float _tileWidth;
	private int _numberOfRanks;
	private int _numberOfFiles;

	public float jumpHeight = 1f;
	public TeamColour team;
	private bool _isKing;

	private bool _dieOnLanding = false;

	public bool IsKing
	{
		get => _isKing;
		set
		{
			if (!IsServer) return;
			ChangeKingStateClientRpc(value);
			_isKing = value;
		}
	}

	private bool _isMoving;
	private List<int[]> _possibleConquers;

	private GameManager _gm;
	private CheckerPieceView _view;
	private Transform _transform;

	private void Start()
	{
		_transform = transform;
		_view = GetComponentInChildren<CheckerPieceView>();
		_gm = GameManager.Singleton;
		_tileLength = _gm.tileLength;
		_tileWidth = _gm.tileWidth;
		_numberOfRanks = _gm.numberOfRanks;
		_numberOfFiles = _gm.numberOfFiles;
		int index = rank + file * _numberOfRanks;
		_gm.OccupancyArray[index] = (int)team;
		_gm.checkerPieces[index] = this;
		_possibleConquers = new();
	}
	
	[ClientRpc]
	private void GetTakenClientRpc()
	{
		if (_isMoving)
		{
			_dieOnLanding = true;
			return;
		}
		Die();
	}

	private void Die()
	{
		int index = rank + file * _numberOfRanks;
		_gm.OccupancyArray[index] = 0;
		_gm.checkerPieces[index] = null;
		gameObject.SetActive(false);
	}
	
	public void GetKicked(float angle)
	{
		if (_isMoving) return;
		int[] rankFileMovement = GetRankFileMovement(angle);
		if (!IsKing && IsMovingBackwards(rankFileMovement)) return;
		Vector3 target = GetTarget(rankFileMovement, _transform.position);
		if (IsOutOfBounds(rankFileMovement)) return;
		int targetOccupation = CheckOccupationStatus(rankFileMovement);
		if (targetOccupation == 0)
		{
			UpdateRankFile(rankFileMovement);
			StartCoroutine(MoveInArc(target, null));
		}
		else if (targetOccupation != (int)team)
		{
			rankFileMovement[0] += rankFileMovement[0];
			rankFileMovement[1] += rankFileMovement[1];
			target = GetTarget(rankFileMovement, _transform.position);
			if (IsOutOfBounds(rankFileMovement)) return;
			targetOccupation = CheckOccupationStatus(rankFileMovement);
			if (targetOccupation == 0)
			{
				UpdateRankFile(rankFileMovement);
				StartCoroutine(MoveInArc(target, _gm.checkerPieces[rank-rankFileMovement[0]/2+(file-rankFileMovement[1]/2)*_numberOfRanks]));
			}
		}
	}
	
	private void UpdateRankFile(int[] rankFileMovement)
	{
		int index = rank + file * _numberOfRanks;
		_gm.OccupancyArray[index] = 0;
		_gm.checkerPieces[index] = null;
		rank += rankFileMovement[0];
		file += rankFileMovement[1];
		index = rank + file * _numberOfRanks;
		_gm.OccupancyArray[index] = (int)team;
		_gm.checkerPieces[index] = this;
	}
	
	private bool CheckAvailableConquers()
	{
		for (int i = 0; i < 4; i++)
		{
			if (i>1 && !IsKing) break;
			float angle = -45+i*90;
			if (team == TeamColour.Black) angle += 180;
			int[] rankFileMovement = GetRankFileMovement(angle);
			int occupationStatus = CheckOccupationStatus(rankFileMovement);
			if(occupationStatus == 0 || occupationStatus == (int)team) continue;
			rankFileMovement[0] += rankFileMovement[0];
			rankFileMovement[1] += rankFileMovement[1];
			if (IsOutOfBounds(rankFileMovement)) continue;
			if (CheckOccupationStatus(rankFileMovement) != 0) continue;
			_possibleConquers.Add(rankFileMovement);
		}
		
		if (_possibleConquers.Count == 0) return false;
		return true;
	}

	private int[] GetRankFileMovement(float angle)
	{
		float angleDeg = math.radians(angle);
		return new[] {(int)math.sign(math.sin(angleDeg)),(int)math.sign(math.cos(angleDeg))};
	}
	
		private Vector3 GetTarget(int[] rankFileMovement, Vector3 position)
    {
	    float xDiff = _tileWidth * rankFileMovement[0];
	    float zDiff = _tileLength * rankFileMovement[1];
	    
	    return new Vector3(position.x + xDiff, position.y, position.z + zDiff);
    }

	private bool IsMovingBackwards(int[] rankFileMovement)
	{
		if (team == TeamColour.White) return rankFileMovement[1] < 0;
		return rankFileMovement[1] > 0;
	}

    private bool IsOutOfBounds(int[] rankFileMovement)
    {
	    if (rank+rankFileMovement[0] > _numberOfRanks-1) return true;
	    if (rank+rankFileMovement[0] < 0) return true;
	    if (file+rankFileMovement[1] > _numberOfFiles-1) return true;
	    return file+rankFileMovement[1] < 0;
    }

    private int CheckOccupationStatus(int[] rankFileMovement)
    {
	    return _gm.OccupancyArray[rank+rankFileMovement[0]+(file+rankFileMovement[1])*_numberOfRanks];
    }

    private IEnumerator MoveInArc(Vector3 target, CheckerPiece opponent)
    {
	    _isMoving = true;
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
	    if (IsServer) StartRippleEffectClientRpc(target);
	    _isMoving = false;
	    if (_dieOnLanding)
	    {
		    _dieOnLanding = false;
		    Die();
		    GameManager.Singleton.CheckAvailableMoves();
	    }
	    if (IsEndOfBoard(target)) IsKing = true;
	    if (opponent == null)
	    {
		    GameManager.Singleton.CheckAvailableMoves();
		    yield break;
	    }
	    if (CheckAvailableConquers())
	    {
		    int[] rankFileMovement = _possibleConquers[Random.Range(0, _possibleConquers.Count)];
		    position = _transform.position;
		    Vector3 moveTarget = GetTarget(rankFileMovement, position);
		    UpdateRankFile(rankFileMovement);
		    StartCoroutine(MoveInArc(moveTarget,
			    _gm.checkerPieces[rank - rankFileMovement[0] / 2 + (file - rankFileMovement[1] / 2) * _numberOfRanks]));
	    }
	    else
	    {
		    GameManager.Singleton.CheckAvailableMoves();
	    }
    }
    
    private bool IsEndOfBoard(Vector3 target) => target.z + _tileLength > _tileLength*_numberOfFiles/2f
                                                 || target.z - _tileLength < -_tileLength*_numberOfFiles/2f;

    [ClientRpc]
    private void StartRippleEffectClientRpc(Vector3 position)
    {
	    if (IsClient) _view.RippleEffect(position);
    }
    
    [ClientRpc]
    private void ChangeKingStateClientRpc(bool state)
    {
	    if (IsClient) _view.ChangeKingState(state);
    }

    [ClientRpc]
    private void MovePieceClientRpc(Vector3 target) => _transform.position = target;
}
