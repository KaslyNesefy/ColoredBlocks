using Unity.Netcode;
using UnityEngine;
public interface IPickableItem
{
    public void PickItemRpc(NetworkObjectReference newParent, Vector3 newPosition, Quaternion newRotation, Vector3 newScale);
    public bool IsPicked();
}
