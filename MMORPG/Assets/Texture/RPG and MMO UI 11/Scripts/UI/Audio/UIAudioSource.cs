using UnityEngine;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/Audio/Audio Source")]
    [DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
    public class UIAudioSource : MonoBehaviour
    {
        #region Singleton
        private static UIAudioSource m_Instance;
        public static UIAudioSource Instance { get { return m_Instance; } }
        #endregion

        [SerializeField][Range(0f, 1f)] private float m_Volume = 1f;

        /// <summary>
        /// Gets or sets the volume level.
        /// </summary>
        public float volume { get { return this.m_Volume; } set { this.m_Volume = value; } }

        private AudioSource m_AudioSource;

        protected void Awake()
        {
            if (m_Instance != null)
            {
                Debug.LogWarning("You have more than one UIAudioSource in the scene, please make sure you have only one.");
                return;
            }

            m_Instance = this;

            // Get the audio source
            this.m_AudioSource = this.gameObject.GetComponent<AudioSource>();
            this.m_AudioSource.playOnAwake = false;
        }

        public void PlayAudio(AudioClip clip)
        {
            this.m_AudioSource.PlayOneShot(clip, this.m_Volume);
        }

        public void PlayAudio(AudioClip clip, float volume)
        {
            this.m_AudioSource.PlayOneShot(clip, this.m_Volume * volume);
        }
    }
}
