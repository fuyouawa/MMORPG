using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MMORPG.Tool
{
    [AddFeedbackMenu("Particle/ParticlePlay")]
    public class FeedbackParticlePlay : Feedback
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
        public ParticleSystem[] RandomParticleSystems = Array.Empty<ParticleSystem>();
        [FoldoutGroup("Bound Particles")]
        public bool MoveToPosition = false;
        [FoldoutGroup("Bound Particles")]
        public bool ActivateOnPlay = false;
        [FoldoutGroup("Bound Particles")]
        public bool StopSystemOnInit = true;
        [FoldoutGroup("Bound Particles")]
        public float DeclaredDuration = 0f;

        [FoldoutGroup("Simulation Speed")]
        public bool ForceSimulationSpeed = false;
        [FoldoutGroup("Simulation Speed")]
        [ShowIf("ForceSimulationSpeed")]
        public Vector2 ForcedSimulationSpeed = new(0.1f, 1f);

        private ParticleSystem.EmitParams _emitParams;

        protected override float GetDuration()
        {
            return DeclaredDuration;
        }

        protected override void OnFeedbackInit()
        {
            if (StopSystemOnInit)
            {
                StopParticles();
            }
        }

        protected override void OnFeedbackStart()
        {
            PlayParticles(Owner.transform.position);
        }

        protected override void OnFeedbackStop()
        {
            StopParticles();
        }

        private void PlayParticles(Vector3 position)
        {
            if (MoveToPosition)
            {
                if (Mode != Modes.Emit)
                {
                    BoundParticleSystem.transform.position = position;

                    RandomParticleSystems?.ForEach(x =>
                    {
                        if (x) x.transform.position = position;
                    });
                }
                else
                {
                    _emitParams.position = position;
                }
            }

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

            switch (Mode)
            {
                case Modes.Play:
                    targetParticleSystem?.Play();
                    break;
                case Modes.Emit:
                    _emitParams.applyShapeToPosition = true;
                    targetParticleSystem.Emit(_emitParams, EmitCount);
                    break;
                case Modes.Stop:
                    targetParticleSystem?.Stop();
                    break;
                case Modes.Pause:
                    targetParticleSystem?.Pause();
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
