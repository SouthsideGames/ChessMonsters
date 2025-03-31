using Unity.Netcode;
using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    public class MonsterBuilder : INetworkSerializable
    {
        private ulong _owner;
        private ushort _resId;
        private ushort _id;
        private Vector2Int _cellPos;
        private BoardPosition _boardPos;

        public ulong Owner { get => _owner; }
        public Vector2Int CellPos { get => _cellPos; }
        public BoardPosition BoardPosition { get => _boardPos; }

        public MonsterBuilder() { }

        public MonsterBuilder(ushort id, ushort resId)
        {
            if (id == 0)
                _id = HashUtility.Hash16(Random.Range(0, ushort.MaxValue));
            else 
                _id = id; 

            _resId = resId;
        }

        public MonsterBuilder SetOwner(ulong owner)
        {
            _owner = owner;
            return this;
        }

        public MonsterBuilder SetId(ushort id)
        {
            _id = id;
            return this;
        }

        public MonsterBuilder SetResourceId(ushort resId)
        {
            _resId = resId;
            return this;
        }

        public MonsterBuilder SetCellPos(Vector2Int cellPos)
        {
            _cellPos = cellPos;
            return this;
        }

        public MonsterBuilder SetBoardPos(BoardPosition pos)
        {
            _boardPos = pos;
            return this;
        }
    
        public Monster Build(Chessboard chessboard)
        {
            GameObject clone = Object.Instantiate(GameSettings.GetInstance().Gameplay.MonsterPrefab);
            if (!clone.TryGetComponent(out Monster monster))
            {
                Debug.LogError("Prefab does not contains Monster component!");
                return null;
            }

            MonsterData data = ResourceDatabase.Load<MonsterData>(_resId);
            monster.Set(_owner, _id, data, _boardPos);

            chessboard.RegisterMonster(_cellPos, monster);
            return monster;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _owner);
            serializer.SerializeValue(ref _resId);
            serializer.SerializeValue(ref _id);
            serializer.SerializeValue(ref _cellPos);
            serializer.SerializeValue(ref _boardPos);
        }
    }
}
