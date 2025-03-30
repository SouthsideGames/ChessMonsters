
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ChessMonsterTactics.UI
{
    public class MonsterCatalogEntry : MonoBehaviour
    {
        [SerializeField] private Image _monsterImage;
        [SerializeField] private TMP_Text _monsterName;

        public void Initialize(MonsterData data)
        {
            _monsterImage.sprite = data.Sprite;
            _monsterImage.color = data.SpriteTint;
            _monsterName.text = data.Name;
        }
    }
}
