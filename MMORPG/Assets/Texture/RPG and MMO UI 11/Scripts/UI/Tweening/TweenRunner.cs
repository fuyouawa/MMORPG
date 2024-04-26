using System.Collections;
using UnityEngine;

namespace DuloGames.UI.Tweens
{
	// Tween runner, executes the given tween.
	// The coroutine will live within the given 
	// behaviour container.
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;
		protected IEnumerator m_Tween;
		
		// utility function for starting the tween
		private static IEnumerator Start(T tweenInfo)
		{
			if (!tweenInfo.ValidTarget())
				yield break;
            
            float elapsedTime = 0.0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += (tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
                
                float percentage = TweenEasingHandler.Apply(tweenInfo.easing, elapsedTime, 0.0f, 1.0f, tweenInfo.duration);
				tweenInfo.TweenValue(percentage);
                
                yield return null;
			}
			tweenInfo.TweenValue(1.0f);
			tweenInfo.Finished();
		}
		
		public void Init(MonoBehaviour coroutineContainer)
		{
            this.m_CoroutineContainer = coroutineContainer;
		}
		
		public void StartTween(T info)
		{
            if (this.m_CoroutineContainer == null)
			{
				Debug.LogWarning ("Coroutine container not configured... did you forget to call Init?");
				return;
			}

            this.StopTween();
            
			if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1.0f);
				return;
			}
            
            this.m_Tween = Start (info);
            this.m_CoroutineContainer.StartCoroutine (this.m_Tween);
		}

        public void StopTween()
        {
            if (this.m_Tween != null)
            {
                this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
                this.m_Tween = null;
            }
        }
    }
}
