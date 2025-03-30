using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ChessMonsterTactics.UI
{
    public class MonsterCatalog : MonoBehaviour
    {
        [SerializeField] private GameObject _catalogPanel;
        [SerializeField] private Transform _monsterGrid;
        [SerializeField] private GameObject _monsterEntryPrefab;
        [SerializeField] private MonsterDetail _monsterDetailPanel;

        private List<MonsterData> _monsters = new List<MonsterData>();

        public void Initialize()
        {
            // Load all monster data from resources
            _monsters.AddRange(Resources.LoadAll<MonsterData>("Monsters"));
            PopulateCatalog();
        }

        private void PopulateCatalog()
        {
            foreach (var monster in _monsters)
            {
                GameObject entryObj = Instantiate(_monsterEntryPrefab, _monsterGrid);
                MonsterCatalogEntry entry = entryObj.GetComponent<MonsterCatalogEntry>();
                entry.Initialize(monster);

                Button button = entryObj.GetComponent<Button>();
                button.onClick.AddListener(() => ShowMonsterDetails(monster));
            }
        }

        private void ShowMonsterDetails(MonsterData monster)
        {
            _monsterDetailPanel.Show();
            _monsterDetailPanel.DisplayMonster(monster);
        }

        public void Show()
        {
            _catalogPanel.SetActive(true);
        }

        public void Hide()
        {
            _catalogPanel.SetActive(false);
            _monsterDetailPanel.Hide();
        }
    }
}