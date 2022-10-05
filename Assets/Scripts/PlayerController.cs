using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour, IKickable
{
    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _kick;
    [SerializeField] private Vector2 moveInput;
    private Transform _transform;
    private Rigidbody _rb;
    public float acceleration = 15f;
    public float maxSpeed = 10f;
    public float turnSpeed = 5f;
    public float drag = 1f;
    
    public List<IKickable> kickTargets = new();

    #region Inputs
    void MoveInputPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    void MoveInputCancelled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    void KickPerformed(InputAction.CallbackContext context)
    {
	    Kick();
    }
    #endregion

    void OnEnable()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
        _playerControls = new();
        _move = _playerControls.Player.Move;
        _move.Enable();
        _move.performed += MoveInputPerformed;
        _move.canceled += MoveInputCancelled;
        _kick = _playerControls.Player.Kick;
        _kick.Enable();
        _kick.performed += KickPerformed;
    }

    void OnDisable()
    {
        _move.performed -= MoveInputPerformed;
        _move.canceled -= MoveInputCancelled;
        _move.Disable();
        _kick.performed -= KickPerformed;
        _kick.Disable();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(IsLocalPlayer) RequestPlayerFixedUpdateServerRpc(moveInput);
        Debug.Log(kickTargets.Count);
    }

    [ServerRpc]
    void RequestPlayerFixedUpdateServerRpc(Vector2 input)
    {
        PlayerFixedUpdateClientRpc(input);
    }

    [ClientRpc]
    void PlayerFixedUpdateClientRpc(Vector2 input)
    {
        MovePlayer(input);
        _rb.angularVelocity = Vector3.zero;
    }

    void MovePlayer(Vector2 input)
    {
        Vector3 velocity = _rb.velocity;
        _rb.AddForce(-velocity*drag, ForceMode.Acceleration);
        Vector3 heading = new (input.x*acceleration, 0,input.y*acceleration);
        _rb.AddForce(heading, ForceMode.Acceleration);
        _rb.velocity = new(Mathf.Clamp(velocity.x,-maxSpeed, maxSpeed),_rb.velocity.y,
            Mathf.Clamp(velocity.z,-maxSpeed, maxSpeed));
        _rb.AddTorque(0f,turnSpeed*Vector3.SignedAngle(_transform.forward,heading,Vector3.up),0f, ForceMode.Acceleration);
        
    }

    private void Kick()
    {
	    foreach (IKickable t in kickTargets)
	    {
		    t.GetKicked(Vector3.SignedAngle(Vector3.forward,
			    ((MonoBehaviour)t).GetComponent<Transform>().position-_transform.position, Vector3.up));
	    }
    }

    public void GetKicked(float angle)
    {
        
    }
}
