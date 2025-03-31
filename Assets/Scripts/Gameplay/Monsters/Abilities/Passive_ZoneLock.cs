using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class Passive_ZoneLock : AbilityBehaviour
    {
        public override bool EvaluateTriggerCondition()
        {
            Vector2Int currentPos = Monster.CellPosition;
            var chessboard = GameServerManager.Current.Chessboard;
            var adjacentPositions = chessboard.GetAdjacentPositions(currentPos);

            foreach (var pos in adjacentPositions)
            {
                Monster adjacent = chessboard.GetMonster(pos);
                if (adjacent != null && adjacent.Owner != Monster.Owner)
                {
                    AbilityController controller = adjacent.AbilityController;
                    if (controller != null && controller.Active != null && controller.Active.Behaviour != null)
                    {
                        Debug.Log($"Zone Lock prevents {adjacent.Name}'s active ability.");
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void OnTrigger() { }

        public override AbilityBehaviour Copy()
        {
            return new Passive_ZoneLock();
        }
    }
}
