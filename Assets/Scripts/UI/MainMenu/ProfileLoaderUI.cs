using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChessMonsterTactics
{
    public class ProfileLoaderUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _profileName;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _loadButton;

        private void Start()
        {
            _createButton.onClick.AddListener(OnCreateButton);
            _loadButton.onClick.AddListener(OnLoadButton);
        }

        private void OnCreateButton()
        {
            if (string.IsNullOrEmpty(_profileName.text))
                return;

            Game.LoadUserProfile(_profileName.text, true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnLoadButton()
        {
            if (string.IsNullOrEmpty(_profileName.text))
                return;

            Game.LoadUserProfile(_profileName.text, false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
