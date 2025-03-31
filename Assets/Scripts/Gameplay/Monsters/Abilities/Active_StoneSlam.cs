using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Stone Slam - Deals 5 damage to the target and 3 damage to adjacent enemies.
    [System.Serializable]
    public class Active_StoneSlam : AbilityBehaviour
    {
        [SerializeField] private int _primaryDamage = 5;
        [SerializeField] private int _splashDamage = 3;

        public override bool EvaluateTriggerCondition()
        {
            // Make sure the ability can be used this turn
            return GameServerManager.Current.ActiveAbilityEnabled;
        }

        protected override void OnTrigger()
        {
            // Get the targeted monster
            ushort targetId = GameServerManager.Current.GetTargetedMonsterId();
            if (targetId == 0) return;

            Monster target = GameServerManager.Current.Chessboard.GetMonster(targetId);
            if (target == null || target.Owner == Monster.Owner)
                return;

            // Apply primary damage to the selected target
            target.TrackDamage(_primaryDamage, Monster);
            DamagePopup.Show(target.transform.position, _primaryDamage);
            Debug.Log($"{target.Name} took {_primaryDamage} damage from Stone Slam (primary).");

            // Get adjacent enemies to the target and deal splash damage
            var adjacentPositions = GameServerManager.Current.Chessboard.GetAdjacentPositions(target.CellPosition);
            foreach (var pos in adjacentPositions)
            {
                Monster splashTarget = GameServerManager.Current.Chessboard.GetMonster(pos);
                if (splashTarget != null && splashTarget.Owner != Monster.Owner && splashTarget.Id != target.Id)
                {
                    splashTarget.TrackDamage(_splashDamage, Monster);
                    DamagePopup.Show(splashTarget.transform.position, _splashDamage);
                    Debug.Log($"{splashTarget.Name} took {_splashDamage} splash damage from Stone Slam.");
                }
            }
        }

        public override AbilityBehaviour Copy()
        {
            return new Active_StoneSlam
            {
                _primaryDamage = this._primaryDamage,
                _splashDamage = this._splashDamage
            };
        }
    }
}
