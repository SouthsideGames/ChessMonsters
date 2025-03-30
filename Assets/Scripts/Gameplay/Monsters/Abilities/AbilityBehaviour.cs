using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public abstract class AbilityBehaviour
    {
        public Monster Monster { get; private set; }
        public AbilityData Data { get; private set; }

        protected int LastTriggered { get; private set; }

        public int Cooldown { get => Data.Cooldown; }
        public int RemainingCooldown { get; private set; }

        public void Initialize(AbilityData data, Monster monster) 
        {
            Data = data;
            Monster = monster;
            OnInit();
        }

        public bool TryEvaluateTriggerCondition(AbilityTrigger trigger)
        {
            if (trigger != Data.AbilityTrigger)
                return false;
            
            if (EvaluateTriggerCondition() && RemainingCooldown <= 0)
            {
                TriggerAbility();
                return true;
            }

            return false;
        }

        public void TriggerAbility()
        {
            LastTriggered = Monster.TurnPassed;
            OnTrigger();
        }

        public void TurnPassed()
        {
            RemainingCooldown = Mathf.Clamp(Data.Cooldown - (Monster.TurnPassed - LastTriggered), 0, int.MaxValue);
            OnTurnPassed();
        }

        protected virtual void OnInit() 
        { 

        }

        protected virtual void OnTurnPassed() 
        { 

        }

        protected abstract void OnTrigger();
        public abstract bool EvaluateTriggerCondition();
        public abstract AbilityBehaviour Copy();
    }
}
