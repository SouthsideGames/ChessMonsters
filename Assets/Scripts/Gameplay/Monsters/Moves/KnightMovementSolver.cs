using System.Collections.Generic;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class KnightMovementSolver : MovementSolver
    {
        private IEnumerable<Vector2Int> Positions
        {
            get
            {
                yield return new Vector2Int(-1, 2);  // North
                yield return new Vector2Int(1, 2);   // North
                yield return new Vector2Int(-1, -2); // South
                yield return new Vector2Int(1, -2);  // South
                yield return new Vector2Int(-2, 1);  // West
                yield return new Vector2Int(-2, -1); // West
                yield return new Vector2Int(2, 1);   // East
                yield return new Vector2Int(2, -1);  // East
            }
        }

        public override IEnumerable<Vector2Int> CheckForValidMovement(Vector2Int from, Chessboard board)
        {
            Monster current = board.GetMonster(from);
            foreach (Vector2Int tpos in Positions)
            {
                Vector2Int pos = from + tpos;
                if (!board.IsWithinBounds(pos))
                    continue;

                if (TryGetMonster(pos, board, out Monster monster))
                {
                    if (monster.Owner == current.Owner)
                        continue;
                }
                
                yield return pos;
            }
        }
    }
}
