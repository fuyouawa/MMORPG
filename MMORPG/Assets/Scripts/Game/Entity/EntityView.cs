using System;
using MMORPG.System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Game
{
    public enum EntityType
    {
        Player,
        Monster,
        NPC
    }

    public sealed class EntityView : MonoBehaviour, IController
    {
        [ShowInInspector]
        [ReadOnly]
        public int EntityId { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public bool IsMine { get; private set; }

        public EntityType EntityType;

        public event Action<EntityTransformSyncData> OnTransformSync;

        private bool _initialized = false;

        public void Initialize(int entityId, EntityType type, bool isMine)
        {
            Debug.Assert(!_initialized);
            if (type != EntityType)
            {
                throw new Exception("EntityType与当前预制体的Type不相同!");
            }
            _initialized = true;
            EntityId = entityId;
            EntityType = type;
            IsMine = isMine;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        public void HandleNetworkSync(EntityTransformSyncData data)
        {
            Debug.Assert(data.Entity == this);
            OnTransformSync?.Invoke(data);
        }
    }

}
