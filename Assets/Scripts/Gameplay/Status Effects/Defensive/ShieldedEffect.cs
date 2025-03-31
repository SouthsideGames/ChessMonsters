using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Shielded - Reduces all damage taken by a fixed amount
    public class ShieldedEffect : StatusEffect
    {
        private int _reduction;

        public ShieldedEffect(int amount, int duration)
        {
            Name = "Shielded";
            _reduction = amount;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is shielded (-{_reduction} damage taken).");
        }

        public override void OnTurnPassed(Monster target) { }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name}'s shield has faded.");
        }

        public int ReduceDamage(int incoming)
        {
            return Mathf.Max(0, incoming - _reduction);
        }
    }
}
