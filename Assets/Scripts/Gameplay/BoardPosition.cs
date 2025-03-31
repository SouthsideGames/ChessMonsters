using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public enum BoardPosition : ushort
    {
        None, Top, Bottom
    }

    public static class BoardPositionExtensions
    {
        public static Vector2Int ToDirection(this BoardPosition position)
        {
            return position switch
            {
                BoardPosition.Top => new Vector2Int(0, 1),
                BoardPosition.Bottom => new Vector2Int(0, -1),
                _ => Vector2Int.zero
            };
        }
    }
}
