using Common.Proto.Event;
using Google.Protobuf;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public enum CharacterType
    {
        Player,
        Enemy
    }

    public class CharacterController : MonoBehaviour, IController
    {
        [Required]
        public EntityView Entity;
        public CharacterType CharacterType;
        public float RotationLerp = 0.2f;
        public float MoveLerp = 0.2f;
        [Title("Binding")]
        [Required]
        public Animator Animator;
        [Title("Action")]
        [ChildGameObjectsOnly]
        public GameObject[] AdditionalAbilityNodes;

        public bool IsMine => Entity.IsMine;

        public Rigidbody Rigidbody { get; private set; }
        public CapsuleCollider Collider { get; private set; }

        private INetworkSystem _newtwork;

        private void Awake()
        {
            _newtwork = this.GetSystem<INetworkSystem>();
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<CapsuleCollider>();
        }

        public void SmoothRotate(Quaternion targetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationLerp);
        }

        public void SmoothMove(Vector3 position)
        {
            transform.position = Vector3.Lerp(transform.position, position, MoveLerp);
        }

        public void NetworkUploadTransform(int stateId, byte[] data)
        {
            Debug.Assert(IsMine);
            _newtwork.SendToServer(new EntityTransformSyncRequest()
            {
                EntityId = Entity.EntityId,
                Transform = new()
                {
                    Direction = transform.rotation.eulerAngles.ToNetVector3(),
                    Position = transform.position.ToNetVector3()
                },
                StateId = stateId,
                Data = data == null ? ByteString.Empty : ByteString.CopyFrom(data)
            });
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}
