namespace ChessMonsterTactics.Gameplay
{
    public class AttackSolver
    {
        /*
        *   NOTE:
        *       If we want more complex calculation or apply some sort of conditional
        *       value, we can do it here
        */

        public virtual AttackData Calculate(Monster attacker, Monster target)
        {
            return new AttackData
            {
                Damage = attacker.AttackPower,
                Defense = target.Defense,
                IsCritical = false
            };
        }
    }
}
