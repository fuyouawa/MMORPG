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
        public float RotationSmooth = 10f;
        public float MoveSmooth = 10f;
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

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;

        private bool _needMove = false;
        private bool _needRotate = false;

        private void Awake()
        {
            _newtwork = this.GetSystem<INetworkSystem>();
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<CapsuleCollider>();
            _targetRotation = transform.rotation;
            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (_needMove)
            {
                if (Vector3.Distance(transform.position, _targetPosition).Abs() > MoveSmooth * Time.deltaTime)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition, MoveSmooth * Time.deltaTime);
                }
                else
                {
                    transform.position = _targetPosition;
                    _needMove = false;
                }
            }

            if (_needRotate)
            {
                if (Quaternion.Dot(transform.rotation, _targetRotation).Abs() < 0.95f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, RotationSmooth * Time.deltaTime);
                }
                else
                {
                    transform.rotation = _targetRotation;
                    _needRotate = false;
                }
            }
        }

        public void SmoothRotate(Quaternion targetRotation)
        {
            _needRotate = true;
            _targetRotation = targetRotation;
        }

        public void SmoothMove(Vector3 position)
        {
            _needMove = true;
            _targetPosition = position;
        }

        public void SmoothMoveDirection(Vector3 direction)
        {
            _targetPosition += direction;
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
