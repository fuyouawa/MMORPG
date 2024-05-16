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
        Player
    }

    public sealed class EntityView : MonoBehaviour, IController
    {
        [ShowInInspector]
        [ReadOnly]
        public int EntityId { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public EntityType EntityType { get; private set; }

        [ShowInInspector]
        [ReadOnly]
        public bool IsMine { get; private set; }

        public event Action<EntityTransformSyncData> OnTransformSync;

        private bool _initialized = false;

        public void Initialize(int entityId, EntityType type, bool isMine)
        {
            Debug.Assert(!_initialized);
            _initialized = true;
            EntityId = entityId;
            EntityType = type;
            IsMine = isMine;
        }


#if UNITY_EDITOR
        [Button]
        public void BuildCharacter()
        {
            if (gameObject.TryGetComponent(out CharacterController _))
            {
                EditorUtility.DisplayDialog("提示", "已经有一个Character了", "确认");
                return;
            }
            var character = gameObject.AddComponent<CharacterController>();
            character.Entity = this;
            if (!gameObject.TryGetComponent(out character.Animator))
            {
                character.Animator = gameObject.GetComponentInChildren<Animator>();
            }
        }
#endif

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
