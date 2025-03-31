using ChessMonsterTactics.Gameplay;
using UnityEngine;

namespace ChessMonsterTactics
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "Settings/UI Settings")]
    public class UISettings : ScriptableSettings<UISettings>
    {
        [SerializeField] private Color _playerHealthBarColor;
        [SerializeField] private Color _oppoentHealthBarColor;
        [SerializeField] private Color _moveHighlightNormalColor;
        [SerializeField] private Color _moveHighlightAttackColor;
        [Space]
        [SerializeField] private Color _abilitySlotActiveColor;
        [SerializeField] private Color _abilitySlotNormalColor;
        [SerializeField] private float _abilitySlotTransition = 0.55f;
        [Space]
        [SerializeField] private DamagePopupSettings _damagePopupSettings;
        [SerializeField] private TeamConfigUISettings _teamConfig;
        [SerializeField] private InventoryUISettings _inventory;

        public Color PlayerHealthBarColor { get => _playerHealthBarColor; }
        public Color OpponentHealthBarColor { get => _oppoentHealthBarColor; }
        public Color MoveHighlightNormalColor { get => _moveHighlightNormalColor; }
        public Color MoveHighlightAttackColor { get => _moveHighlightAttackColor; }

        public Color AbilitySlotActiveColor { get => _abilitySlotActiveColor; }
        public Color AbilitySlotNormalColor { get => _abilitySlotNormalColor; }
        public float AbilitySlotTransition { get => _abilitySlotTransition; }

        public DamagePopupSettings DamagePopup { get => _damagePopupSettings; }
        public TeamConfigUISettings TeamConfig { get => _teamConfig; }
        public InventoryUISettings Inventory { get => _inventory; }

        protected override string GetAssetPath()
        {
            return "UISettings";
        }

        [System.Serializable]
        public class TeamConfigUISettings
        {
            [SerializeField] private TeamConfigSlot _slotPrefab;
            [Space]
            [SerializeField] private Sprite _pawnIcon;
            [SerializeField] private Sprite _rookIcon;
            [SerializeField] private Sprite _knightIcon;
            [SerializeField] private Sprite _bishopIcon;
            [SerializeField] private Sprite _queenIcon;
            [SerializeField] private Sprite _kingIcon;

            public TeamConfigSlot SlotPrefab { get => _slotPrefab; }
            public Sprite PawnIcon { get => _pawnIcon; }
            public Sprite RookIcon { get => _rookIcon; }
            public Sprite KnightIcon { get => _knightIcon; }
            public Sprite BishopIcon { get => _bishopIcon; }
            public Sprite QueenIcon { get => _queenIcon; }
            public Sprite KingIcon { get => _kingIcon; }
        }

        [System.Serializable]
        public class InventoryUISettings
        {
            [SerializeField] private InventorySlot _slotPrefab;

            public InventorySlot SlotPrefab { get => _slotPrefab; }
        }

        [System.Serializable]
        public class DamagePopupSettings
        {
            [SerializeField] private DamagePopup _prefab;
            [SerializeField] private float _transition;

            public DamagePopup Prefab { get => _prefab; }
            public float Transition { get => _transition; }
        }
    }
}
