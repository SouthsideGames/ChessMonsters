using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // MarkedEffect - Status effect that causes the target to take +X bonus damage from all sources for a limited duration.
    public class MarkedEffect : StatusEffect
    {
        private int _bonusDamage;

        public MarkedEffect(int bonus, int duration)
        {
            Name = "Marked";
            _bonusDamage = bonus;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is marked and will take +{_bonusDamage} damage.");
        }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name}'s marked effect expired.");
        }

        public override void OnTurnPassed(Monster target) { }

        public int GetBonusDamage() => _bonusDamage;
    }
}
