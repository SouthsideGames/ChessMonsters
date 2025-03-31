using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    public class Chessboard : NetworkBehaviour
    {
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private Monster _gameplayPiece;

        private Node[] _nodes;
        private Transform _container;

        public int Width { get => _width; }
        public int Height { get => _height; }

        public IEnumerable<Monster> Monsters
        {
            get
            {
                foreach (Node node in _nodes)
                {
                    if (node.Monster != null)
                        yield return node.Monster;   
                }
            }
        }

        public void Initialize()
        {
            _nodes = new Node[_width * _height];
            int index = 0;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _height; x++)
                {
                    _nodes[index] = new Node();
                    _nodes[index].Position = _offset + new Vector2(x, y);
                    _nodes[index].CellPosition = WorldToCell(_nodes[index].Position);

                    index++;
                }
            }

            _container = new GameObject("Pieces").transform;
            transform.SetParent(transform);
        }

        public void SpawnMonster(ulong owner, Vector2Int pos, ushort resId, BoardPosition position)
        {
            Node node = GetNode(pos);
            MonsterData monsterData = ResourceDatabase.Load<MonsterData>(resId);

            Monster copy = Instantiate(_gameplayPiece);
            copy.Set(owner, 0, monsterData, position);
            copy.transform.position = node.Position;
            copy.transform.parent = _container;
            
            node.Monster = copy;
        }

        public void RegisterMonster(Vector2Int pos, Monster monster)
        {
            Node node = GetNode(pos);
            node.SetMonster(monster, true);
            // node.Monster = monster;
            // node.Monster.CellPosition = pos;
            // node.Monster.transform.position = node.Position;
            // node.SetMonster(monster);
        }

        public Monster GetMonster(Vector2Int cellPos)
        {
            Node node = GetNode(cellPos);
            return node?.Monster;
        }

        public Monster GetMonster(Vector2 worldPos)
        {
            return GetMonster(WorldToCell(worldPos));
        }

        public Monster GetMonster(ushort id)
        {
            foreach (Monster monster in Monsters)
            {
                if (monster.Id == id)
                    return monster;
            }

            return null;
        }

        public Vector2Int GetCellPosition(ushort pieceId)
        {
            Monster monster = GetMonster(pieceId);
            if (monster != null)
                return monster.CellPosition;

            return new Vector2Int(-1, -1);
        }

        public Node GetNode(Vector2Int pos)
        {
            if (!IsWithinBounds(pos))
                return null;
                
            int index = (pos.y * _width) + pos.x;
            if (index < 0 || index >= _nodes.Length)
                return null;

            return _nodes[index];
        }

        public Node GetNode(Vector2 worldPos)
        {
            Vector2Int cellPos = WorldToCell(worldPos);            
            if (cellPos.x < 0 || cellPos.x >= _width)
                return null;
            
            if (cellPos.y < 0 || cellPos.y >= _height)
                return null;

            return GetNode(cellPos);
        }

        public Node GetNode(ushort pieceId)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].Monster != null && _nodes[i].MonsterId == pieceId) 
                {
                    return _nodes[i];
                }
            }

            return null;
        }

        public bool IsWithinBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < _width && pos.y >= 0 && pos.y < _height;
        }

        public Vector2Int WorldToCell(Vector2 world, bool mirror = false)
        {
            float halfWidth = _width * 0.5f;
            float halfHeight = _height * 0.5f;

            Vector2 localPos = world - (Vector2)transform.position;
            if (mirror)
                localPos.y = -localPos.y;

            localPos += new Vector2(halfWidth, halfHeight);

            return new Vector2Int((int)localPos.x, (int)localPos.y);
        }

        public Vector3 CellToWorld(Vector2Int cell)
        {
            Node node = GetNode(cell);
            if (node != null)
                return node.Position;

            return Vector3.zero;
        }

        public Vector2Int MirrorY(Vector2Int v)
        {
            int y = _height - (v.y + 1);
            return new Vector2Int(v.x, y);
        }

        public Vector2 MirrorY(Vector2 v)
        {
            Vector2 localPos = v - (Vector2)transform.position;
            localPos.y = -localPos.y;

            return localPos;
        }


        //New Code
        public List<Vector2Int> GetAdjacentPositions(Vector2Int center)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),    // Up
                new Vector2Int(1, 0),    // Right
                new Vector2Int(0, -1),   // Down
                new Vector2Int(-1, 0),   // Left
            };

            foreach (var dir in directions)
            {
                Vector2Int check = center + dir;
                if (IsWithinBounds(check))
                    positions.Add(check);
            }

            return positions;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && _nodes != null)
            {
                foreach (Node node in _nodes)
                {
                    Gizmos.DrawWireCube(node.Position, Vector2.one);
                }

                return;
            }

            Gizmos.color = Color.green;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _height; x++)
                {
                    if (x == 0 && y == 0)
                        Gizmos.color = Color.yellow;
                    else
                        Gizmos.color = Color.green;

                    Gizmos.DrawWireCube(_offset + new Vector2(x, y), Vector2.one);
                }
            }
        }
#endif

        [System.Serializable]
        public class Node
        {
            public Vector2 Position;
            public Vector2Int CellPosition;
            public Monster Monster;

            public ushort MonsterId { get => Monster.Id; }

            public void SetMonster(Monster monster, bool setTransform = false)
            {
                if (monster == null)
                {
                    Monster = null;
                    return;
                }
                
                Monster = monster;
                Monster.CellPosition = CellPosition;

                if (setTransform)
                    Monster.transform.position = Position;
            }
        }
    }
}
