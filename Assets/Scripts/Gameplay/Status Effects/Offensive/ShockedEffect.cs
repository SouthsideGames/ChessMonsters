using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Shocked - Takes damage if they activate an ability on their next turn
    public class ShockedEffect : StatusEffect
    {
        private int _penaltyDamage;
        private bool _isTriggered = false;

        public ShockedEffect(int damage, int duration)
        {
            Name = "Shocked";
            _penaltyDamage = damage;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is shocked and will take damage if they use an ability.");
        }

        public override void OnTurnPassed(Monster target) { }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name}'s shocked status expired.");
        }

        public void OnAbilityUsed(Monster target)
        {
            if (!_isTriggered)
            {
                _isTriggered = true;
                target.TrackDamage(_penaltyDamage, null);
                DamagePopup.Show(target.transform.position, _penaltyDamage);
                Debug.Log($"{target.Name} triggered Shocked and took {_penaltyDamage} damage.");
            }
        }
    }
}
