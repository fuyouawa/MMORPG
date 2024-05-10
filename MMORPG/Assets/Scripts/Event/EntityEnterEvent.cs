using MMORPG.Game;

namespace MMORPG.Event
{
    public class EntityEnterEvent
    {
        public EntityView Entity { get; }

        public EntityEnterEvent(EntityView entity)
        {
            Entity = entity;
        }
    }
}
