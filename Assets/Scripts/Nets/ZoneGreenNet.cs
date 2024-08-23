using UnityEngine;

public class ZoneGreenNet : Net
{
    [SerializeField] private GameObject zoneRedNet;
    [SerializeField] private float offsetHorizontal = 0;
    [SerializeField] private float offsetVertical = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer && !IsHost)
            return;
        Invoke(nameof(BuildGreenNet), 0.2f);
    }
    private void BuildGreenNet()
    {
        BuildNetCopy(transform, zoneRedNet.GetComponent<ZoneRedNet>().GetOriginPieces(), offsetHorizontal, offsetVertical);
    }
}
