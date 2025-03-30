using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

namespace ChessMonsterTactics
{
    public class InventoryUI : UIPanel
    {
        [SerializeField] private Transform _container;
        [SerializeField] private TMP_Text _info;

        private List<Item> _items;

        private System.Func<GameResource, bool> _filter;

        public event System.Action<ushort> OnItemClicked;

        private void LoadInventoryItems()
        {
            _items = new List<Item>();

            foreach (Inventory.Item item in Game.CurrentProfile.Inventory.Items)
            {
                _items.Add(new Item { Id = item.ItemId, Level = item.Level, Count = item.Count });
            }

            UpdateInventorySlots();
        }

        private void UpdateInventorySlots()
        {
            if (_items == null)
                return;

            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.GetChild(i).gameObject);
            }

            InventorySlot prefab = UISettings.GetInstance().Inventory.SlotPrefab;

            int displayedItems = 0;
            for (int i = 0; i < _items.Count; i++)
            {
                Item item = _items[i];
                GameResource res = ResourceDatabase.Load<GameResource>(item.Id);

                if (_filter?.Invoke(res) == false)
                {
                    continue;
                }

                InventorySlot slot = Instantiate(prefab, _container);

                slot.Set(res.Icon, res.IconTint, item.Count);
                slot.SetOnClicked(() => OnItemClicked?.Invoke(res.Id));

                displayedItems++;
            }

            if (displayedItems <= 0)
            {
                SetInfoText("You don't have any compatible item.");
            }
            else
            {
                SetInfoText(null);
            }
        }

        private void SetInfoText(string text)
        {
            _info.text = text;
            _info.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        public void SetFilter(System.Func<GameResource, bool> filter)
        {
            _filter = filter;
            UpdateInventorySlots();
        }

        public bool RemoveItem(ushort id, int level, int count)
        {
            int index = GetItemIndex(id, level);
            if (index < 0)
                return false;
            
            _items[index].Count -= count;
            if (_items[index].Count <= 0)
                _items.RemoveAt(index);

            UpdateInventorySlots();

            return true;
        }

        public bool HasItem(ushort id, int level, int count)
        {
            int index = GetItemIndex(id, level);
            if (index >= 0)
            {
                Item item = _items[index];
                if (item.Count < count)
                {
                    Debug.Log("Insufictient count.");
                    return false;
                }
            } 

            return true;
        }

        private int GetItemIndex(ushort id, int level)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Item item = _items[i];
                if (item.Id == id && item.Level == level)
                {
                    return i;
                }
            }

            return -1;
        }

        public override YieldInstruction Show(bool useTransition = true)
        {
            SetInfoText(string.Empty);
            LoadInventoryItems();
        
            if (!useTransition)
            {
                RectTransform.position = Vector2.zero;
                gameObject.SetActive(true);
                return null;
            }

            return RectTransform.DOAnchorPos(Vector2.zero, 0.15f)
                .WaitForCompletion();
        }

        public override YieldInstruction Hide(bool useTransition = true)
        {
            float height = RectTransform.rect.height;
            Vector2 targetPos = new Vector2(0, -height);

            if (!useTransition)
            {
                RectTransform.anchoredPosition = targetPos;
                gameObject.SetActive(true);
                SetFilter(null);
                return null;
            }

            return RectTransform.DOAnchorPos(targetPos, 0.15f)
                .OnComplete(() => SetFilter(null))
                .WaitForCompletion();
        }

        private class Item
        {
            public ushort Id;
            public int Level;
            public int Count;
        }
    }
}
