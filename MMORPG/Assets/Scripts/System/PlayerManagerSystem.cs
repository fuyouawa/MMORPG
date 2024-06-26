using System;
using QFramework;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Entity;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.Model;
using UnityEngine;

namespace MMORPG.System
{
    public interface IPlayerManagerSystem : ISystem
    {
        public EntityView MineEntity { get; }
        public int CharacterId { get; }
        public Dictionary<int, EntityView> PlayerDict { get; }
        public void SetMine(EntityView mineEntity);

        public Task<EntityView> GetMineEntityTask();
    }

    public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem
    {
        public Dictionary<int, EntityView> PlayerDict { get; } = new();

        public int CharacterId { get; private set; } = -1;
        public EntityView MineEntity { get; private set; }

        private TaskCompletionSource<EntityView> _mineEntityTcs = new();

        public void SetMine(EntityView mineEntity)
        {
            MineEntity = mineEntity;
            _mineEntityTcs.TrySetResult(mineEntity);
        }

        public Task<EntityView> GetMineEntityTask()
        {
            if (MineEntity != null)
            {
                return Task.FromResult(MineEntity);
            }

            return _mineEntityTcs.Task;
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
            MineEntity = null;
            CharacterId = -1;
            PlayerDict.Clear();
        }

        protected override void OnDeinit()
        {
            Clear();
        }
    }
}
