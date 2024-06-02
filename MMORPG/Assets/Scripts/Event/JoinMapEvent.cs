namespace MMORPG.Event
{
    public class JoinMapEvent
    {
        public int MapId { get; }
        public long CharacterId { get; }

        public JoinMapEvent(int mapId, long characterId)
        {
            MapId = mapId;
            CharacterId = characterId;
        }
    }
}
