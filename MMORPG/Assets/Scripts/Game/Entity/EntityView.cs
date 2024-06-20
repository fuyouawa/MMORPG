using System;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public sealed class EntityView : MonoBehaviour, IController
    {
        [ShowInInspector]
        [ReadOnly]
        public int EntityId { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        private int _unitId => UnitDefine?.ID ?? 0;
        public UnitDefine UnitDefine { get; private set; }

        public EntityType EntityType;

        public Action<EntityTransformSyncData> OnTransformSync;

        private bool _initialized = false;


        public void Initialize(int entityId, UnitDefine unitDefine)
        {
            Debug.Assert(!_initialized);
            _initialized = true;

            EntityId = entityId;
            UnitDefine = unitDefine;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}
