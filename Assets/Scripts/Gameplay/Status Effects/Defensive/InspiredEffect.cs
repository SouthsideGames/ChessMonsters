using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Inspired - Temporarily increases Attack
    public class InspiredEffect : StatusEffect
    {
        private int _bonus;

        public InspiredEffect(int amount, int duration)
        {
            Name = "Inspired";
            _bonus = amount;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            target.ApplyAttackModifier(_bonus);
            Debug.Log($"{target.Name} is inspired (+{_bonus} Attack).");
        }

        public override void OnExpire(Monster target)
        {
            target.ApplyAttackModifier(-_bonus);
            Debug.Log($"{target.Name}'s inspired effect has ended.");
        }

        public override void OnTurnPassed(Monster target) { }
    }
}
