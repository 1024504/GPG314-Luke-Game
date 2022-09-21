using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    [SerializeField] private Transform whiteParent;
    [SerializeField] private Transform blackParent;

    void Start()
    {
        // if (IsServer)
        // {
        //     SpawnPieces();
        // }
    }

    void Update()
    {
        if (!IsServer) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPieces();
        }
    }

    void SpawnPieces()
    {
        for (int i = 0; i < 12; i++)
        {
            Vector3 positionWhite;
            Vector3 positionBlack;
            if (i > 3 && i < 8)
            {
                positionWhite = new Vector3(-25f+20f*(i%4), 1.05f, -35f+10f*((i-i%4)/4));
                positionBlack = new Vector3(-35f+20f*(i%4), 1.05f, 35f-10f*((i-i%4)/4));
            }
            else
            {
                positionWhite = new Vector3(-35f+20f*(i%4), 1.05f, -35f+10f*((i-i%4)/4));
                positionBlack = new Vector3(-25f+20f*(i%4), 1.05f, 35f-10f*((i-i%4)/4));
            }
            SpawnWhitePieceClientRpc(positionWhite);
            SpawnBlackPieceClientRpc(positionBlack);
        }
    }

    [ClientRpc]
    private void SpawnWhitePieceClientRpc(Vector3 position)
    {
        Instantiate(whitePiecePrefab, position, Quaternion.identity, whiteParent);
    }
    
    [ClientRpc]
    private void SpawnBlackPieceClientRpc(Vector3 position)
    {
        Instantiate(blackPiecePrefab, position, Quaternion.identity, blackParent);
    }
}
