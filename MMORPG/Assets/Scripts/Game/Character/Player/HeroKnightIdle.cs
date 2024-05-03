public class HeroKnightIdle : PlayerAction
{
    [StateCondition]
    public bool DoWalking()
    {
        return Brain.InputControls.Player.Move.IsPressed();
    }
}
