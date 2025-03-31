using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Poisoned - Takes increasing damage every turn
    public class PoisonedEffect : StatusEffect
    {
        private int _baseDamage;
        private int _turnsElapsed = 0;

        public PoisonedEffect(int damage, int duration)
        {
            Name = "Poisoned";
            _baseDamage = damage;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is poisoned!");
        }

        public override void OnTurnPassed(Monster target)
        {
            _turnsElapsed++;
            int poisonDamage = _baseDamage + _turnsElapsed - 1;
            target.TrackDamage(poisonDamage, null);
            DamagePopup.Show(target.transform.position, poisonDamage);
            Debug.Log($"{target.Name} takes {poisonDamage} poison damage.");
        }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name} is no longer poisoned.");
        }
    }
}
