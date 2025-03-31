using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Hardened - Temporarily increases Defense
    public class HardenedEffect : StatusEffect
    {
        private int _bonus;

        public HardenedEffect(int amount, int duration)
        {
            Name = "Hardened";
            _bonus = amount;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            target.ApplyDefenseModifier(_bonus);
            Debug.Log($"{target.Name} is hardened (+{_bonus} Defense).");
        }

        public override void OnExpire(Monster target)
        {
            target.ApplyDefenseModifier(-_bonus);
            Debug.Log($"{target.Name} loses Hardened effect.");
        }

        public override void OnTurnPassed(Monster target) { }
    }
}
