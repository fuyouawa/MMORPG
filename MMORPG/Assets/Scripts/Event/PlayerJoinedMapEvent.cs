using MMORPG.Game;

namespace MMORPG.Event
{
    public class PlayerJoinedMapEvent
    {
        public EntityView PlayerEntity { get; }

        public PlayerJoinedMapEvent(EntityView playerEntity)
        {
            PlayerEntity = playerEntity;
        }
    }
}
