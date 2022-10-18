using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RippleEffect : MonoBehaviour
{
	private Renderer _renderer;
	
	public float rippleStart = 0f;
	public float rippleEnd = 0.8f;
	public float rippleDuration = 1f;
	
	private void Awake()
	{
		_renderer = GetComponent<Renderer>();
		RippleEffectStart();
	}
	
	private void RippleEffectStart()
	{
		StartCoroutine(RippleEffectRunner());
	}

	private IEnumerator RippleEffectRunner()
	{
		int steps = 20;
		for (int i = 0; i < steps; i++)
		{
			_renderer.material.SetFloat("_TimeField", rippleStart+i*(rippleEnd-rippleStart)/steps);
			yield return new WaitForSeconds(rippleDuration/steps);
		}
		RippleEffectEnd();
	}

	private void RippleEffectEnd()
	{
		Destroy(gameObject);
	}
}
