using System;
using Unity.Netcode;
using UnityEngine;

public class ZoneBlueNet : Net
{
    public static Action OnEqualNets;
    public static Action OnUnequalNets;

    [SerializeField] private GameObject[] piecesSamples;
    [SerializeField] private GameObject zoneRedNet;
    [SerializeField] private GameObject[] blueNetPieces;
    [SerializeField] private int piecesAmount = 0;
    [SerializeField] private float offsetHorizontal = 0;
    [SerializeField] private float offsetVertical = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsHost && !IsServer)
            return;

        BuildNet(transform, piecesSamples, piecesAmount, offsetHorizontal, offsetVertical);
        blueNetPieces = SetBlueNet();

        PieceContainer.OnContentChange += OnNetChange;
    }

    private void OnNetChange()
    {
        if (!IsHost && !IsServer)
            return;

        blueNetPieces = SetBlueNet();
        if (IsEqualNetsByMaterial(blueNetPieces, zoneRedNet.GetComponent<ZoneRedNet>().GetOriginPieces()))
        {
            Debug.Log("Equale");
            InvokeOnEqualNetsRpc();
            return;
        }
        InvokeOnUnequalNetsRpc();
        Debug.Log("Non equale");
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnEqualNetsRpc() => OnEqualNets?.Invoke();
    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUnequalNetsRpc() => OnUnequalNets?.Invoke();

    private GameObject[] SetBlueNet()
    {
        GameObject[] tempBlueNet = new GameObject[transform.childCount];
        for (int i = 0; i < tempBlueNet.Length; i++)
        {
            tempBlueNet[i] = transform.GetChild(i).childCount == 0 ? transform.GetChild(i).gameObject : transform.GetChild(i).GetChild(0).gameObject;
        }
        return tempBlueNet;
    }
}
