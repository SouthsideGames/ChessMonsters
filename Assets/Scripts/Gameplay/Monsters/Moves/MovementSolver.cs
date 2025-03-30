using System.Collections.Generic;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public abstract class MovementSolver
    {
        public abstract IEnumerable<Vector2Int> CheckForValidMovement(Vector2Int from, Chessboard board);

        protected IEnumerable<Vector2Int> GetNodesInDirection(Vector2Int from, Vector2Int dir, Chessboard board)
        {
            for (Vector2Int pos = from + dir; board.IsWithinBounds(pos); pos += dir)
            {
                Chessboard.Node node = board.GetNode(pos);
                if (node != null)
                {
                    yield return pos;

                    if (node.Monster != null)
                        break;
                }
            }
        }

        protected IEnumerable<Vector2Int> GetNodesInDirections(Vector2Int from, IEnumerable<Vector2Int> dirs, Chessboard board)
        {
            foreach (Vector2Int dir in dirs)
            {
                foreach (Vector2Int pos in GetNodesInDirection(from, dir, board))
                {
                    yield return pos;
                }
            }
        }

        protected bool TryGetMonster(Vector2Int pos, Chessboard board, out Monster monster)
        {
            Chessboard.Node n = board.GetNode(pos);
            monster = n?.Monster;

            return monster != null;
        }

        protected bool TryCheckMonster(Vector2Int pos, Chessboard board)
        {
            return board.GetNode(pos)?.Monster != null;
        }
    }
}