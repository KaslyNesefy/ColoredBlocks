using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BlueZoneCongratsText : MonoBehaviour
{
    private void Start()
    {
        ZoneBlueNet.OnEqualNets += ShowText;
        ZoneBlueNet.OnUnequalNets += HideText;
    }
    private void OnDestroy()
    {
        ZoneBlueNet.OnEqualNets -= ShowText;
        ZoneBlueNet.OnUnequalNets -= HideText;
    }
    private void ShowText() => GetComponent<TextMeshProUGUI>().enabled = true;
    private void HideText() => GetComponent<TextMeshProUGUI>().enabled = false;
}
