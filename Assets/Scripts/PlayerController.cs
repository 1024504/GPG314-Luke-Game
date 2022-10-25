using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
	public IControllable Controlled;
    [SerializeField] private GameObject player;
    private Transform _playerTransform;

    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _aim;
    private InputAction _interact;
    private InputAction _action1;
    private InputAction _action2;

    private Vector2 _moveInput;
    private Vector2 _moveDirection;
    private Vector2 _aimInput;
    private Vector2 _aimDirection;
    
    private void OnEnable()
    {
	    _playerTransform = player.transform;

	    _playerControls = new PlayerControls();
	    _move = _playerControls.Player.Move;
	    _aim = _playerControls.Player.Aim;
	    _interact = _playerControls.Player.Interact;
	    _action1 = _playerControls.Player.Action1;
	    _action2 = _playerControls.Player.Action2;

	    _move.Enable();
	    _move.performed += MovePerformed;
	    _move.canceled += MoveCancelled;
	    
	    _aim.Enable();
	    _aim.performed += AimPerformed;
	    _aim.canceled += AimCancelled;

	    _interact.Enable();
	    _interact.performed += InteractPerformed;
	    
	    _action1.Enable();
	    _action1.performed += Action1Performed;
	    
	    _action2.Enable();
	    _action2.performed += Action2Performed;
    }

    private void OnDisable()
    {
	    _move.performed -= MovePerformed;
	    _move.canceled -= MoveCancelled;
	    _move.Disable();

	    _aim.performed -= AimPerformed;
	    _aim.canceled -= AimCancelled;
	    _aim.Disable();
	    
	    _interact.performed -= InteractPerformed;
	    _interact.Disable();
	    
	    _action1.performed -= Action1Performed;
	    _action1.Disable();
	    
	    _action2.performed -= Action2Performed;
	    _action2.Disable();
    }

    private void FixedUpdate()
    {
	    player.GetComponent<IControllable>()?.Move(_moveDirection);
	    player.GetComponent<IControllable>()?.Aim(_aimDirection);
    }
    
    private void MovePerformed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    _moveInput = context.ReadValue<Vector2>();
    }
    
    private void MoveCancelled(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    _moveInput = Vector2.zero;
    }
    
    private void AimPerformed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    _aimInput = context.ReadValue<Vector2>();
	    if (context.control.parent.name == "Mouse")
	    {
		    if (Camera.main == null) return;
		    Vector3 playerPos = Camera.main.WorldToScreenPoint(_playerTransform.position);
		     _aimDirection = _aimInput-new Vector2(playerPos.x, playerPos.y);
		    return;
	    }

	    _aimDirection = _aimInput;
    }
    
    private void AimCancelled(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    _aimInput = Vector2.zero;
    }
    
    private void InteractPerformed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    player.GetComponent<IControllable>()?.Interact();
    }
    
    private void Action1Performed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    player.GetComponent<IControllable>()?.Action1();
    }
    
    private void Action2Performed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    player.GetComponent<IControllable>()?.Action2();
    }
}
