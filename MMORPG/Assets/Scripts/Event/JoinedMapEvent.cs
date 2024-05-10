namespace MMORPG.Event
{
    public class JoinedMapEvent
    {
        public int MapId { get; }

        public JoinedMapEvent(int mapId)
        {
            MapId = mapId;
        }
    }
}
