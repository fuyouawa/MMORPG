using GameServer.AiSystem;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Entity;
using GameServer.MapSystem;
using GameServer.EntitySystem;
using GameServer.Manager;

namespace GameServer.NpcSystem
{
    public class Npc : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        private AiBase? _ai;
        private Random _random = new();

        public NpcDefine NpcDefine;
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

            foreach (var npcDefine in DataManager.Instance.NpcDict.Values)
            {
                if (npcDefine.UnitId == UnitId)
                {
                    NpcDefine = npcDefine;
                    break;
                }
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
