using UnityEngine;

namespace MMORPG.Game
{
    public class RemotePlayerAbility : PlayerAbility
    {
        public virtual void OnStateNetworkSyncTransform(EntityTransformSyncData data)
        {
            Debug.Log($"{GetType().Name}同步:{data.Position}, {data.Rotation}");
            Brain.ActorController.SmoothMove(data.Position);
            Brain.ActorController.SmoothRotate(data.Rotation);
        }
    }
    
}
