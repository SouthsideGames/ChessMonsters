using UnityEngine;
using ChessMonsterTactics.UI;
using TMPro;
using DG.Tweening;

namespace ChessMonsterTactics.Gameplay
{
    // TODO: Use events dispatched by the server to notify the UI system
    internal class GameClientUIManager : MonoBehaviour
    {
        [SerializeField] private GameServerManager _server;
        [SerializeField] private GameClientManager _client;
        [Space]
        [SerializeField] private TMP_Text _playerSlot0;
        [SerializeField] private TMP_Text _playerSlot1;
        [SerializeField] private MonsterDetail _monsterDetail;
        [SerializeField] private GameObject _waitForPlayers;
        [SerializeField] private ProgressBar _timer;

        private void Start()
        {
            _client.RegisterEvent(GameEvents.EVT_INITIALIZE_PLAYERS, OnInitializePlayers);
            _client.RegisterEvent(GameEvents.EVT_TURN_BEGIN, OnBeginTurn);
            _client.RegisterEvent(GameEvents.EVT_TURN_TRANSITION, OnTurnTransition);

            _timer.SetMinMax(0, GameSettings.GetInstance().TurnDuration);
            _timer.SetValue(0);

            _monsterDetail.Hide(false);

            InitializeMonsterDetailPanel();

            if (_server.IsServer)
            {
                _waitForPlayers.GetComponentInChildren<TMP_Text>().SetText("Server");
            }
            else
            {
                _waitForPlayers.gameObject.SetActive(true);
            }
        }

        private void OnInitializePlayers()
        {
            _waitForPlayers.gameObject.SetActive(false);

            _playerSlot0.text = _server.Slots[0].Profile.UserName;
            _playerSlot1.text = _server.Slots[1].Profile.UserName;
        }

        private void OnTurnTransition()
        {                        
            if (_client.NetworkManager.LocalClientId != _server.CurrentSlot.LocalId)
                _timer.SetColor(UISettings.GetInstance().PlayerHealthBarColor);
            else
                _timer.SetColor(UISettings.GetInstance().OpponentHealthBarColor);

            _timer.Animate(GameSettings.GetInstance().TurnDuration, GameSettings.GetInstance().TurnTransition);
        }

        private void OnBeginTurn()
        {
            // _timer.AnimateNormalized(0, GameSettings.GetInstance().TurnDuration);
            _timer.Animate(0, GameSettings.GetInstance().TurnDuration);
        }
    
        private void InitializeMonsterDetailPanel()
        {
            // TODO: Detect if the user uses an ability.

            _client.OnMonsterTakeDamage += (m) => _monsterDetail.UpdateUI(m);
            _client.OnSelectPiece += (m) => _monsterDetail.Show(m);
            _client.OnDeselectPiece += () => {
                // TODO: Currently on deselect piece is also called during OnPointerDown event
                if (!_monsterDetail.Hovered)
                {
                    _monsterDetail.Hide();
                    _client.ToggleActiveAbilityRpc(false);
                }
            };

            _monsterDetail.OnActiveAbilityToggled += (active) => {
                _client.ToggleActiveAbilityRpc(active);
            };

            // _client.RegisterEvent(GameEvents.EVT_PIECE_TAKE_DAMAGE, () => {
            //     // _monsterDetail.UpdateUI(_client.GetSelectedMonster());
            // });
        }

    }
}
