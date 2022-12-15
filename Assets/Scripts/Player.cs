using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour, IControllable, IKickable
{
    private PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _kick;
    [SerializeField] private Vector2 moveInput;
    private Transform _transform;
    private Rigidbody _rb;
    public CheckerPiece.TeamColour team;
    public Renderer teamColourView;
    
    public float acceleration = 100f;
    public float maxSpeed = 15f;
    public float turnSpeed = 5f;
    public float drag = 7f;
    
    public List<IKickable> KickTargets = new();

    private void OnEnable()
    {
	    _transform = transform;
        _rb = GetComponent<Rigidbody>();
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
	    foreach (IKickable t in KickTargets)
	    {
		    t.GetKicked(Vector3.SignedAngle(Vector3.forward,
			    ((MonoBehaviour)t).GetComponent<Transform>().position-_transform.position, Vector3.up));
	    }
    }

    public void GetKicked(float angle)
    {
        
    }

    [ClientRpc]
    public void AssignToClientEntityClientRpc(ulong clientId)
    {
        if (IsOwner)
        {
            ClientEntity clientEntity = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ClientEntity>();
            clientEntity.ControlledPlayer = gameObject;
        }
    }

    public void Move(Vector2 direction)
    {
        MovePlayer(direction); //Clientside prediction
        RequestPlayerFixedUpdateServerRpc(direction);
    }

    public void Aim(Vector2 direction)
    {
        
    }

    public void Action1() //Kick
    {
        RequestKickServerRpc();
    }

    public void Action2()
    {
        
    }
    
    public void Action3()
    {
        
    }
}
