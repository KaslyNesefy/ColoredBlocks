using Unity.Netcode;
using UnityEngine;

public class Piece : NetworkBehaviour, IPickableItem, IPlaceableItem
{
    [Rpc(SendTo.Server)]
    public void PickItemRpc(NetworkObjectReference newParent, Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    {
        if (!newParent.TryGet(out NetworkObject parentNetworkObject))
            return;

        if (!GetComponent<NetworkObject>().TrySetParent(parentNetworkObject))
            return;

        gameObject.layer = (int)Layers.Hands;

        SetTransformOnServerRpc(newPosition, newRotation, newScale);
        SetTransformOnClientsRpc(newPosition, newRotation, newScale);
        //transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //transform.SetLocalPositionAndRotation(newPosition, newRotation);
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void SetTransformOnClientsRpc(Vector3 newPosition, Quaternion newRotation, Vector3 newScale) => SetTransform(newPosition, newRotation, newScale);
    [Rpc(SendTo.Server)]
    private void SetTransformOnServerRpc(Vector3 newPosition, Quaternion newRotation, Vector3 newScale) => SetTransform(newPosition, newRotation, newScale);
    private void SetTransform(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
    { 
        transform.localScale = newScale;
        transform.SetLocalPositionAndRotation(newPosition, newRotation);
    }
    [Rpc(SendTo.Server)]
    public void PlaceItemRpc(NetworkObjectReference newPlace)
    {
        if (!newPlace.TryGet(out NetworkObject placeNetworkObject))
            return;

        if (!gameObject.GetComponent<NetworkObject>().TrySetParent(placeNetworkObject))
            return;

        gameObject.layer = (int)Layers.Default;

        SetTransformOnServerRpc(Vector3.zero, Quaternion.identity, Vector3.one);
        SetTransformOnClientsRpc(Vector3.zero, Quaternion.identity, Vector3.one);
    }

    private void OnTransformParentChanged() => GetComponent<MeshCollider>().enabled = !IsPicked();
    public bool IsPicked() => transform.parent.TryGetComponent<Player>(out _);
}
