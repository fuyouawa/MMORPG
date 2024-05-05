public class HeroKnightIdle : PlayerAbility
{
    [StateCondition]
    public bool CanMovement()
    {
        return true;
    }
}
