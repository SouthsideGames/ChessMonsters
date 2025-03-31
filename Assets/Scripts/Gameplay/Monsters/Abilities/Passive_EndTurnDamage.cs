using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class Passive_EndOfTurnDamage : AbilityBehaviour
    {
        [SerializeField] private int _damage = 2;

        public override bool EvaluateTriggerCondition() => false;


       protected override void OnTurnPassed()
        {
            Vector2Int direction = Monster.BoardPosition.ToDirection();
            Vector2Int forward = Monster.CellPosition + direction;

            if (!GameServerManager.Current.Chessboard.IsWithinBounds(forward))
                return;

            Monster target = GameServerManager.Current.Chessboard.GetMonster(forward);
            if (target != null && target.Owner != Monster.Owner)
            {
                target.TrackDamage(_damage, Monster);
                DamagePopup.Show(target.transform.position, _damage);
                Debug.Log($"{target.Id} took {_damage} damage from EndOfTurnDamage.");
            }
        }

        protected override void OnTrigger()
        {
            // Not used for passives
        }

        public override AbilityBehaviour Copy()
        {
            return new Passive_EndOfTurnDamage
            {
                _damage = this._damage
            };
        }
    }
}
