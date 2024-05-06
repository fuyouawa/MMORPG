using System;
using Common.Proto.Event;
using MMORPG;
using QFramework;
using System.Collections;
using Sirenix.OdinInspector;
using Tool;
using UnityEditor;
using UnityEngine;


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

        if (character.Animator != null)
        {
            if (!character.Animator.gameObject.TryGetComponent(out character.AnimationController))
            {
                character.AnimationController =
                    character.Animator.gameObject.AddComponent<CharacterAnimationController>();
            }
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
        OnTransformSync?.Invoke(data);
    }
}
