using UnityEngine;
using ChessMonsterTactics.Gameplay;

namespace ChessMonsterTactics
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Monsters/Monster")]
    public class MonsterData : GameResource
    {
        [Header("Settings")]
        [SerializeField] private MonsterType _type;
        [SerializeField] private int _health = 100;
        [SerializeField] private int _defense = 25;
        [SerializeField] private int _attack = 40;
        [Header("Graphics")]
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Color _spriteTint;
        [Space]
        // Gameplay
        [SerializeReference, SubclassSelector] private MovementSolver _movementSolver; 
        [Space]
        [Header("Abilities")]
        // TODO: Use array instead
        [SerializeField] private AbilityData _passive;
        [SerializeField] private AbilityData _active;
        [SerializeField] private AbilityData _ultimate;

        public MonsterType Type { get => _type; } 
        public int Health { get => _health; }
        public int Defense { get => _defense; }
        public int Attack { get => _attack; }
        public Sprite Sprite { get => _sprite; }
        public Color SpriteTint { get => _spriteTint; }
        public MovementSolver MovementSolver { get => _movementSolver; }

        public AbilityData Passive { get => _passive; }
        public AbilityData Active { get => _active; }
        public AbilityData Ultimate { get => _ultimate; }
    }
}
