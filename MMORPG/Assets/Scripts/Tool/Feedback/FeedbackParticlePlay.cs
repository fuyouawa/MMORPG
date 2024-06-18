using System;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MMORPG.Tool
{
    [FeedbackHelp("这个Feedback用于在Play Feedback时播放场景中指定的ParticleSystem")]
    [AddFeedbackMenu("Particle/ParticlePlay", "播放粒子")]
    public class FeedbackParticlePlay : AbstractFeedback
    {
        public enum Modes { Play, Stop, Pause, Emit }

        [FoldoutGroup("Bound Particles")]
        public Modes Mode = Modes.Play;

        [FoldoutGroup("Bound Particles")]
        [ShowIf("Mode", Modes.Emit)]
        public int EmitCount = 100;

        [FoldoutGroup("Bound Particles")]
        [Required]
        public ParticleSystem BoundParticleSystem;
        [FoldoutGroup("Bound Particles")]
        public bool WithChildrenParticles = true;
        [FoldoutGroup("Bound Particles")]
        public ParticleSystem[] RandomParticleSystems = Array.Empty<ParticleSystem>();
        [FoldoutGroup("Bound Particles")]
        public bool ActivateOnPlay = false;
        [FoldoutGroup("Bound Particles")]
        public bool StopSystemOnInit = true;
        [FoldoutGroup("Bound Particles")]
        public float DeclaredDuration = 0f;

        [FoldoutGroup("Transform")]
        [Tooltip("在Play时改变Parent, 在Stop时还原")]
        public bool ChangeParentOnPlay = false;
        [FoldoutGroup("Transform")]
        [ShowIf("ChangeParentOnPlay")]
        [Tooltip("在Play时要改变到的Parent")]
        public Transform ParentOnPlay;
        [FoldoutGroup("Transform")]
        [ShowIf("ChangeParentOnPlay")]
        [Tooltip("在改变Parent时是否保持世界坐标")]
        public bool WorldPositionStays = true;

        [FoldoutGroup("Simulation Speed")]
        public bool ForceSimulationSpeed = false;
        [FoldoutGroup("Simulation Speed")]
        [ShowIf("ForceSimulationSpeed")]
        public Vector2 ForcedSimulationSpeed = new(0.1f, 1f);

        private ParticleSystem.EmitParams _emitParams;
        private Dictionary<ParticleSystem, Transform> _originalParentRecords = new();

        

        public override float GetDuration()
        {
            return DeclaredDuration;
        }

        protected override void OnFeedbackInit()
        {
            if (StopSystemOnInit)
            {
                StopParticles();
            }

            _originalParentRecords[BoundParticleSystem] = BoundParticleSystem.transform.parent;
            if (RandomParticleSystems != null)
            {
                foreach (var particle in RandomParticleSystems)
                {
                    _originalParentRecords[particle] = particle.transform.parent;
                }
            }
        }

        protected override void OnFeedbackPlay()
        {
            PlayParticles();
        }

        protected override void OnFeedbackStop()
        {
            StopParticles();

            foreach (var record in _originalParentRecords)
            {
                record.Key.transform.SetParent(record.Value, WorldPositionStays);
            }
        }

        private void PlayParticles()
        {
            if (ActivateOnPlay)
            {
                BoundParticleSystem.gameObject.SetActive(true);

                RandomParticleSystems?.ForEach(x => x?.gameObject.SetActive(true));
            }

            if (RandomParticleSystems?.Length > 0)
            {
                int random = Random.Range(0, RandomParticleSystems.Length);
                HandleParticleSystemAction(RandomParticleSystems[random]);
            }
            else if (BoundParticleSystem != null)
            {
                HandleParticleSystemAction(BoundParticleSystem);
            }
        }
        private void HandleParticleSystemAction(ParticleSystem targetParticleSystem)
        {
            if (ForceSimulationSpeed)
            {
                var main = targetParticleSystem.main;
                main.simulationSpeed = Random.Range(ForcedSimulationSpeed.x, ForcedSimulationSpeed.y);
            }

            if (ChangeParentOnPlay)
            {
                targetParticleSystem.transform.SetParent(ParentOnPlay, WorldPositionStays);
            }

            switch (Mode)
            {
                case Modes.Play:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Play(WithChildrenParticles);
                    break;
                case Modes.Emit:
                    _emitParams.applyShapeToPosition = true;
                    if (targetParticleSystem != null)
                        targetParticleSystem.Emit(_emitParams, EmitCount);
                    break;
                case Modes.Stop:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Stop(WithChildrenParticles);
                    break;
                case Modes.Pause:
                    if (targetParticleSystem != null)
                        targetParticleSystem.Pause(WithChildrenParticles);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StopParticles()
        {
            RandomParticleSystems?.ForEach(x => x?.Stop());
            if (BoundParticleSystem != null)
            {
                BoundParticleSystem.Stop();
            }
        }
    }
}
