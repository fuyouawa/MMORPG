
namespace MMORPG.Global
{
    public static class Config
    {
        public static float NetworkUpdateDeltaTime = 0.1f;

        public static string ItemIconPath = "Sprites/Icons/Items";
        public static string SkillIconPath = "Sprites/Icons/Skills";
        public static string CharacterPrefabsPath = "Prefabs/Character";
        public static string MonsterPrefabsPath = $"{CharacterPrefabsPath}/Monster";
        public static string NpcPrefabsPath = $"{CharacterPrefabsPath}/Npc";
        public static string PlayerPrefabsPath = $"{CharacterPrefabsPath}/Player";
        public static string ItemsPrefabsPath = $"Prefabs/Items";
    }
}
