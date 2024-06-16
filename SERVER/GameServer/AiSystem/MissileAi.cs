using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Monster;
using GameServer.PlayerSystem;
using GameServer.MonsterSystem;
using GameServer.EntitySystem;
using GameServer.AiSystem.Ability;
using GameServer.FightSystem;
using GameServer.MissileSystem;
using System.Linq;

namespace GameServer.AiSystem
{
    public class MissileAbilityManager
    {
        public Missile Missile;
        public MoveAbility MoveAbility;

        private CastTarget _chasingTarget;
        private float _attackRange = 1f;
        private Action<Entity> _hitCallback;
        public HashSet<int> _hitEntitySet;
        
        public MissileAbilityManager(Missile missile, CastTarget chasingTarget, Action<Entity> hitCallback)
        {
            Missile = missile;
            MoveAbility = new(missile, missile.InitPos.Y, missile.Speed);
            _chasingTarget = chasingTarget;
            _hitCallback = hitCallback;
        }

        public void Start()
        {
            MoveAbility.Start();
        }

        public void Update()
        {
            if (!_chasingTarget.Selectable) return;
            MoveAbility.Move(_chasingTarget.Position);
            MoveAbility.Update();

            Missile.Map.ScanEntityFollower(Missile, entity =>
            {
                var distance = Vector2.Distance(entity.Position.ToVector2(), Missile.Position.ToVector2());
                if (distance > _attackRange) return;
                if (_hitEntitySet.Contains(entity.EntityId)) return;
                _hitEntitySet.Add(entity.EntityId);
                _hitCallback(entity);
            });
        }

        private void UpdateSyncState()
        {
            var res = new EntityTransformSyncResponse()
            {
                EntityId = Missile.EntityId,
                Transform = ProtoHelper.ToNetTransform(Missile.Position, Missile.Direction)
            };
            Missile.Map.PlayerManager.Broadcast(res, Missile);
        }
    }

    public class MissileAi : AiBase
    {
        public MissileAbilityManager AbilityManager;

        public MissileAi(Missile missile, CastTarget chasingTarget, Action<Entity> hitCallback)
        {
            AbilityManager = new MissileAbilityManager(missile, chasingTarget, hitCallback);
        }

        public override void Start()
        {
            AbilityManager.Start();
        }

        public override void Update()
        {
            AbilityManager.Update();
        }
    }
}
