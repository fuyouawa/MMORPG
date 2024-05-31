namespace MMORPG.Event
{
    public class JoinedMapEvent
    {
        public int MapId { get; }
        public int CharacterId { get; }

        public JoinedMapEvent(int mapId, int characterId)
        {
            MapId = mapId;
            CharacterId = characterId;
        }
    }
}
