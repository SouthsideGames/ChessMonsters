using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Shell Guard - Grants +2 Defense at the end of the turn if the monster has not moved.
    [System.Serializable]
    public class Passive_ShellGuard : AbilityBehaviour
    {
        [SerializeField] private int _defenseBonus = 2;
        private bool _buffApplied = false;

        public override bool EvaluateTriggerCondition() => false;

        protected override void OnTrigger() { }

        protected override void OnTurnPassed()
        {
            if (!Monster.Moved && !_buffApplied)
            {
                Monster.ApplyDefenseModifier(_defenseBonus);
                _buffApplied = true;

                Debug.Log($"{Monster.Name} gains +{_defenseBonus} Defense from Shell Guard.");
            }
            else if (Monster.Moved && _buffApplied)
            {
                Monster.ApplyDefenseModifier(-_defenseBonus);
                _buffApplied = false;

                Debug.Log($"{Monster.Name} loses Shell Guard defense bonus.");
            }
        }

        public override AbilityBehaviour Copy()
        {
            return new Passive_ShellGuard
            {
                _defenseBonus = this._defenseBonus
            };
        }
    }
}
