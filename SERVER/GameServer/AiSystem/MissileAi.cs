using MMORPG.Common.Proto.Entity;
using GameServer.Tool;
using System.Numerics;
using MMORPG.Common.Proto.Entity;
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
        public Missile OwnerMissile;
        public MoveAbility MoveAbility;

        private CastTarget _chasingTarget;
        private float _range;
        private Action<Entity> _hitCallback;
        public HashSet<int> _hitEntitySet;
        
        public MissileAbilityManager(Missile ownerMissile, float range, CastTarget chasingTarget, Action<Entity> hitCallback)
        {
            OwnerMissile = ownerMissile;
            MoveAbility = new(OwnerMissile, OwnerMissile.InitPos.Y, OwnerMissile.Speed);
            _range = range;
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

            OwnerMissile.Map.ScanEntityFollower(OwnerMissile, entity =>
            {
                var distance = Vector2.Distance(entity.Position.ToVector2(), OwnerMissile.Position.ToVector2());
                if (distance > _range) return;
                if (_hitEntitySet.Contains(entity.EntityId)) return;
                _hitEntitySet.Add(entity.EntityId);
                _hitCallback(entity);
            });
        }

        private void UpdateSyncState()
        {
            var res = new EntityTransformSyncResponse()
            {
                EntityId = OwnerMissile.EntityId,
                Transform = ProtoHelper.ToNetTransform(OwnerMissile.Position, OwnerMissile.Direction)
            };
            OwnerMissile.Map.PlayerManager.Broadcast(res, OwnerMissile);
        }
    }

    public class MissileAi : AiBase
    {
        public MissileAbilityManager AbilityManager;

        public MissileAi(Missile missile, float range, CastTarget chasingTarget, Action<Entity> hitCallback)
        {
            AbilityManager = new MissileAbilityManager(missile, range, chasingTarget, hitCallback);
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
