using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Gaze Pulse - Active ability that marks a target for 1 turn, causing them to take +3 extra damage from all sources.
    [System.Serializable]
    public class Active_GazePulse : AbilityBehaviour
    {
        [SerializeField] private int _bonusDamage = 3;
        [SerializeField] private int _duration = 1;

        public override bool EvaluateTriggerCondition()
        {
            return GameServerManager.Current.ActiveAbilityEnabled;
        }

        protected override void OnTrigger()
        {
            ushort targetId = GameServerManager.Current.GetTargetedMonsterId();
            Monster target = GameServerManager.Current.Chessboard.GetMonster(targetId);

            if (target != null && target.Owner != Monster.Owner)
            {
                target.AddStatusEffect(new MarkedEffect(_bonusDamage, _duration));
                Debug.Log($"{target.Name} has been marked by Gaze Pulse.");
            }
        }

        public override AbilityBehaviour Copy()
        {
            return new Active_GazePulse
            {
                _bonusDamage = this._bonusDamage,
                _duration = this._duration
            };
        }
    }
}
