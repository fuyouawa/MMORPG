public class LocalHeroKnightIdle : LocalPlayerAbility
{
    public override void OnStateEnter()
    {
        Brain.CharacterController.NetworkUploadTransform(OwnerStateId, null);
    }

    [StateCondition]
    public bool CanMovement()
    {
        return true;
    }
}
