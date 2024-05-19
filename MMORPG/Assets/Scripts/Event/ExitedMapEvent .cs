namespace MMORPG.Event
{
    public class ExitedMapEvent
    {
        public int MapId { get; }

        public ExitedMapEvent(int mapId)
        {
            MapId = mapId;
        }
    }
}
