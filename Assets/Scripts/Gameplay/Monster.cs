using UnityEngine;
using System.Collections.Generic; //new

namespace ChessMonsterTactics.Gameplay
{
    public class Monster : MonoBehaviour
    {
        public enum SortingOrder
        {
            Default, Front
        }

        public event System.Action<Monster, int> OnDamagedByEnemy; //new
        

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
        private int _defenseModifier = 0; //new
        private int _attackModifier = 0;//new
        private int _maxHealthModifier = 0; //new
        private List<StatusEffect> _statusEffects = new List<StatusEffect>(); //new


        public ushort Id { get => _uid; }
        public ulong Owner { get => _owner; }

        public ushort ResId { get => _data.Id; }
        public string Name { get => _data.Name; }

        public int MaxHealth => _data.Health + _maxHealthModifier; //new
        public int CurrentHealth { get => _currentHealth; }
        public int AttackPower => _data.Attack + _attackModifier; //new
        public int Defense => GetModifiedDefense(); //new
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
            ResetStatModifiers();
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
            ProcessStatusEffects(); // new
        }

        //New Functions

        public void TrackDamage(int amount, Monster attacker)
        {
            TakeDamage(amount);

            // Optional: trigger event-based reactions
            OnDamagedByEnemy?.Invoke(attacker, amount);

            Debug.Log($"{Name} took {amount} damage from {attacker?.Name ?? "Unknown"} via TrackDamage.");
        }

        public void ApplyDefenseModifier(int amount)
        {
            _defenseModifier += amount;
            _ui.UpdateUI();
        }

        public int GetModifiedDefense()
        {
            return _data.Defense + _defenseModifier;
        }

        public void ApplyMaxHealthModifier(int amount)
        {
            _maxHealthModifier += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth); // keep HP within bounds
            _ui.UpdateUI();
        }

        public void ApplyAttackModifier(int amount)
        {
            _attackModifier += amount;
            _ui.UpdateUI();
        }

        public void ResetStatModifiers()
        {
            _attackModifier = 0;
            _defenseModifier = 0;
            _maxHealthModifier = 0;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, MaxHealth);
            _ui.UpdateUI();
        }

        public void AddStatusEffect(StatusEffect effect)
        {
            effect.OnApply(this);
            _statusEffects.Add(effect);
        }

        public void ProcessStatusEffects()
        {
            for (int i = _statusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _statusEffects[i];
                effect.OnTurnPassed(this);
                effect.Duration--;

                if (effect.Duration <= 0)
                {
                    effect.OnExpire(this);
                    _statusEffects.RemoveAt(i);
                }
            }
        }

        public void Heal(int amount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, MaxHealth);
            _ui.UpdateUI();
        }

        public List<StatusEffect> GetActiveStatusEffects()
        {
            return _statusEffects;
        }
    
    }
}
