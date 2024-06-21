using UnityEngine;

namespace MMORPG.Game
{
    public class RemotePlayerAbility : PlayerAbility
    {
        public virtual void OnStateNetworkSyncTransform(EntityTransformSyncData data)
        {
            Debug.Log($"{GetType().Name}同步:{data.Position}, {data.Rotation}");
            Brain.CharacterController.SmoothMove(data.Position);
            Brain.CharacterController.SmoothRotate(data.Rotation);
        }
    }
    
}
