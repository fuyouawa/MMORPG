using GameServer.AiSystem;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Entity;
using GameServer.MapSystem;
using GameServer.EntitySystem;

namespace GameServer.MonsterSystem
{
    public class Monster : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        private AiBase? _ai;
        private Random _random = new();

        public Vector3 InitPos;

        public Monster(int entityId, int unitId, Map map, string name, Vector3 pos, Vector3 dire) 
            : base(EntityType.Monster, entityId, unitId, map, pos, dire, name)
        {
            InitPos = pos;
        }

        public override void Start()
        {
            base.Start();

            switch (DataHelper.GetUnitDefine(UnitId).Ai)
            {
                case "Monster":
                    _ai = new MonsterAi(this);
                    break;
            }

            var unitDefine = DataHelper.GetUnitDefine(UnitId);
            Speed = unitDefine.Speed;
            Hp = AttributeManager.Final.MaxHp;
            Mp = AttributeManager.Final.MaxMp;

            _ai?.Start();
        }

        public override void Update()
        {
            base.Update();
            _ai?.Update();
        }

        public override void Revive()
        {
            if (_ai is MonsterAi)
            {
                var ai = (MonsterAi)_ai;
                ai.AbilityManager.Revive();
            }
        }
    }
}
