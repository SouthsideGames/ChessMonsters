using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace ChessMonsterTactics
{
    public class InventorySlot : Selectable
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _count;

        private System.Action _onClick;

        public void Set(Sprite s, Color tint, int count)
        {
            _icon.sprite = s;
            _icon.color = tint;
            _count.text = count.ToString();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _onClick?.Invoke();
        }

        public void SetOnClicked(System.Action onClick)
        {
            _onClick = onClick;
        }
    }
}
