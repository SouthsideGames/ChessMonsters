using UnityEngine;

namespace ChessMonsterTactics
{
    public abstract class GameResource : ScriptableObject
    {
        [SerializeField, UID] protected ushort _id;
        [SerializeField] protected string _name;
        [SerializeField, TextArea] protected string _description;
        [SerializeField] protected Sprite _icon;
        [SerializeField] protected Color _iconTint;

        public ushort Id { get => _id; }
        public string Name { get => _name; }
        public string Description { get => _description; }
        public Sprite Icon { get => _icon; }
        public Color IconTint { get => _iconTint; }
    }
}
