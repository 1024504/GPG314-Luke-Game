using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickTrigger : MonoBehaviour
{
	[SerializeField]
	private Player player;

	private void OnTriggerEnter(Collider other)
	{
		IKickable target = other.GetComponentInParent<IKickable>();
		CheckerPiece checker = other.GetComponentInParent<CheckerPiece>();
		if (checker != null) if (checker.team != player.team) target = null;
		if (target != null) player.KickTargets.Add(target);
	}

	private void OnTriggerExit(Collider other)
	{
		IKickable target = other.GetComponentInParent<IKickable>();
		if (target != null) player.KickTargets.Remove(target);
	}
}
