using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

namespace ChessMonsterTactics
{
    public class HomeUI : UIPanel
    {
        [SerializeField] private TMP_Text _loadedProfile;
        [SerializeField] private GameObject _teamNotice;
        [Space]
        [SerializeField] private Button _startAsServer;
        [SerializeField] private Button _findMatchButton;
        [SerializeField] private Button _teamConfigButton;
        [SerializeField] private TeamConfigUI _teamConfig;

        private void Start()
        {
            _loadedProfile.text = "Current Profile: " + Game.CurrentProfile.UserName;
            _startAsServer.onClick.AddListener(OnStartAsServerClicked);
            _findMatchButton.onClick.AddListener(OnFindMathClicked);
            _teamConfigButton.onClick.AddListener(OnTeamConfigClicked);

            Show(false);
        }

        private void OnStartAsServerClicked()
        {
            NetworkManager.Singleton.StartServer();
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        private void OnFindMathClicked()
        {
            // TODO: Initiate matchmaking
            NetworkManager.Singleton.StartClient();
        }

        private void OnTeamConfigClicked()
        {
            _teamConfig.Show();
            Hide();

            Backstack.RegisterBackstack(() => {
                _teamConfig.Hide();
                Show();
            });
        }

        public override YieldInstruction Show(bool useTransition = true)
        {
            bool teamConfigured = Game.CurrentProfile.TeamConfig.Validate();
            _teamNotice.gameObject.SetActive(!teamConfigured);
            _findMatchButton.interactable = teamConfigured;

            return base.Show(useTransition);
        }
    }
}
