using Unity.Netcode;
using UnityEngine;
public class Net : NetworkBehaviour
{
    private bool HasPieceMeshRenderer(GameObject piece)
    {
        if (!piece.TryGetComponent<MeshRenderer>(out _))
        {
            Debug.LogError("No MeshRenderer component on your piece!");
            return false;
        }
        return true;
    }
    private int CalculateNetColoumnsAmount(int piecesAmount)
    {
        int tryDevider = 1;
        while (piecesAmount / tryDevider > tryDevider)
        {
            tryDevider++;
        }
        return --tryDevider;
    }
    private GameObject ChoosePiece(GameObject[] pieces) => pieces[UnityEngine.Random.Range(0, pieces.Length)];
    private void InstantiatePieces(Transform parent, GameObject[] pieces, int piecesAmount)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (!HasPieceMeshRenderer(pieces[i]))
                return;
        }

        if (pieces.Length == 1)
        {
            for (int i = 0; i < piecesAmount; i++)
            {
                InstantiatePiece(parent, pieces[0]);
            }
            return;
        }

        for (int i = 0; i < piecesAmount; i++)
        {
            InstantiatePiece(parent, ChoosePiece(pieces));
        }
    }
    private void InstantiatePiece(Transform parent, GameObject piece)
    {
        GameObject tempGameObject = Instantiate(piece, parent.position, parent.rotation);
        tempGameObject.GetComponent<NetworkObject>().Spawn();
        tempGameObject.GetComponent<NetworkObject>().TrySetParent(parent);
    }
    private void PlacePiecesByOrder(Transform parent, int piecesAmount, float offsetHorizontal, float offsetVertical)
    {
        int coloumnAmount = CalculateNetColoumnsAmount(piecesAmount);
        int coloumnNumber = 0;
        int rowNumber = 0;

        for (int i = 0; i < piecesAmount; i++)
        {
            parent.GetChild(i).localPosition = new Vector3(offsetHorizontal * coloumnNumber, offsetVertical * rowNumber, 0);
            if (coloumnNumber < coloumnAmount)
            {
                coloumnNumber++;
            }
            else
            {
                coloumnNumber = 0;
                rowNumber++;
            }
        }
    }
    public void BuildNet(Transform parent, GameObject[] pieces, int piecesAmount, float offsetHorizontal, float offsetVertical)
    {
        if (!IsServer && !IsHost)
            return;

        if (piecesAmount < 1)
            Debug.LogWarning("No pieces.");
        InstantiatePieces(parent, pieces, piecesAmount);
        PlacePiecesByOrder(parent, piecesAmount, offsetHorizontal, offsetVertical);
    }
    public void BuildNetCopy(Transform parent, GameObject[] originPieces, float originOffsetHorizontal, float originOffsetVertical)
    {
        if (!IsServer && !IsHost)
            return;

        if (originPieces.Length < 1)
            Debug.LogWarning("No pieces.");
        InstantiateCopiedPieces(parent, originPieces);
        PlacePiecesByOrder(parent, originPieces.Length, originOffsetHorizontal, originOffsetVertical);
    }
    private void InstantiateCopiedPieces(Transform parent, GameObject[] originPieces)
    {
        for (int i = 0; i < originPieces.Length; i++)
        {
            if (!HasPieceMeshRenderer(originPieces[i]))
                return;
        }

        for (int i = 0; i < originPieces.Length; i++)
        {
            InstantiatePiece(parent, originPieces[i]);
        }
    }
    public bool IsEqualNetsByMaterial(GameObject[] net1, GameObject[] net2)
    {
        if (net1.Length != net2.Length)
            return false;

        for (int i = 0; i < net1.Length; i++)
        {
            if (net1[i].GetComponent<MeshRenderer>().sharedMaterial != net2[i].GetComponent<MeshRenderer>().sharedMaterial)
                return false;
        }

        return true;
    }
}
