using QFramework;
using System.Collections.Generic;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.Model;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.System
{
    public interface IPlayerManagerSystem : ISystem
    {
        public int MineId { get; }
        public int CharacterId { get; }
        public Dictionary<int, EntityView> PlayerDict { get; }
        public void SetMineId(int entityId);
    }

    public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem
    {
        public Dictionary<int, EntityView> PlayerDict { get; } = new();

        public int CharacterId { get; private set; } = -1;
        public int MineId { get; private set; } = -1;


        public void SetMineId(int entityId)
        {
            MineId = entityId;
        }


        protected override void OnInit()
        {
            this.RegisterEvent<EntityEnterEvent>(OnEntityEnter);
            this.RegisterEvent<EntityLeaveEvent>(OnEntityLeave);
            this.RegisterEvent<ExitedMapEvent>(OnExitedMap);
        }

        private void OnEntityLeave(EntityLeaveEvent e)
        {
            if (e.Entity.EntityType == EntityType.Player)
            {
                var suc= PlayerDict.Remove(e.Entity.EntityId);
                Debug.Assert(suc);
            }
        }

        private void OnExitedMap(ExitedMapEvent e)
        {
            Clear();
        }

        private void OnEntityEnter(EntityEnterEvent e)
        {
            Debug.Assert(!PlayerDict.ContainsKey(e.Entity.EntityId));
            if (e.Entity.EntityType == EntityType.Player)
            {
                PlayerDict[e.Entity.EntityId] = e.Entity;
            }
        }

        private void Clear()
        {
            MineId = -1;
            CharacterId = -1;
            PlayerDict.Clear();
        }
    }
}
