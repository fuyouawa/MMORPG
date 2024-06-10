using MMORPG.Game;
using UnityEngine;

namespace MMORPG.Game
{
    public abstract class CastTarget
    {
        public virtual Vector3 Position => Vector3.zero;
        public virtual Quaternion Rotation => Quaternion.identity;
    }

    public class CastTargetEntity : CastTarget
    {
        public EntityView Entity;

        public CastTargetEntity(EntityView entity)
        {
            Entity = entity;
        }

        public override Vector3 Position => Entity.transform.position;
        public override Quaternion Rotation => Entity.transform.rotation;
    }

    public class CastTargetPosition : CastTarget
    {
        private Vector3 _position;
        public override Vector3 Position => _position;

        public CastTargetPosition(Vector3 position)
        {
            _position = position;
        }

    }
}
