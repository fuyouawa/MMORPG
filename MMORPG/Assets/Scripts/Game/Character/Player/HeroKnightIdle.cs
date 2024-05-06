public class HeroKnightIdle : PlayerAbility
{
    public override void OnStateEnter()
    {
        if (IsMine)
        {
            Brain.CharacterController.NetworkUploadTransform(OwnerStateId, null);
        }
    }

    [StateCondition]
    public bool CanMovement()
    {
        return true;
    }
}
