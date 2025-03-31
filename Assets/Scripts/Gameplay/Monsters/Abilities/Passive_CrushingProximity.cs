using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class Passive_CrushingProximity : AbilityBehaviour
    {
        [SerializeField] private int _damage = 2;

        public override bool EvaluateTriggerCondition()
        {
            return false; // not directly triggered by the host
        }

        protected override void OnTurnPassed()
        {
            Vector2Int currentPos = Monster.CellPosition;
            var chessboard = GameServerManager.Current.Chessboard;
            var adjacentPositions = chessboard.GetAdjacentPositions(currentPos);

            foreach (var pos in adjacentPositions)
            {
                Monster adjacent = chessboard.GetMonster(pos);
                if (adjacent != null && adjacent.Owner != Monster.Owner)
                {
                    // If adjacent monster has Crushing Proximity
                    var passive = adjacent.AbilityController?.Passive;
                    if (passive != null && passive.Behaviour is Passive_CrushingProximity)
                    {
                        Monster.TrackDamage(_damage, adjacent);
                        DamagePopup.Show(Monster.transform.position, _damage);
                        Debug.Log($"{Monster.Name} took {_damage} damage from {adjacent.Name}'s Crushing Proximity.");
                    }
                }
            }
        }

        protected override void OnTrigger() { }

        public override AbilityBehaviour Copy()
        {
            return new Passive_CrushingProximity
            {
                _damage = this._damage
            };
        }
    }
}
