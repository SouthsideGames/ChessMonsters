using UnityEngine;

namespace ChessMonsterTactics
{
    public class TeamConfigUI : UIPanel
    {
        [SerializeField] private InventoryUI _inventory;
        [SerializeField] private TeamConfigGrid _grid;

        private TeamConfigSlot _selectedSlot;

        private void Start()
        {
            // Initialize team config grids
            Hide(false);

            _inventory.OnItemClicked += OnInventoryItemClicked;

            _grid.Initialize();
            _grid.OnSlotSelected += OnTeamConfifSlotSelected;
            _grid.OnSlotDeselected += OnTeamCongigSlotDeselected;
            _grid.OnSlotSwapped += OnSlotSwapped;
        }

        private void OnTeamConfifSlotSelected(TeamConfigSlot slot)
        {
            _selectedSlot = slot;
            _inventory.SetFilter(res => {
                if (res is MonsterData md && (int)md.Type == slot.SlotType)
                    return true;

                return false;
            });
        }

        private void OnTeamCongigSlotDeselected()
        {
            _selectedSlot = null;
            _inventory.SetFilter(null);
        }
    
        private void OnInventoryItemClicked(ushort id)
        {
            if (_selectedSlot == null)
                return;

            if (Game.CurrentProfile.TeamConfig.Pieces[_selectedSlot.SlotIndex] != 0)
                return;

            MonsterData monsterData = ResourceDatabase.Load<MonsterData>(id);
            _selectedSlot.SetIcon(monsterData.Sprite, monsterData.SpriteTint);
            _inventory.RemoveItem(id, 0, 1);

            // Set the team config
            Game.CurrentProfile.TeamConfig.Pieces[_selectedSlot.SlotIndex] = monsterData.Id;
            Game.CurrentProfile.Inventory.RemoveItem(id, 0, 1);

            // TODO: Save when exiting
            Game.SaveUserProfile();
        }

        private void OnSlotSwapped(int a, int b)
        {
            ushort aId = Game.CurrentProfile.TeamConfig.Pieces[a];
            Game.CurrentProfile.TeamConfig.Pieces[a] = Game.CurrentProfile.TeamConfig.Pieces[b];
            Game.CurrentProfile.TeamConfig.Pieces[b] = aId;

            Game.SaveUserProfile();
        }

        public override YieldInstruction Show(bool useTransition = true)
        {
            _inventory.Show(useTransition);
            return base.Show(useTransition);
        }

        public override YieldInstruction Hide(bool useTransition = true)
        {
            if (_selectedSlot != null)
                _selectedSlot.Deselect();
                
            _inventory.Hide(useTransition);
            return base.Hide(useTransition);
        }
    }
}
