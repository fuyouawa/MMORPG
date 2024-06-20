using MMORPG.Common.Proto.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Aoi;
using GameServer.MapSystem;

namespace GameServer.EntitySystem
{
    public abstract class Entity
    {
        public EntityType EntityType;
        public int EntityId;
        public UnitDefine UnitDefine;
        public bool Valid = true;
        public Map Map;
        public AoiWord.AoiEntity? AoiEntity;
        public Vector3 Position;
        public Vector3 Direction;

        protected Entity(EntityType entityType, int entityId, UnitDefine unitDefine, Map map, Vector3 pos, Vector3 dire)
        {
            EntityType = entityType;
            EntityId = entityId;
            UnitDefine = unitDefine;
            Map = map;
            Position = pos;
            Direction = dire;
        }

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual bool IsValid()
        {
            return Valid;
        }
    }
}
