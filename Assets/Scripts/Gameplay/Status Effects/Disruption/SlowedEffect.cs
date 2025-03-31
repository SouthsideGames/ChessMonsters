using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Slowed - Prevents use of ultimate abilities
    public class SlowedEffect : StatusEffect
    {
        public SlowedEffect(int duration)
        {
            Name = "Slowed";
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is slowed and cannot use ultimate abilities.");
        }

        public override void OnTurnPassed(Monster target) { }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name} is no longer slowed.");
        }

        public bool PreventsUltimate() => true;
    }
}
