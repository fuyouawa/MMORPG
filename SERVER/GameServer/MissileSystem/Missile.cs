using GameServer.FightSystem;
using GameServer.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MMORPG.Common.Proto.Entity;
using GameServer.AiSystem;
using GameServer.Tool;
using GameServer.MapSystem;
using GameServer.EntitySystem;

namespace GameServer.MissileSystem
{
    public class Missile : Entity
    {
        public Vector3 InitPos;
        public float Speed;
        
        private AiBase? _ai;
        private float _range;
        private CastTarget _castTarget;
        private Action<Entity> _hitCallback;
       
        public Missile(int entityId, UnitDefine unitDefine, Map map, Vector3 pos, Vector3 dire, 
            float range, float speed, CastTarget castTarget, Action<Entity> hitCallback) 
            : base(EntityType.Missile, entityId, unitDefine, map, pos, dire)
        {
            EntityType = EntityType.Missile;
            InitPos = pos;
            Speed = speed;
            _range = range;
            _castTarget = castTarget;
            _hitCallback = hitCallback;
        }

        public override void Start()
        {
            base.Update();
            switch (UnitDefine.Ai)
            {
                case "Missile":
                    _ai = new MissileAi(this, _range, _castTarget, _hitCallback);
                    break;
            }
            _ai?.Start();
        }

        public override void Update()
        {
            base.Update();
            _ai?.Update();
        }
    }
}
