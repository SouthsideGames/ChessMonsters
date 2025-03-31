using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChessMonsterTactics
{
    [RequireComponent(typeof(LayoutElement))]
    public class TeamConfigSlot : MonoBehaviour, 
        IPointerEnterHandler, IPointerExitHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerClickHandler
    {
        private static TeamConfigSlot Selected { get; set;}
        private static TeamConfigSlot Hovered { get; set; }

        [SerializeField] private LayoutElement _layout;
        [SerializeField] private Image _targetGraphic;
        [SerializeField] private Image _icon;
        [Space]
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _hoveredColor;

        private Transform _parent;
        private int _slotIndex;
        private int _slotType;

        public Transform Parent { get => _parent; }
        public LayoutElement LayoutElement { get => _layout; }
        public Image Icon { get => _icon; }
        public int SlotIndex { get => _slotIndex; }
        public int SlotType { get => _slotType; }

        public event System.Action<TeamConfigSlot> OnDragStarted;
        public event System.Action<TeamConfigSlot> OnDragEnded;
        public event System.Action<TeamConfigSlot> OnSlotSelected;
        public event System.Action OnSlotDeselected;

        private void Awake()
        {
            if (_layout == null)
                _layout = GetComponent<LayoutElement>();
        }

        public void Init(int index, int type)
        {
            _slotIndex = index;
            _slotType = type;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Selected != this)
            {
                _targetGraphic.color = _hoveredColor;
            }

            Hovered = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Selected != this)
            {
                _targetGraphic.color = _normalColor;
            }

            Hovered = null;
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            Select();
            OnDragStarted?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnded?.Invoke(Hovered);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected == this)
            {
                OnSlotDeselected?.Invoke();
                Deselect();
            }
            else
            {
                OnSlotSelected?.Invoke(this);
                Select();
            }
        }

        public void Select()
        {
            if (Selected != null)
            {
                if (Selected == this)
                    return;
                    
                Selected.Deselect();
            }

            _targetGraphic.color = _selectedColor;
            Selected = this;
        }

        public void Deselect()
        {
            if (Selected == this)
                Selected = null;

            _targetGraphic.color = _normalColor;
        }

        public void SetIcon(Sprite s, Color color)
        {
            _icon.sprite = s;
            _icon.color = color;
        }
    }
}
