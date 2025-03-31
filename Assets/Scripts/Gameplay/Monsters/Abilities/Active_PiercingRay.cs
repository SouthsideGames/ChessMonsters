using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Piercing Ray - Active ability that deals 7 damage to the first enemy found in a straight line 3 tiles ahead.
    [System.Serializable]
    public class Active_PiercingRay : AbilityBehaviour
    {
        [SerializeField] private int _damage = 7;
        [SerializeField] private int _range = 3;

        public override bool EvaluateTriggerCondition()
        {
            return GameServerManager.Current.ActiveAbilityEnabled;
        }

        protected override void OnTrigger()
        {
            var chessboard = GameServerManager.Current.Chessboard;
            Vector2Int origin = Monster.CellPosition;
            Vector2Int direction = Monster.BoardPosition.ToDirection();

            for (int i = 1; i <= _range; i++)
            {
                Vector2Int check = origin + (direction * i);
                if (!chessboard.IsWithinBounds(check)) break;

                Monster target = chessboard.GetMonster(check);
                if (target != null && target.Owner != Monster.Owner)
                {
                    target.TrackDamage(_damage, Monster);
                    DamagePopup.Show(target.transform.position, _damage);
                    Debug.Log($"{target.Name} was struck by Piercing Ray from {Monster.Name} for {_damage}.");
                    break;
                }
            }
        }

        public override AbilityBehaviour Copy()
        {
            return new Active_PiercingRay
            {
                _damage = this._damage,
                _range = this._range
            };
        }
    }
}
