using GameServer.Ai;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Entity;

namespace GameServer.Model
{
    public class Npc : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        private AiBase? _ai;
        private Random _random = new();

        public Vector3 InitPos;

        public Npc(int entityId, int unitId, 
            Map map, string name, Vector3 initPos) : base(EntityType.Npc, entityId, unitId, map, name)
        {
            InitPos = initPos;
        }

        public override void Start()
        {
            base.Start();

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

    } 
}
