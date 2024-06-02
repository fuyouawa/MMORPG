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

        public EntityType EntityType;

        public event Action<EntityTransformSyncData> OnTransformSync;

        private bool _initialized = false;

        public void Initialize(int entityId)
        {
            Debug.Assert(!_initialized);
            _initialized = true;
            EntityId = entityId;
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

#if UNITY_EDITOR
        [Button]
        private void BuildHealth()
        {
            var health = gameObject.GetOrAddComponent<Health>();
            health.Entity = this;
            var point = new GameObject("Damage Number Point");
            point.transform.SetParent(transform, false);
            point.transform.localPosition = Vector3.up;
            health.DamageNumberPoint = point.transform;
        }
#endif
    }

}
