using System.Collections.Generic;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class KingMovementSolver : MovementSolver
    {
        private IEnumerable<Vector2Int> Positions
        {
            get
            {
                yield return new Vector2Int(-1, 1);
                yield return new Vector2Int(0, 1);
                yield return new Vector2Int(1, 1);

                yield return new Vector2Int(-1, 0);
                yield return new Vector2Int(1, 0);

                yield return new Vector2Int(-1, -1);
                yield return new Vector2Int(0, -1);
                yield return new Vector2Int(1, -1);
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
