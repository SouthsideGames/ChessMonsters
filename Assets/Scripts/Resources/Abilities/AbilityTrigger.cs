namespace ChessMonsterTactics
{
    public enum AbilityTrigger
    {
        OnTurnBegin,        // Ability trigger condition will be evaluated on the beginning of a turn 
        OnTurnEnd,          // Ability trigger condition will be evaluated on the end of a turn
        OnActivate,         // Ability trigger condition will be evaluated on the manual trigger (e.g. when pressed on a ability button)
        OnAttack,           // Ability trigger condition will be evaluated when performing an attack
        OnReceiveDamage     // Ability trigger condition will be evaluated when receiving damage
    }
}
