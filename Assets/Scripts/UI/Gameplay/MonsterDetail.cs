using System.Collections;
using ChessMonsterTactics.Gameplay;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.EventSystems;

namespace ChessMonsterTactics.UI
{
    public class MonsterDetail : BottomPanel, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _monsterName;
        [SerializeField] private ProgressBar _healthBar;
        [Space]
        [SerializeField] private AbilitySlot _passive;
        [SerializeField] private AbilitySlot _active;
        [SerializeField] private AbilitySlot _ultimate;

        private bool _hovered;

        public bool Hovered { get => _hovered; }

        public event System.Action<bool> OnActiveAbilityToggled;

        public void UpdateUI(Monster monster)
        {
            _monsterName.text = monster.Owner + " | " + monster.Name + " | " + monster.Id;
            _healthBar.SetMinMax(0, monster.MaxHealth);
            _healthBar.SetValue(monster.CurrentHealth);
            _healthBar.SetColor(monster.Owner == NetworkManager.Singleton.LocalClientId ? 
                UISettings.GetInstance().PlayerHealthBarColor : UISettings.GetInstance().OpponentHealthBarColor);

            // TODO: These should be used with AbilityData instead of ability behavior
            InitializeAbilitySlot(_passive, monster.AbilityController.Passive);
            InitializeAbilitySlot(_active, monster.AbilityController.Active);
            InitializeAbilitySlot(_ultimate, monster.AbilityController.Ultimate);
        }

        public YieldInstruction Show(Monster monster, bool useTransition = true)
        {
            IEnumerator exec()
            {
                _passive.Toggle(false);
                _active.Toggle(false);
                _ultimate.Toggle(false);
                
                UpdateUI(monster);
                yield return Show(useTransition);
            }

            return StartCoroutine(exec());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
        }
    
        private void InitializeAbilitySlot(AbilitySlot slot, AbilityData abilityData)
        {
            if (slot.gameObject == null)
                return;

            slot.gameObject.SetActive(abilityData);
            if (abilityData == null)
                return;
                
            slot.Set(abilityData, abilityData.AbilityType == AbilityType.Active ? OnActiveAbilityToggled : null);
            slot.UpdateAbilityCooldown(abilityData.Behaviour);
        }
    }
}
