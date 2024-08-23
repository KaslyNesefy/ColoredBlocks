using Unity.Netcode;

public interface IPlaceableItem
{
    public void PlaceItemRpc(NetworkObjectReference place);
}
