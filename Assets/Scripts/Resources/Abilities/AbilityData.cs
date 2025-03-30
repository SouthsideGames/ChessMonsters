using UnityEngine;
using ChessMonsterTactics.Gameplay;

namespace ChessMonsterTactics
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Monsters/Ability")]
    public class AbilityData : GameResource
    {
        // Cooldown will be measured in turns
        [SerializeField] private AbilityType _abilityType;
        [SerializeField] private int _cooldown;
        [SerializeField] private int _energyCost;
        [Header("Gameplay")]
        [SerializeField] private AbilityTrigger _abilityTrigger;
        [SerializeReference, SubclassSelector] private AbilityBehaviour _behaviour;

        public AbilityType AbilityType { get => _abilityType; }
        public int Cooldown { get => _cooldown; }
        public int EnergyCost { get => _energyCost; }
        public AbilityTrigger AbilityTrigger { get => _abilityTrigger; }
        public AbilityBehaviour Behaviour { get => _behaviour; }
    }
}
