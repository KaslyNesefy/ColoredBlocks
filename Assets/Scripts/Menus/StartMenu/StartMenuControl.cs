using Networking;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;

public class StartMenuControl : MonoBehaviour
{
    [SerializeField] private TMP_Text ipAddress;

    public void StartGame()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IpAddressCleaner(ipAddress.text), 7777);
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkSceneSwitcher.Singleton.RegisterCallbacks();
            NetworkSceneSwitcher.Singleton.SwitchToSceneInSingleMode(nameof(Scenes.Level), Scenes.Level);
            return;
        }
        Debug.LogError("Failed to start Host!");
    }

    public void JoinGame()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IpAddressCleaner(ipAddress.text), 7777);
        if (NetworkManager.Singleton.StartClient())
        {
            NetworkSceneSwitcher.Singleton.RegisterCallbacks();
        }
        else
        {
            Debug.LogError("Failed to start Client!");
        }

    }
    private string IpAddressCleaner(string stringToClean)
    {
        string tempCleanedString = Regex.Replace(stringToClean, "[^A-Za-z0-9.]", "");

        if ((tempCleanedString.ToLower() == "localhost") || (tempCleanedString == ""))
            return "127.0.0.1";
        
        string ipPattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
        tempCleanedString = Regex.Replace(tempCleanedString, $@"(?!{ipPattern})\S+", "").Replace("  ", " ").Trim();
        if (Regex.IsMatch(tempCleanedString, ipPattern))
            return tempCleanedString;
        
        Debug.LogWarning("Incorrect ip");
        return "";
    }
}
