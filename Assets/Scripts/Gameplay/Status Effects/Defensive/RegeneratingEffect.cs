using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Regenerating - Heals X HP at the start of each turn
    public class RegeneratingEffect : StatusEffect
    {
        private int _healAmount;

        public RegeneratingEffect(int amount, int duration)
        {
            Name = "Regenerating";
            _healAmount = amount;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} begins regenerating.");
        }

        public override void OnTurnPassed(Monster target)
        {
            target.Heal(_healAmount);
            Debug.Log($"{target.Name} heals {_healAmount} HP from regeneration.");
        }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name}'s regeneration ends.");
        }
    }
}
