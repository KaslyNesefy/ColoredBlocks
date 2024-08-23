using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject hands;
    [SerializeField] private Camera cameraPlayer;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        Invoke(nameof(SetPositionToSpawnPointRpc), 0.5f);
    }
    /// <summary>
    /// Finds spawn place by tag Spawn, also sets it's tranform to Player exluding scale
    /// </summary>
    private void SetPositionToSpawnPointRpc()
    {
        GameObject spawnPlace;
        if (GameObject.FindGameObjectWithTag(nameof(Tags.Spawn)) == null)
        {
            Debug.LogWarning("No spawn place!");
            return;
        }

        spawnPlace = GameObject.FindGameObjectWithTag(nameof(Tags.Spawn));
        transform.SetPositionAndRotation(spawnPlace.transform.position, spawnPlace.transform.rotation);
    }
    /// <summary>
    /// Reads Player functional input
    /// </summary>
    private void Update()
    {
        if (!IsOwner)
            return;

        if (transform.position.y < -2)
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);

        if (Input.GetKeyDown(KeyCode.E))
        {
            ManipulatePiece();
        }
    }
    private void ManipulatePiece()
    {
        if (!Physics.Raycast(cameraPlayer.ScreenPointToRay(Input.mousePosition), out RaycastHit hittedObject))
            return;

        if (IsHandsEmpty())
        {
            if (IsHittedPiece(hittedObject) && !hittedObject.transform.parent.TryGetComponent<ZoneRedNet>(out _))
            {
                GetItemToHands(hittedObject.transform.gameObject);
                return;
            }

            return;
        }

        if (IsHittedPieceContainer(hittedObject))
        {
            PlaceItemFromHandsIntoContainer(transform.GetComponentInChildren<Piece>().gameObject, hittedObject.transform.gameObject);
            return;
        }

        if (IsHittedPiece(hittedObject) && IsParentNetPieceContainer(hittedObject))
        {
            ReplacePieceInHandsByPieceFromContainer(hittedObject.transform.gameObject, transform.GetComponentInChildren<Piece>().gameObject, hittedObject.transform.parent.gameObject);
            return;
        }
    }
    private void GetItemToHands(GameObject item) => item.GetComponent<IPickableItem>().PickItemRpc(gameObject, hands.transform.localPosition, hands.transform.localRotation, Vector3.one * 0.2f);
    private void PlaceItemFromHandsIntoContainer(GameObject pieceInHands, GameObject container) => pieceInHands.GetComponent<IPlaceableItem>().PlaceItemRpc(container);
    private bool IsHandsEmpty() => transform.GetComponentInChildren<Piece>() == null;
    private bool IsHittedPiece(RaycastHit hittedObject) => hittedObject.transform.TryGetComponent<Piece>(out _);
    private bool IsHittedPieceContainer(RaycastHit hittedObject) => hittedObject.transform.TryGetComponent<PieceContainer>(out _);
    private bool IsParentNetPieceContainer(RaycastHit hittedObject) => hittedObject.transform.parent.TryGetComponent<PieceContainer>(out _);
    private void ReplacePieceInHandsByPieceFromContainer(GameObject netPieceInContainer, GameObject netPieceInHands, GameObject container)
    {
        GetItemToHands(netPieceInContainer);
        PlaceItemFromHandsIntoContainer(netPieceInHands, container);
    }
}
