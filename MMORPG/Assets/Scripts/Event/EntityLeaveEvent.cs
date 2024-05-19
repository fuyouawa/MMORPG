using MMORPG.Game;

namespace MMORPG.Event
{
    public class EntityLeaveEvent
    {
        public EntityView Entity { get; }

        public EntityLeaveEvent(EntityView entity)
        {
            Entity = entity;
        }
    }
}
