using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPieceView : MonoBehaviour
{
	public GameObject rippleEffectPrefab;

	[SerializeField] private GameObject kingView;

	public void ChangeKingState(bool state)
	{
		kingView.SetActive(state);
	}

	public void RippleEffect(Vector3 position)
	{
		Instantiate(rippleEffectPrefab, position, Quaternion.identity);
	}
}
