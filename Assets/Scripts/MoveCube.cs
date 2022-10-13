using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveCube : NetworkBehaviour
{
	public NetworkVariable<Vector3> position;
	
	// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = position.Value;
    }
}
