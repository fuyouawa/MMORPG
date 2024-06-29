using System.Collections;
using MMORPG.Common.Proto.Entity;
using MMORPG.Event;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class ActorController : MonoBehaviour, IController, ICanSendEvent
    {
        [Required]
        public EntityView Entity;
        public float RotationSmooth = 10f;
        public float MoveSmooth = 10f;
        [Title("Binding")]
        [Required]
        public Animator Animator;
        public SkillsEffectManager EffectManager;

        [Title("Properties")]
        [ShowInInspector]
        [ReadOnly]
        public int Level { get; set; }
        [ShowInInspector]
        [ReadOnly]
        public int Exp { get; set; }
        [ShowInInspector]
        [ReadOnly]
        public int Gold { get; set; }
        [ShowInInspector]
        [ReadOnly]
        public int Hp { get; set; }
        [ShowInInspector]
        [ReadOnly]
        public int Mp { get; set; }
        [ShowInInspector]
        [ReadOnly]
        public int MaxHp { get; set; }

        [Title("Hit Effects")]
        public ParticleSystem HitParticlePrefab;
        public AudioSource HitAudio;
        public ParticleSystem CritHitParticlePrefab;
        public AudioSource CritHitAudio;
        public ParticleSystem MissHitParticlePrefab;
        public AudioSource MissHitAudio;
        public float HitParticleDuration = 1f;
        public float HitAudioDuration = 1f;

        [Title("Hurt Feedback")]
        public FeedbacksManager HurtFeedbacks;
        public FeedbacksManager CritHurtFeedbacks;
        public FeedbacksManager MissHurtFeedbacks;

        [Title("Hurt Property")]
        [Required]
        public Transform HurtPoint;

        public void ApplyNetActor(NetActor netActor)
        {
            if (netActor != null)
            {
                Level = netActor.Level;
                Hp = netActor.Hp;
                Mp = netActor.Mp;
                MaxHp = netActor.MaxHp;
            }
        }

        public CharacterSkillManager SkillManager { get; private set; }

        public bool IsPreventingMovement { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public CapsuleCollider Collider { get; private set; }

        private IEntityManagerSystem _entityManager;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<CapsuleCollider>();
            _entityManager = this.GetSystem<IEntityManagerSystem>();

            SkillManager = new(this);

            Entity.OnHit += info =>
            {
                if (_entityManager.EntityDict.TryGetValue(info.TargetId, out var wounded))
                {
                    var woundedActor = wounded.GetComponentInChildren<ActorController>();
                    if (woundedActor == null) return;

                    if (info.IsMiss)
                    {
                        PlayHitEffects(woundedActor, MissHitParticlePrefab, MissHitAudio);
                    }
                    else if (info.IsCrit)
                    {
                        PlayHitEffects(woundedActor, CritHitParticlePrefab, CritHitAudio);
                    }
                    else
                    {
                        PlayHitEffects(woundedActor, HitParticlePrefab, HitAudio);
                    }
                }
            };

            Entity.OnHurt += info =>
            {
                if (info.IsMiss)
                {
                    MissHurtFeedbacks?.Play();
                }
                else if (info.IsCrit)
                {
                    CritHurtFeedbacks?.Play();
                }
                else
                {
                    HurtFeedbacks?.Play();
                }
            };
        }

        private void PlayHitEffects(ActorController actor, ParticleSystem particle, AudioSource audio)
        {
            if (particle != null)
            {
                var p = Instantiate(particle, actor.HurtPoint);
                p.gameObject.AddComponent<LifeTime>().Run(HitParticleDuration);
                p.Play();
            }

            if (audio != null)
            {
                var a = Instantiate(audio, actor.HurtPoint);
                a.gameObject.AddComponent<LifeTime>().Run(HitAudioDuration);
                a.PlayWithChildren();
            }
        }

        public void Initialize()
        {
            SkillManager.Initialize();
        }

        private void Update()
        {
            SkillManager.Update();
        }

        private Coroutine _smoothMoveCoroutine;
        public void SmoothMove(Vector3 position)
        {
            if (_smoothMoveCoroutine != null)
                StopCoroutine(_smoothMoveCoroutine);
            _smoothMoveCoroutine = StartCoroutine(SmoothMoveCo(position));
        }

        private IEnumerator SmoothMoveCo(Vector3 position)
        {
            while (!MathHelper.Approximately(transform.position, position))
            {
                if (IsPreventingMovement)
                {
                    yield break;
                }
                transform.position = Vector3.Lerp(transform.position, position, MoveSmooth * Time.deltaTime);
                yield return null;
            }
        }

        private Coroutine _smoothRotateCoroutine;
        public void SmoothRotate(Quaternion rotation)
        {
            if (_smoothRotateCoroutine != null)
                StopCoroutine(_smoothRotateCoroutine);
            _smoothRotateCoroutine = StartCoroutine(SmoothRotateCo(rotation));
        }

        private IEnumerator SmoothRotateCo(Quaternion rotation)
        {
            while (!MathHelper.Approximately(transform.rotation, rotation))
            {
                if (IsPreventingMovement)
                {
                    yield break;
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, RotationSmooth * Time.deltaTime);
                yield return null;
            }
        }

        public void MoveDirection(Vector3 direction)
        {
            Move(transform.position + direction);
        }

        public void RelativeRotate(Quaternion rotation)
        {
            Rotate(transform.rotation * rotation);
        }

        public void Move(Vector3 position)
        {
            if (IsPreventingMovement) return;
            transform.position = position;
        }

        public void Rotate(Quaternion rotation)
        {
            if (IsPreventingMovement) return;
            transform.rotation = rotation;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        protected Coroutine PreventMovementCoroutine;

        public void PreventMovement(float delay = float.MaxValue)
        {
            if (PreventMovementCoroutine != null)
                StopCoroutine(PreventMovementCoroutine);
            PreventMovementCoroutine = StartCoroutine(PreventMovementCo(delay));
        }

        public void StopPreventMovement()
        {
            if (PreventMovementCoroutine != null)
                StopCoroutine(PreventMovementCoroutine);
            IsPreventingMovement = false;
        }

        protected virtual IEnumerator PreventMovementCo(float delay)
        {
            IsPreventingMovement = true;
            yield return new WaitForSeconds(delay);
            IsPreventingMovement = false;
        }

        //private void OnGUI()
        //{
        //    if (!IsInView(gameObject))
        //    {
        //        return;
        //    }

        //    float height = 2f;
        //    var camera = Camera.main;
        //    var pos = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        //    Vector2 uiPos = camera.WorldToScreenPoint(pos);
        //    uiPos = new(uiPos.x, Screen.height - uiPos.y);

        //    Vector2 nameSize = GUI.skin.label.CalcSize(new("帅比"));
        //    GUI.color = Color.yellow;

        //    var rect = new Rect(uiPos.x - (nameSize.x / 2), uiPos.y - nameSize.y, nameSize.x, nameSize.y);
        //    GUI.Label(rect, "帅比");

        //}

        //// 判断物体是否处于摄像机拍摄的角度中
        //private bool IsInView(GameObject go)
        //{
        //    var worldPos = go.transform.position;
        //    var cameraTransform = Camera.main.transform;
        //    Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        //    Vector2 dir = (worldPos - cameraTransform.position).normalized;
        //    float dot = Vector3.Dot(cameraTransform.forward, dir);
        //    return dot > 0 &&
        //           viewPos.x >= 0 &&
        //           viewPos.x <= 1 &&
        //           viewPos.y >= 0 &&
        //           viewPos.y <= 1;
        //}
    }

}
