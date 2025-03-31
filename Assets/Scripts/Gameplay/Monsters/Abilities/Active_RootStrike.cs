using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public class Active_RootStrike : AbilityBehaviour
    {
        [SerializeField] private int _damage = 5;

        public override bool EvaluateTriggerCondition()
        {
            Debug.Log(Monster.Id + " evaluating trigger condition.");
            return GameServerManager.Current.ActiveAbilityEnabled;
        }

        protected override void OnTrigger()
        {
            ushort targeted = GameServerManager.Current.GetTargetedMonsterId();
            if (targeted == 0)
                return;

            Monster monster = GameServerManager.Current.Chessboard.GetMonster(targeted);
            if (monster != null)
            {
                monster.TakeDamage(_damage);
                DamagePopup.Show(monster.transform.position, _damage);
                
                Debug.Log(monster.Id + " received " + _damage + " from Root Strike ability.");
            }
        }

        public override AbilityBehaviour Copy()
        {
            return new Active_RootStrike
            {
                _damage = this._damage
            };
        }
    }
}
