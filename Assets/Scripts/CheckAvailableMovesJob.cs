using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompatible]
public struct CheckAvailableMovesJob : IJobParallelFor
{
	public int NumberOfRanks;
	public int NumberOfFiles;

	[ReadOnly]
	public NativeArray<int> OccupancyArray;
	
	[ReadOnly]
	public NativeList<int> Ranks;
	[ReadOnly]
	public NativeList<int> Files;
	[ReadOnly]
	public NativeList<CheckerPiece.TeamColour> Teams;
	[ReadOnly]
	public NativeList<bool> IsKings;

	public NativeArray<bool> HasMoves;

	public void Execute(int index)
	{
		bool hasMoves = false;
		
	    for (int i = 0; i < 4; i++)
	    {
		    if (i>1 && !IsKings[index]) break;
		    float angle = -45+i*90;
		    if (Teams[index] == CheckerPiece.TeamColour.Black) angle += 180;
		    int[] rankFileMovement = GetRankFileMovement(angle);
		    if (IsOutOfBounds(rankFileMovement, index)) continue;
		    int targetOccupation = OccupancyArray[Ranks[index]+rankFileMovement[0]+(Files[index]+rankFileMovement[1])*NumberOfRanks];;
		    if (targetOccupation == 0)
		    {
			    hasMoves = true;
			    break;
		    }
		    if (targetOccupation != (int)Teams[index])
		    {
			    rankFileMovement[0] += rankFileMovement[0];
			    rankFileMovement[1] += rankFileMovement[1];
			    if (IsOutOfBounds(rankFileMovement, index)) continue;
			    targetOccupation = OccupancyArray[Ranks[index]+rankFileMovement[0]+(Files[index]+rankFileMovement[1])*NumberOfRanks];;
			    if (targetOccupation == 0)
			    {
				    hasMoves = true;
				    break;
			    }
		    }
	    }

	    HasMoves[index] = hasMoves;
	}
	
	private int[] GetRankFileMovement(float angle)
	{
		float angleDeg = math.radians(angle);
		return new[] {(int)math.sign(math.sin(angleDeg)),(int)math.sign(math.cos(angleDeg))};
	}
	
	private bool IsOutOfBounds(int[] rankFileMovement, int index)
	{
		if (Ranks[index]+rankFileMovement[0] > NumberOfRanks-1) return true;
		if (Ranks[index]+rankFileMovement[0] < 0) return true;
		if (Files[index]+rankFileMovement[1] > NumberOfFiles-1) return true;
		return Files[index]+rankFileMovement[1] < 0;
	}
}
