using ChessMonsterTactics.Gameplay;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChessMonsterTactics.UI
{
    public class AbilitySlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool _interactable = true;
        [Space]
        [SerializeField] private TMP_Text _abilityName;
        [SerializeField] private TMP_Text _abilityType;
        [Header("Icons")]
        [SerializeField] private Image _border;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _cooldownMask;
        [SerializeField] private TMP_Text _cooldownText;
        [Space]
        [SerializeField] private string _abilityTypeName;

        private bool _enabled;
        private System.Action<bool> _onClicked;

        public bool Interactable { get => _interactable; set => _interactable = value; }

        private void OnValidate()
        {
            if (_abilityType != null)
                _abilityType.text = _abilityTypeName;
        }

        public void Set(AbilityData ability, System.Action<bool> onClicked = null)
        {
            if (ability == null)
            {
                // _icon.sprite = null;
                _abilityName.text = "-";
            }
            else
            {
                if (ability.Icon != null)
                    _icon.sprite = ability.Icon;

                _abilityName.text = ability.Name;
            }

            _abilityType.text = _abilityTypeName;

            _onClicked = null;
            _onClicked = onClicked;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            Toggle(!_enabled);

            _onClicked?.Invoke(_enabled);
        }

        public void UpdateAbilityCooldown(AbilityBehaviour behaviour)
        {
            if (behaviour == null)
                return;
            
            bool cooldown = behaviour.RemainingCooldown > 0;
            _cooldownText.gameObject.SetActive(cooldown);
            _cooldownMask.gameObject.SetActive(cooldown);
            _interactable = !cooldown;

            _cooldownText.text = behaviour.RemainingCooldown.ToString();
            
            if (behaviour.Cooldown != 0)
                _cooldownMask.fillAmount = Mathf.Clamp01(behaviour.RemainingCooldown / behaviour.Cooldown);
            else
                _cooldownMask.fillAmount = 0;
        }
    
        public void Toggle(bool active)
        {
            // Activate active ability
            UISettings settings = UISettings.GetInstance();

            _enabled = active;
            _border.DOColor(_enabled ? settings.AbilitySlotActiveColor : settings.AbilitySlotNormalColor, 
                settings.AbilitySlotTransition);
        }
    }
}
