using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class NetworkSceneSwitcher : MonoBehaviour
    {
        public static NetworkSceneSwitcher Singleton { get; internal set; }

        [HideInInspector] public delegate void SceneSwitcherDelegate(Scenes newScene);
        [HideInInspector] public event SceneSwitcherDelegate OnCurrentSceneChanged;
        [HideInInspector] public delegate void ClientLoadedSceneDelegate(ulong clientId);
        [HideInInspector] public event ClientLoadedSceneDelegate OnClientLoadedScene;

        private Scenes _currentScene;
        private int _loadedClientsAmount;
        public Scenes GetCurrentScene() => _currentScene;
        public void RegisterCallbacks() => NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
        public bool IsAllClientsLoaded() => _loadedClientsAmount == NetworkManager.Singleton.ConnectedClients.Count;
        /// <summary>
        /// Switches to newSceneName. Use nameof(Scenes.name) as new scene name.
        /// </summary>
        /// <param name="newSceneName"></param>
        public void SwitchToSceneInSingleMode(string newSceneName, Scenes newSceneIndex)
        {
            if (NetworkManager.Singleton.IsListening)
            {
                _loadedClientsAmount = 0;
                NetworkManager.Singleton.SceneManager.LoadScene(newSceneName, LoadSceneMode.Single);
                SetCurrentScene(newSceneIndex);
            }
            else
            {
                SceneManager.LoadSceneAsync(newSceneName);
                SetCurrentScene(newSceneIndex);
            }
        }
        /// <summary>
        /// Exits current scene and loads StartScene
        /// </summary>
        public void ExitToStartMenu()
        {
            if ((NetworkManager.Singleton != null) && (NetworkManager.Singleton.SceneManager != null))
                NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
            OnClientLoadedScene = null;
            SwitchToSceneInSingleMode(nameof(Scenes.StartMenu), Scenes.StartMenu);
        }
        /// <summary>
        /// If another version exists, destroys it and use current version.
        /// Set scene to InitialBootStrap
        /// </summary>
        private void Awake()
        {
            if ((Singleton != this) && (Singleton != null))
                Destroy(Singleton.gameObject);
            Singleton = this;
            SetCurrentScene(Scenes.InitialBootStrap);
            DontDestroyOnLoad(this);
        }
        /// <summary>
        /// Loads default menu at game start (should be a component of NetworkManager)
        /// </summary>
        private void Start()
        {
            if (_currentScene == Scenes.InitialBootStrap)
                SwitchToSceneInSingleMode(nameof(Scenes.StartMenu), Scenes.StartMenu);
        }
        private void SetCurrentScene(Scenes newScene)
        {
            _currentScene = newScene;
            OnCurrentSceneChanged?.Invoke(_currentScene);
        }
        /// <summary>
        /// Triggers when client connects completely
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="sceneName"></param>
        /// <param name="loadSceneMode"></param>
        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            _loadedClientsAmount++;
            OnClientLoadedScene?.Invoke(clientId);
        }
    }
}
