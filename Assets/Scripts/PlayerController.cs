using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private NetworkManager _networkManager;
    
    #region Inputs
    private void MoveInputPerformed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
        moveInput = context.ReadValue<Vector2>();
    }
    
    private void MoveInputCancelled(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
        moveInput = Vector2.zero;
    }

    private void KickPerformed(InputAction.CallbackContext context)
    {
	    if (!IsLocalPlayer) return;
	    RequestKickServerRpc();
    }
    #endregion

    private void OnEnable()
    {
	    _networkManager = NetworkManager.Singleton;
	    _networkManager.OnClientConnectedCallback += MoveOnSpawn;
        if (IsServer) _networkManager.SceneManager.OnLoad += ActivatePlayer;
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

    private void OnDisable()
    {
	    _move.performed -= MoveInputPerformed;
        _move.canceled -= MoveInputCancelled;
        _move.Disable();
        _kick.performed -= KickPerformed;
        _kick.Disable();
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
	    if (IsLocalPlayer)
	    {
		    MovePlayer(moveInput); //Clientside prediction
		    RequestPlayerFixedUpdateServerRpc(moveInput);
	    }
    }

    void MoveOnSpawn(ulong clientId)
    {
	    if (!IsServer) return;
	    if (OwnerClientId != clientId) return;
	    Vector3 displacement = new Vector3(clientId*4f,0f,0f);
	    MoveOnSpawnClientRpc(displacement);
    }

    void ActivatePlayer(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
    {
        ActivatePlayerClientRpc();
    }
    
    [ClientRpc]
    void ActivatePlayerClientRpc()
    {
        gameObject.SetActive(true);
    }

    [ClientRpc]
    void MoveOnSpawnClientRpc(Vector3 displacement)
    {
	    _transform.position += displacement;
    }

    [ServerRpc]
    private void RequestPlayerFixedUpdateServerRpc(Vector2 input)
    {
        MovePlayer(input);
    }

    private void MovePlayer(Vector2 input)
    {
        Vector3 velocity = _rb.velocity;
        Vector3 dragVector3 = new Vector3(-velocity.x*drag, 0, -velocity.z*drag);
        _rb.AddForce(dragVector3, ForceMode.Acceleration);
        Vector3 heading = new (input.x*acceleration, 0,input.y*acceleration);
        _rb.AddForce(heading, ForceMode.Acceleration);
        _rb.velocity = new(Mathf.Clamp(velocity.x,-maxSpeed, maxSpeed),velocity.y,
            Mathf.Clamp(velocity.z,-maxSpeed, maxSpeed));
        _rb.AddTorque(0f,turnSpeed*Vector3.SignedAngle(_transform.forward,heading,Vector3.up),0f, ForceMode.Acceleration);
        _rb.angularVelocity = Vector3.zero;
    }

    [ServerRpc]
    private void RequestKickServerRpc()
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
