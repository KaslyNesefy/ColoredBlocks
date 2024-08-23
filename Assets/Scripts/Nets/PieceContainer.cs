using System;
using Unity.Netcode;
using UnityEngine;

public class PieceContainer : NetworkBehaviour
{
    public static Action OnContentChange;
    private void OnTransformChildrenChanged()
    {
        GetComponent<MeshRenderer>().enabled = IsContainerEmpty();
        GetComponent<MeshCollider>().enabled = IsContainerEmpty();
        if (!IsContainerEmpty())
            transform.GetChild(0).localScale = Vector3.one;
        InvokeOnContentChangeRpc();
    }
    [Rpc(SendTo.Server)]
    private void InvokeOnContentChangeRpc() => OnContentChange?.Invoke();
    private bool IsContainerEmpty() => transform.childCount == 0;
}
