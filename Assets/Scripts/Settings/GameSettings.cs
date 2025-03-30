using UnityEngine;

namespace ChessMonsterTactics
{
    [CreateAssetMenu(menuName = "Settings/Game Settings")]
    public class GameSettings : ScriptableSettings<GameSettings>
    {
        [SerializeField] private float _turnDuration = 10.0f;
        [SerializeField] private float _turnTransition = 2.0f;
        [Space]
        [SerializeField] private GameplaySettings _gameplay;

        public float TurnDuration { get => _turnDuration; }
        public float TurnTransition { get => _turnTransition; }
        public GameplaySettings Gameplay { get => _gameplay; }

        protected override string GetAssetPath()
        {
            return "GameSettings";
        }

        [System.Serializable]
        public class GameplaySettings
        {
            [SerializeField] private GameObject _monsterPrefab;
            [SerializeField] private GameObject _moveHighlightPrefab;

            public GameObject MonsterPrefab { get => _monsterPrefab; }
            public GameObject HighlightPrefab { get => _moveHighlightPrefab;}
        }
    }
}
