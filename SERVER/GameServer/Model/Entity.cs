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
    public class Entity
    {
        public EntityType EntityType;
        public int UnitId;
        public int EntityId;
        public bool Vaild = true;
        public Map? Map;
        public AoiWord.AoiEntity? AoiEntity;
        public Vector3 Position;
        public Vector3 Direction;

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual bool IsValid()
        {
            return Vaild;
        }
    }
}
