using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    public class Monster : MonoBehaviour
    {
        public enum SortingOrder
        {
            Default, Front
        }

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private MonsterUI _ui;

        private ushort _uid; 
        private ulong _owner;
        private MonsterData _data;
        private AbilityController _abilityController;
        private BoardPosition _direction;
        private Vector2Int _cellPosition;
        private int _currentHealth;
        private bool _moved;
        private int _turnPassed;

        public ushort Id { get => _uid; }
        public ulong Owner { get => _owner; }

        public ushort ResId { get => _data.Id; }
        public string Name { get => _data.Name; }

        public int MaxHealth { get => _data.Health; }
        public int CurrentHealth { get => _currentHealth; }
        public int AttackPower { get => _data.Attack; }
        public int Defense { get => _data.Defense; }
        public int TurnPassed { get => _turnPassed; }

        public bool Moved { get => _moved; set => _moved = value; }
        public Vector2Int CellPosition { get => _cellPosition; set => _cellPosition = value; }
        public BoardPosition BoardPosition { get => _direction; }
        public MovementSolver MovementSolver { get => _data.MovementSolver; }
        public AbilityController AbilityController { get => _abilityController; }

        public void Set(ulong owner, ushort id, MonsterData monsterData, BoardPosition position)
        {
            _uid = id;
            _owner = owner;

            _data = monsterData;
            _direction = position;
            _renderer.sprite = _data.Sprite;
            // _renderer.color = monsterData.SpriteTint;

            _abilityController = new AbilityController(this, _data);

            InitializeStats();

            _ui.Initialize(this);
        }

        private void InitializeStats()
        {
            _currentHealth = MaxHealth;
        }

        public void SetSpriteSortOrder(SortingOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortingOrder.Default:
                    _renderer.sortingOrder = 0;
                    _ui.SetSortingOrder(1);
                    break;
                case SortingOrder.Front:
                    _renderer.sortingOrder = 10;
                    _ui.SetSortingOrder(11);
                    break;
            }
        }

        public void TakeDamage(int amount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - amount, 0, MaxHealth);
            _ui.UpdateUI();
        }
    
        public void OnTurnStarted()
        {
            _abilityController.OnTurnStarted();
        }

        public void OnTurnPassed()
        {
            _turnPassed++;
            _abilityController.OnTurnPassed();
        }
    
    }
}
