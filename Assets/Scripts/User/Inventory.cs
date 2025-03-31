using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChessMonsterTactics
{
    public class Inventory
    {
        [JsonProperty] private List<Item> _items = new List<Item>();

        public IEnumerable<Item> Items
        {
            get
            {
                foreach (Item item in _items)
                {
                    yield return item;
                }
            }
        }

        private int GetItemIndex(int id, int level)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].ItemId == id && _items[i].Level == level)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetItemCount(int id, int level)
        {
            int index = GetItemIndex(id, level);
            if (index <= -1)
                return -1;

            return _items[index].Count;
        }

        public void AddItem(GameResource res, int level, int count)
        {
            AddItem(res.Id, level, count);
        }

        public void AddItem(ushort id, int level, int count)
        {
            int index = GetItemIndex(id, level);
            if (index >= 0)
            {
                Item owned = _items[index];
                _items[index] = new Item(id, level, owned.Count + count);
            }
            
            _items.Add(new Item(id, level, count));
        }

        public void RemoveItem(ushort id, int level, int count)
        {
            int index = GetItemIndex(id, level);
            if (index >= 0)
            {
                Item owned = _items[index];
                if (owned.Count - count <= 0)
                {
                    _items.RemoveAt(index);
                }
                else
                {
                    _items[index] = new Item(id, level, owned.Count - count);
                }
            }
        }

        [System.Serializable]
        public readonly struct Item
        {
            public static Item NULL { get => new Item(0, 0, 0); }

            [JsonProperty] private readonly ushort _itemId;
            [JsonProperty] private readonly int _level;
            [JsonProperty] private readonly int _count;

            [JsonIgnore] public ushort ItemId { get => _itemId; }
            [JsonIgnore] public int Level { get => _level; }
            [JsonIgnore] public int Count { get => _count; }

            [JsonIgnore] public bool IsNull { get => _itemId <= 0; }

            public Item(ushort id, int level, int count)
            {
                _itemId = id;
                _level = level;
                _count = count;
            }
        }
    }
}
