using System;
using QFramework;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Game
{
    public sealed class EntityView : MonoBehaviour, IController
    {
        [SerializeField]
        [ReadOnly]
        private int _entityId;
        [SerializeField]
        [ReadOnly]
        private bool _isMine;

        public event Action<EntityTransformSyncData> OnTransformSync;

        public int EntityId => _entityId;

        public bool IsMine => _isMine;

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

        public void SetEntityId(int entityId)
        {
            _entityId = entityId;
        }

        public void SetIsMine(bool isMine)
        {
            _isMine = isMine;
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
