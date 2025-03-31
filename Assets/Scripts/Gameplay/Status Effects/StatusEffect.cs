using UnityEngine;

namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public abstract class StatusEffect
    {
        public string Name;
        public int Duration;

        public abstract void OnApply(Monster target);
        public abstract void OnExpire(Monster target);
        public abstract void OnTurnPassed(Monster target);
    }
}
