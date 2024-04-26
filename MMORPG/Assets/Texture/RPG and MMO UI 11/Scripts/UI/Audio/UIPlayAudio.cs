using UnityEngine;
using UnityEngine.EventSystems;

namespace DuloGames.UI
{
    [AddComponentMenu("UI/Audio/Play Audio")]
    public class UIPlayAudio : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum Event
        {
            None,
            PointerEnter,
            PointerExit,
            PointerDown,
            PointerUp,
            Click,
            DoubleClick
        }

        [SerializeField] private AudioClip m_AudioClip;
        [SerializeField][Range(0f, 1f)] private float m_Volume = 1f;
        [SerializeField] private Event m_PlayOnEvent = Event.None;

        /// <summary>
        /// Gets or sets the audio clip.
        /// </summary>
        public AudioClip audioClip { get { return this.m_AudioClip; } set { this.m_AudioClip = value; } }

        /// <summary>
        /// Gets or sets the volume level.
        /// </summary>
        public float volume { get { return this.m_Volume; } set { this.m_Volume = value; } }

        /// <summary>
        /// Gets or sets the event on which the audio clip should be played.
        /// </summary>
        public Event playOnEvent { get { return this.m_PlayOnEvent; } set { this.m_PlayOnEvent = value; } }
        
        private bool m_Pressed = false;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.m_Pressed)
                this.TriggerEvent(Event.PointerEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!this.m_Pressed)
                this.TriggerEvent(Event.PointerExit);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            this.TriggerEvent(Event.PointerDown);

            this.m_Pressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            this.TriggerEvent(Event.PointerUp);

            if (this.m_Pressed)
            {
                if (eventData.clickCount > 1)
                {
                    this.TriggerEvent(Event.DoubleClick);
                    eventData.clickCount = 0;
                }
                else
                {
                    this.TriggerEvent(Event.Click);
                }
            }

            this.m_Pressed = false;
        }

        private void TriggerEvent(Event e)
        {
            if (e == this.m_PlayOnEvent)
            {
                this.PlayAudio();
            }
        }

        public void PlayAudio()
        {
            if (!this.enabled || !this.gameObject.activeInHierarchy)
            {
                return;
            }

            if (this.m_AudioClip == null)
            {
                return;
            }

            if (UIAudioSource.Instance == null)
            {
                Debug.LogWarning("You dont have UIAudioSource in your scene. Cannot play audio clip.");
                return;
            }

            // Play the audio clip
            UIAudioSource.Instance.PlayAudio(this.m_AudioClip, this.m_Volume);
        }
    }
}
