using Unity.Netcode;

namespace ChessMonsterTactics
{
    public struct AttackData : INetworkSerializable
    {
        private int _damage;
        private int _defense;
        private bool _isDead;
        private bool _isCritical;

        public int Damage { get => _damage; set => _damage = value; }
        public int Defense { get => _defense; set => _defense = value; }
        public bool IsDead { get => _isDead; set => _isDead = value; }
        public bool IsCritical { get => _isCritical; set => _isCritical = value; }

        public int TotalDamage { get => _damage - _defense; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _damage);
            serializer.SerializeValue(ref _defense);
            serializer.SerializeValue(ref _isDead);
            serializer.SerializeValue(ref _isCritical);
        }
    }
}
