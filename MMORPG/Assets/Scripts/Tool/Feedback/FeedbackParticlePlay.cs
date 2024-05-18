using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    public class FeedbackParticlePlay : Feedback
    {
        public enum Modes
        {
            Play,
            Pause,
            Stop
        }

        [FoldoutGroup("Particle Play")]
        public Modes Mode = Modes.Play;
        [FoldoutGroup("Particle Play")]
        public ParticleSystem Particle;

        protected override void OnFeedbackStart()
        {
            if (Particle != null)
            {
                switch (Mode)
                {
                    case Modes.Play:
                        Particle.Play();
                        break;
                    case Modes.Pause:
                        Particle.Pause();
                        break;
                    case Modes.Stop:
                        Particle.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void OnFeedbackStop()
        {
            Particle?.Stop();
        }
    }
}
