using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Suppressed - Disables passive ability
    public class SuppressedEffect : StatusEffect
    {
        public SuppressedEffect(int duration)
        {
            Name = "Suppressed";
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is suppressed. Passive ability is disabled.");
            var passive = target.AbilityController?.Passive;
            if (passive != null)
                passive.IsSuppressed = true;
        }

        public override void OnExpire(Monster target)
        {
            var passive = target.AbilityController?.Passive;
            if (passive != null)
                passive.IsSuppressed = false;

            Debug.Log($"{target.Name}'s passive ability is re-enabled.");
        }

        public override void OnTurnPassed(Monster target) { }
    }
}
