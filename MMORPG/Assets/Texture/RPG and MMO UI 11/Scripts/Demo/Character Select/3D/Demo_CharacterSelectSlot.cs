using UnityEngine;
using UnityEngine.EventSystems;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
    public class Demo_CharacterSelectSlot : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Light m_Light;
        #pragma warning restore 0649

        private Demo_CharacterInfo m_Info;
        private int m_Index;
        private float m_Intensity;

        public Demo_CharacterInfo info
        {
            get { return this.m_Info; }
            set { this.m_Info = value; }
        }

        public int index
        {
            get { return this.m_Index; }
            set { this.m_Index = value; }
        }

        // Tween controls
		[System.NonSerialized] private readonly TweenRunner<FloatTween> m_TweenRunner;
		
		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected Demo_CharacterSelectSlot()
		{
			if (this.m_TweenRunner == null)
				this.m_TweenRunner = new TweenRunner<FloatTween>();
			
			this.m_TweenRunner.Init(this);
		}

        protected void Awake()
        {
            if (this.m_Light != null)
            {
                this.m_Light.enabled = false;
                this.m_Intensity = this.m_Light.intensity;
                this.m_Light.intensity = 0;
            }
        }

        public void OnSelected()
        {
            if (this.m_Light != null)
            {
                this.m_Light.enabled = true;
                this.StartIntensityTween(this.m_Intensity, 0.3f);
            }
        }

        public void OnDeselected()
        {
            if (this.m_Light != null)
            {
                this.m_Light.enabled = false;
                this.m_Light.intensity = 0f;
            }
        }

        private void OnMouseDown()
        {
            if (this.m_Info == null)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Demo_CharacterSelectMgr.instance != null)
            {
                Demo_CharacterSelectMgr.instance.SelectCharacter(this);
            }
        }
        
		private void StartIntensityTween(float target, float duration)
		{
			if (this.m_Light == null)
				return;
			
			if (!Application.isPlaying || duration == 0f)
			{
				this.m_Light.intensity = target;
			}
			else
			{
				var colorTween = new FloatTween { duration = duration, startFloat = this.m_Light.intensity, targetFloat = target };
				colorTween.AddOnChangedCallback(SetIntensity);
				colorTween.ignoreTimeScale = true;
				
				this.m_TweenRunner.StartTween(colorTween);
			}
		}

        private void SetIntensity(float intensity)
        {
            if (this.m_Light == null)
				return;
            
            this.m_Light.intensity = intensity;
        }
    }
}
