using UnityEngine;
using UnityEngine.UI;

namespace ChessMonsterTactics
{
    public class TeamConfigGrid : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _container;
        [SerializeField] private float _spacing;
        [SerializeField] private MonsterType[] _slots;

        private Image _placeholder;
        private TeamConfigSlot _draggedSlot;

        public event System.Action<TeamConfigSlot> OnSlotSelected;
        public event System.Action OnSlotDeselected;
        public event System.Action<int, int> OnSlotSwapped;

        public void Initialize()
        {
            InitializeSlots();
            InitializePlaceholderElement();
        }

        private void LateUpdate()
        {
            if (_draggedSlot != null)
            {
                _placeholder.transform.position = Input.mousePosition;
            }
        }

        private void InitializeSlots()
        {
            int count = _slots.Length / 2;
            RectTransform rt = (RectTransform)_container.transform;
            float size = (rt.rect.width / count) - _spacing;
            _container.cellSize = new Vector2(size, size);
            _container.spacing = new Vector2(_spacing, _spacing);

            TeamConfigSlot prefab = UISettings.GetInstance().TeamConfig.SlotPrefab;
            TeamConfig config = Game.CurrentProfile.TeamConfig;

            for (int i = 0; i < count * 2; i++)
            {
                int index = i;
                TeamConfigSlot slot = Instantiate(prefab, _container.transform);
                slot.Init(index, (int)_slots[i]);
                slot.OnDragStarted += OnSlotDragStarted;
                slot.OnDragEnded += OnSlotDragEnded;
                slot.OnSlotSelected += (x) => OnSlotSelected?.Invoke(x);
                slot.OnSlotDeselected += () => OnSlotDeselected?.Invoke();

                if (config.Pieces[i] != 0)
                {
                    MonsterData res = ResourceDatabase.Load<MonsterData>(config.Pieces[i]);
                    slot.SetIcon(res.Sprite, res.SpriteTint);
                }
                else
                {
                    switch (_slots[i])
                    {
                        default:
                        case MonsterType.Pawn:
                            slot.SetIcon(UISettings.GetInstance().TeamConfig.PawnIcon, Color.white);
                            break;
                        case MonsterType.Rook:
                            slot.SetIcon(UISettings.GetInstance().TeamConfig.RookIcon, Color.white);
                            break;
                        case MonsterType.Knight:
                            slot.SetIcon(UISettings.GetInstance().TeamConfig.KnightIcon, Color.white);
                            break;
                        case MonsterType.Bishop:
                            slot.SetIcon(UISettings.GetInstance().TeamConfig.BishopIcon, Color.white);
                            break;
                        case MonsterType.Queen:
                            slot.SetIcon(UISettings.GetInstance().TeamConfig.QueenIcon, Color.white);
                            break;
                        case MonsterType.King:
                            slot.SetIcon(UISettings.GetInstance().TeamConfig.KingIcon, Color.white);
                            break;
                    }
                }
            }
        }

        private void InitializePlaceholderElement()
        {
            GameObject g = new GameObject("_placeholder_");
            g.transform.SetParent(transform);
            g.transform.localScale = Vector2.one;

            _placeholder = g.AddComponent<Image>();
            _placeholder.raycastTarget = false;

            _placeholder.gameObject.SetActive(false);
        }

        private void OnSlotDragStarted(TeamConfigSlot slot)
        {
            _draggedSlot = slot;

            _placeholder.sprite = slot.Icon.sprite;
            _placeholder.color = slot.Icon.color;
            _placeholder.gameObject.SetActive(true);
        }

        private void OnSlotDragEnded(TeamConfigSlot slot)
        {
            _placeholder.gameObject.SetActive(false);
            _placeholder.sprite = null;
            _placeholder.color = Color.white;

            if (slot != null && slot != _draggedSlot)
            {
                if (_draggedSlot.SlotType == slot.SlotType)
                {
                    Sprite s = _draggedSlot.Icon.sprite;
                    Color c = _draggedSlot.Icon.color;

                    _draggedSlot.SetIcon(slot.Icon.sprite, slot.Icon.color);
                    slot.SetIcon(s, c);

                    _draggedSlot.Deselect();
                    OnSlotSwapped?.Invoke(_draggedSlot.SlotIndex, slot.SlotIndex);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.Log("Slot type mismatch.");
                }
#endif
            }

            _draggedSlot = null;
        } 
    
    }
}
