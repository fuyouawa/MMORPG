namespace MMORPG.Event
{
    public class JoinedMapEvent
    {
        public int MapId { get; }
        public long CharacterId { get; }

        public JoinedMapEvent(int mapId, long characterId)
        {
            MapId = mapId;
            CharacterId = characterId;
        }
    }
}
