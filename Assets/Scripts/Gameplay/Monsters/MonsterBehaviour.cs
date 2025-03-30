namespace ChessMonsterTactics.Gameplay
{
    [System.Serializable]
    public abstract class MonsterBehaviour
    {
        private Monster _monster;

        public virtual void Init(Monster monster)
        {
            _monster = monster;
        }
    }
}
