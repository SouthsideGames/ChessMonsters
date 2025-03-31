using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    // Burning - Deals fixed damage at the end of each turn
    public class BurningEffect : StatusEffect
    {
        private int _damagePerTurn;

        public BurningEffect(int damage, int duration)
        {
            Name = "Burning";
            _damagePerTurn = damage;
            Duration = duration;
        }

        public override void OnApply(Monster target)
        {
            Debug.Log($"{target.Name} is burning!");
        }

        public override void OnTurnPassed(Monster target)
        {
            target.TrackDamage(_damagePerTurn, null);
            DamagePopup.Show(target.transform.position, _damagePerTurn);
            Debug.Log($"{target.Name} takes {_damagePerTurn} burning damage.");
        }

        public override void OnExpire(Monster target)
        {
            Debug.Log($"{target.Name} is no longer burning.");
        }
    }
}
