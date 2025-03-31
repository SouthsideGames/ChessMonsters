using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Silenced - Prevents use of active abilities
    public class SilencedEffect : StatusEffect
    {
        public SilencedEffect(int duration)
        {
            Name = "Silenced";
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is silenced and cannot use active abilities.");
        }

        public override void OnTurnPassed(Monster target) { }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name} is no longer silenced.");
        }

        public bool PreventsActive() => true;
    }
}
