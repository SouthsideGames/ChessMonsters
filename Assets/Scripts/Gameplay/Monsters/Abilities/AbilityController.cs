using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    public class AbilityController
    {
        private Monster _monster;
        private AbilityData _passive;
        private AbilityData _active;
        private AbilityData _ultimate;

        public AbilityData Passive { get => _passive; }
        public AbilityData Active { get => _active; }
        public AbilityData Ultimate { get => _ultimate; }

        public AbilityController(Monster monster, MonsterData data)
        {
            _monster = monster;

            if (data.Passive)
            {
                _passive = Object.Instantiate(data.Passive);
                _passive.Behaviour?.Initialize(_passive, _monster);
            }

            if (data.Active)
            {
                _active = Object.Instantiate(data.Active);
                _active.Behaviour?.Initialize(_active, _monster);
            }

            if (data.Ultimate)
            {
                _ultimate = Object.Instantiate(data.Ultimate);
                _ultimate.Behaviour?.Initialize(_ultimate, _monster);
            }
        }

        public void OnTurnStarted()
        {
            TryEvaluateTriggerCondition(AbilityTrigger.OnTurnBegin);
        }

        public void OnTurnPassed()
        {
            _passive?.Behaviour?.TurnPassed();
            _active?.Behaviour?.TurnPassed();
            _ultimate?.Behaviour?.TurnPassed();

            TryEvaluateTriggerCondition(AbilityTrigger.OnTurnEnd);
        }

        public bool TryEvaluateTriggerCondition(AbilityTrigger trigger)
        {
            bool passive = _passive ? _passive.Behaviour?.TryEvaluateTriggerCondition(trigger) ?? false : false;
            bool active = _active ? _active.Behaviour?.TryEvaluateTriggerCondition(trigger) ?? false : false;
            bool ultimate = _ultimate ? _ultimate.Behaviour?.TryEvaluateTriggerCondition(trigger) ?? false : false;

            return passive || active || ultimate;
        }
    }
}
