using UnityEngine;

namespace MMORPG.Game
{
    public class RemotePlayerAbility : PlayerAbility
    {
        public virtual void OnStateNetworkSyncTransform(EntityTransformSyncData data)
        {
            Brain.ActorController.SmoothMove(data.Position);
            Brain.ActorController.SmoothRotate(data.Rotation);
        }
    }
    
}
