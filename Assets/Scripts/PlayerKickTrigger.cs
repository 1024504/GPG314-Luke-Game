using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickTrigger : MonoBehaviour
{
	[SerializeField]
	private PlayerController player;

	private void OnTriggerEnter(Collider other)
	{
		IKickable target = other.GetComponentInParent<IKickable>();
		if (target != null) player.kickTargets.Add(target);
	}

	private void OnTriggerExit(Collider other)
	{
		IKickable target = other.GetComponentInParent<IKickable>();
		if (target != null) player.kickTargets.Remove(target);
	}
}
