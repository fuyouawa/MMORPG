namespace MMORPG.Event
{
    public class WannaJoinMapEvent
    {
        public int MapId { get; }
        public long CharacterId { get; }

        public WannaJoinMapEvent(int mapId, long characterId)
        {
            MapId = mapId;
            CharacterId = characterId;
        }
    }
}
