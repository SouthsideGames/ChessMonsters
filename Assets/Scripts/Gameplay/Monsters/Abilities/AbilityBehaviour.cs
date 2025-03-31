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

            if (RemainingCooldown > 0 || !EvaluateTriggerCondition())
                return false;

            // Handle status effect-based blocks
            if (Data.AbilityType == AbilityType.Active)
            {
                foreach (var effect in Monster.GetActiveStatusEffects())
                {
                    if (effect is SilencedEffect s && s.PreventsActive())
                    {
                        Debug.Log($"{Monster.Name} is silenced and cannot use active abilities.");
                        return false;
                    }
                }
            }
            else if (Data.AbilityType == AbilityType.Ultimate)
            {
                foreach (var effect in Monster.GetActiveStatusEffects())
                {
                    if (effect is SlowedEffect s && s.PreventsUltimate())
                    {
                        Debug.Log($"{Monster.Name} is slowed and cannot use ultimate abilities.");
                        return false;
                    }
                }
            }
            else if (Data.AbilityType == AbilityType.Passive)
            {
                foreach (var effect in Monster.GetActiveStatusEffects())
                {
                    if (effect is SuppressedEffect)
                    {
                        Debug.Log($"{Monster.Name}'s passive is suppressed and won't trigger.");
                        return false;
                    }
                }
            }

            TriggerAbility();
            return true;
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
