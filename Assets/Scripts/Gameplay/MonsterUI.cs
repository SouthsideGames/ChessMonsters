using Unity.Netcode;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    public class MonsterUI : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ProgressBar _healthBar;

        private Monster _monster;

        public void Initialize(Monster monster)
        {
            ulong localId = NetworkManager.Singleton.LocalClientId;

            _monster = monster;

            _healthBar.SetMinMax(0, _monster.MaxHealth);
            _healthBar.SetColor(_monster.Owner == localId ? 
                UISettings.GetInstance().PlayerHealthBarColor : UISettings.GetInstance().OpponentHealthBarColor);

            UpdateUI();
        }

        public void UpdateUI()
        {
            _healthBar.SetValue(_monster.CurrentHealth);
        }

        public void SetSortingOrder(int sortOrder)
        {
            _canvas.sortingOrder = sortOrder;
        }
    }
}
