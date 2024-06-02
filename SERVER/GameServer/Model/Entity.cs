using Common.Proto.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Aoi;

namespace GameServer.Model
{
    public abstract class Entity
    {
        public EntityType EntityType;
        public int EntityId;
        public int UnitId;
        public bool Valid = true;
        public Map Map;
        public AoiWord.AoiEntity? AoiEntity;
        public Vector3 Position;
        public Vector3 Direction;

        protected Entity(EntityType entityType, int entityId, int unitId,
            Map map)
        {
            EntityType = entityType;
            EntityId = entityId;
            UnitId = unitId;
            Map = map;
        }

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual bool IsValid()
        {
            return Valid;
        }
    }
}
