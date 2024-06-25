using GameServer.AiSystem;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Entity;
using GameServer.MapSystem;
using GameServer.EntitySystem;
using GameServer.Manager;

namespace GameServer.MonsterSystem
{
    public class Monster : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        private AiBase? _ai;
        private Random _random = new();
        
        public Vector2 InitPos { get; }
        public SpawnDefine SpawnDefine { get; }

        public Monster(SpawnDefine spawnDefine, int entityId, UnitDefine unitDefine, Map map, Vector3 pos, Vector3 dire, string name, int level) 
            : base(EntityType.Monster, entityId, unitDefine, map, pos, dire, name, level)
        {
            SpawnDefine = spawnDefine;
            InitPos = pos.ToVector2();
        }

        public override void Start()
        {
            base.Start();

            Speed = UnitDefine.Speed;
            Hp = AttributeManager.Final.MaxHp;
            Mp = AttributeManager.Final.MaxMp;

            switch (UnitDefine.Ai)
            {
                case "Monster":
                    _ai = new MonsterAi(this);
                    break;
            }

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
