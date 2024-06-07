using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Proto.Entity;
using Common.Proto.Fight;
using Google.Protobuf;
using MMORPG.Global;
using MMORPG.System;
using MMORPG.Tool;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;
using QFramework;
using UnityEngine.EventSystems;

namespace MMORPG.Game
{
    public class PlayerBrain : MonoBehaviour, IController
    {
        [Required]
        public CharacterController CharacterController;
        [Required]
        public PlayerAnimationController AnimationController;

#if UNITY_EDITOR
        [SerializeField]
        [ReadOnly]
        [LabelText("CurrentState")]
        private string _currentStateName = "NONE";
#endif

        // public string StartStateName = string.Empty;

        [Information("状态机中有报错还没处理!", InfoMessageType.Error, "CheckStatesHasError")]
        [Information("空状态机是没有意义的!", InfoMessageType.Warning, "IsEmptyStates")]
        [Information("不能有相同名称的状态!", InfoMessageType.Error, "HasRepeatStateName")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Name")]
        public PlayerState[] States;

        public PlayerState CurrentState { get; private set; }

        [Title("Binding")]
        public GameObject[] AdditionalAbilityNodes;

        public GameInputControls InputControls { get; private set; }

        public LocalPlayerAbility[] GetAttachLocalAbilities() => GetAttachAbilities<LocalPlayerAbility>();

        public RemotePlayerAbility[] GetAttachRemoteAbilities() => GetAttachAbilities<RemotePlayerAbility>();

        public Vector2 GetMoveInput() => InputControls.Player.Move.ReadValue<Vector2>();
        public bool IsPressingRun() => InputControls.Player.Run.inProgress;

        private bool? _isMine = null;

        [ShowInInspector]
        [ReadOnly]
        public bool IsMine
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                    return false;
#endif
                if (_isMine == null)
                {
                    var mine = this.GetSystem<IPlayerManagerSystem>().MineEntity;
                    if (mine == null)
                        throw new Exception("Player还未初始化!");
                    _isMine = mine.EntityId == CharacterController.Entity.EntityId;
                }

                return _isMine == true;
            }
        }

        private INetworkSystem _newtwork;

        private TAbility[] GetAttachAbilities<TAbility>() where TAbility : PlayerAbility
        {
            var total = new List<TAbility>();
            total.AddRange(GetComponents<TAbility>());
            foreach (var node in AdditionalAbilityNodes)
            {
                total.AddRange(node.GetComponents<TAbility>());
            }
            return total.ToArray();
        }

        public bool ChangeState(PlayerState state)
        {
            Debug.Assert(state != null);
            Debug.Assert(States.Contains(state));
            CurrentState?.Exit();
            CurrentState = state;
#if UNITY_EDITOR
            _currentStateName = CurrentState.Name;
#endif
            CurrentState.Enter();
            return true;
        }

        public PlayerState GetState(string stateName)
        {
            return Array.Find(States, x => x.Name == stateName);
        }

        public void NetworkUploadTransform(int stateId, byte[] data)
        {
            _newtwork.SendToServer(new EntityTransformSyncRequest()
            {
                EntityId = CharacterController.Entity.EntityId,
                Transform = new()
                {
                    Direction = transform.rotation.eulerAngles.ToNetVector3(),
                    Position = transform.position.ToNetVector3()
                },
                StateId = stateId,
                Data = data == null ? ByteString.Empty : ByteString.CopyFrom(data)
            });
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (CheckStatesHasError())
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "Player的PlayerBrain中的状态机有错误还未处理!", "确定");
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
#endif
            _newtwork = this.GetSystem<INetworkSystem>();
            AnimationController.Setup(this);
            CurrentState = null;
            if (States.Length == 0) return;
            CharacterController.Entity.OnTransformSync += OnTransformEntitySync;
        }

        private void OnTransformEntitySync(EntityTransformSyncData data)
        {
            Debug.Assert(!IsMine);
            var state = States[data.StateId];
            Debug.Assert(state != null);
            if (state != CurrentState)
            {
                ChangeState(state);
            }

            foreach (var action in state.Actions)
            {
                action.TransformEntitySync(data);
            }
        }

        private void Start()
        {
            if (IsMine)
            {
                InputControls = new();
                InputControls.Enable();
            }
            else
            {
                Destroy(CharacterController.Rigidbody);
                Destroy(CharacterController.Collider);
            }
            if (States.IsNullOrEmpty()) return;
            InitStates();
            ChangeState(States[0]);
            StartCoroutine(NetworkFixedUpdate());
        }

        private bool _prepareFire = false;

        private void Update()
        {
            if (States.IsNullOrEmpty()) return;
            CurrentState?.Update();
        }

        private void FixedUpdate()
        {
            if (States.IsNullOrEmpty()) return;
            CurrentState?.FixedUpdate();
        }


        private IEnumerator NetworkFixedUpdate()
        {
            while (true)
            {
                CurrentState?.NetworkFixedUpdate();
                yield return new WaitForSeconds(Config.NetworkUpdateDeltaTime);
            }
        }

        private void OnEnable()
        {
            InputControls?.Enable();
        }

        private void OnDisable()
        {
            InputControls?.Disable();
        }

        private void InitStates()
        {
            States.ForEach((x, i) =>
            {
                x.Setup(this, i);
                x.Initialize();
                x.OnTransitionEvaluated += (transition, condition) =>
                {
                    ChangeState(condition ? transition.TrueState : transition.FalseState);
                };
            });
        }


#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            foreach (var state in States)
            {
                state.Brain = this;
            }
        }

        private bool HasRepeatStateName => States.GroupBy(x => x.Name).Any(g => g.Count() > 1);

        private bool IsEmptyStates => States.Length == 0;

        private bool CheckStatesHasError()
        {
            return States.Any(x => x.HasError());
        }
#endif
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }

}
