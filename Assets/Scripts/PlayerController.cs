using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour, IKickable
{
    private PlayerControls _playerControls;
    private InputAction _move;
    [SerializeField] private Vector2 moveInput;
    private Transform _transform;
    private Rigidbody _rb;
    public float acceleration = 15f;
    public float maxSpeed = 10f;
    public float drag = 1f;

    #region Inputs
    void MoveInputPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    void MoveInputCancelled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
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
    }

    void OnDisable()
    {
        _move.performed -= MoveInputPerformed;
        _move.canceled -= MoveInputCancelled;
        _move.Disable();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(IsOwner) RequestPlayerFixedUpdateServerRpc(moveInput);
    }

    [ServerRpc]
    void RequestPlayerFixedUpdateServerRpc(Vector2 input)
    {
        PlayerFixedUpdateClientRpc(input);
    }

    [ClientRpc]
    void PlayerFixedUpdateClientRpc(Vector2 input)
    {
        Quaternion rotation = _transform.rotation;
        MovePlayer(input);
        _rb.angularVelocity = Vector3.zero;
        _transform.rotation = rotation;
    }

    void MovePlayer(Vector2 input)
    {
        Vector3 velocity = _rb.velocity;
        _rb.AddForce(-velocity*drag, ForceMode.Acceleration);
        Vector3 heading = new (input.x*acceleration, 0,input.y*acceleration);
        _rb.AddForce(heading, ForceMode.Acceleration);
        _rb.velocity = new(Mathf.Clamp(velocity.x,-maxSpeed, maxSpeed),0,
            Mathf.Clamp(velocity.z,-maxSpeed, maxSpeed));
    }

    public void GetKicked()
    {
        
    }
}
