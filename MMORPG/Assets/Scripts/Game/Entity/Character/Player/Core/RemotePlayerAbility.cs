namespace MMORPG.Game
{
    public class RemotePlayerAbility : PlayerAbility
    {
        public virtual void OnStateNetworkSyncTransform(EntityTransformSyncData data)
        {
            Brain.CharacterController.SmoothMove(data.Position);
            Brain.CharacterController.SmoothRotate(data.Rotation);
        }
    }
    
}
