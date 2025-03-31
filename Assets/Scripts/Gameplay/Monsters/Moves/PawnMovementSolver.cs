using System.Collections.Generic;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class PawnMovementSolver : MovementSolver
    {
        private IEnumerable<Vector2Int> Diagonal
        {
            get
            {
                yield return new Vector2Int(-1, 0);
                yield return new Vector2Int(1, 0);
            }
        }

        public override IEnumerable<Vector2Int> CheckForValidMovement(Vector2Int from, Chessboard board)
        {
            Monster current = board.GetMonster(from);
            Vector2Int moveDir = current.BoardPosition == BoardPosition.Bottom ? Vector2Int.up : Vector2Int.down;

            int count = current.Moved ? 1 : 2;
            for (int i = 1; i <= count; i++)
            {
                Vector2Int pos = from + (moveDir * i);
                
                // TODO: Check forward diagonal for opponents
                if (TryCheckMonster(pos, board))
                {
                    break;
                }

                yield return pos;
            }

            foreach (Vector2Int d in Diagonal)
            {
                Vector2Int pos = from + moveDir + d;
                if (TryGetMonster(pos, board, out Monster monster))
                {
                    // TODO: Check if monster is oppoent
                    if (current.Owner != monster.Owner)
                    {
                        yield return pos;
                    }
                }
            }
        }
    }
}
