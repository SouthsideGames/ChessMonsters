using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChessMonsterTactics
{
    public class DebugLoginHelper : MonoBehaviour
    {
        public static string NextSceneName { get; set; }

        [SerializeField] private TMP_InputField _profileName;
        [SerializeField] private Button _startServer;
        [SerializeField] private Button _startHost;
        [SerializeField] private Button _startClient;

        private void Start()
        {
            _startServer.onClick.AddListener(() => {
                NetworkManager.Singleton.StartServer();
                LoadNextSceneServer();
            });

            _startHost.onClick.AddListener(() => {
                NetworkManager.Singleton.StartHost();
                SceneManager.LoadScene("MainMenu");
            });

            _startClient.onClick.AddListener(() => {
                NetworkManager.Singleton.StartClient();
                SceneManager.LoadScene("MainMenu");
            });
        }

        private void LoadNextSceneServer()
        {
            Game.LoadUserProfile(_profileName.text);

            if (SceneManager.GetActiveScene().name == NextSceneName)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                NetworkManager.Singleton.SceneManager.LoadScene(NextSceneName, LoadSceneMode.Single);
            }
        }
    }
}
