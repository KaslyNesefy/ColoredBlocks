using System;
using Unity.Netcode;
using UnityEngine;

public class ZoneRedNet : Net
{
    [SerializeField] private GameObject[] piecesSamples;
    [SerializeField] private GameObject[] originNet;
    [SerializeField] private int piecesAmount = 0;
    [SerializeField] private float offsetHorizontal = 0;
    [SerializeField] private float offsetVertical = 0;

    public static Action OnOriginNetCreated;

    public override void OnNetworkSpawn()
    {
        if (!IsServer && !IsHost)
            return;
        BuildNet(transform, piecesSamples, piecesAmount, offsetHorizontal, offsetVertical);
        originNet = SetOriginPieces();
        OnOriginNetCreated?.Invoke();
    }
    private GameObject[] SetOriginPieces()
    {
        GameObject[] tempOriginPieces = new GameObject[piecesAmount];
        for (int i = 0; i < tempOriginPieces.Length; i++)
        {
                tempOriginPieces[i] = transform.GetChild(i).gameObject;
        }
        return tempOriginPieces;
    }
    public GameObject[] GetOriginPieces() => originNet;
}
