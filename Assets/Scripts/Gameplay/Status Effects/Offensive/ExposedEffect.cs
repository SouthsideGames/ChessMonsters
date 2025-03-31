using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Exposed - Temporarily reduces target’s defense
    public class ExposedEffect : StatusEffect
    {
        private int _defensePenalty;

        public ExposedEffect(int penalty, int duration)
        {
            Name = "Exposed";
            _defensePenalty = penalty;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            target.ApplyDefenseModifier(-_defensePenalty);
            Debug.Log($"{target.Name} is exposed (-{_defensePenalty} Defense).");
        }

        public override void OnExpire(Monster target)
        {
            target.ApplyDefenseModifier(_defensePenalty);
            Debug.Log($"{target.Name}'s exposed effect ended.");
        }

        public override void OnTurnPassed(Monster target) { }
    }
}
