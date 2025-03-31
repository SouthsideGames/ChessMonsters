using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Weakened - Temporarily reduces target’s attack
    public class WeakenedEffect : StatusEffect
    {
        private int _attackPenalty;

        public WeakenedEffect(int penalty, int duration)
        {
            Name = "Weakened";
            _attackPenalty = penalty;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            target.ApplyAttackModifier(-_attackPenalty);
            Debug.Log($"{target.Name} is weakened (-{_attackPenalty} Attack).");
        }

        public override void OnExpire(Monster target)
        {
            target.ApplyAttackModifier(_attackPenalty);
            Debug.Log($"{target.Name}'s weakened effect ended.");
        }

        public override void OnTurnPassed(Monster target) { }
    }
}
