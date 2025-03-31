using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Vital Surge - Temporarily increases max health
    public class VitalSurgeEffect : StatusEffect
    {
        private int _bonus;

        public VitalSurgeEffect(int amount, int duration)
        {
            Name = "Vital Surge";
            _bonus = amount;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            target.ApplyMaxHealthModifier(_bonus);
            Debug.Log($"{target.Name} gains +{_bonus} max HP from Vital Surge.");
        }

        public override void OnExpire(Monster target)
        {
            target.ApplyMaxHealthModifier(-_bonus);
            Debug.Log($"{target.Name}'s Vital Surge has faded.");
        }

        public override void OnTurnPassed(Monster target) { }
    }
}
