using UnityEngine;
using UnityEngine.Events;

namespace DuloGames.UI.Tweens
{
	public struct Vector2Tween : ITweenValue
	{
		public class Vector2TweenCallback : UnityEvent<Vector2> {}
		public class Vector2TweenFinishCallback : UnityEvent {}
		
		private Vector2 m_StartVector2;
		private Vector2 m_TargetVector2;
		private float m_Duration;
		private bool m_IgnoreTimeScale;
		private TweenEasing m_Easing;
		private Vector2TweenCallback m_Target;
		private Vector2TweenFinishCallback m_Finish;
		
		/// <summary>
		/// Gets or sets the starting Vector2.
		/// </summary>
		/// <value>The start color.</value>
		public Vector2 startVector2
		{
			get { return m_StartVector2; }
			set { m_StartVector2 = value; }
		}
		
		/// <summary>
		/// Gets or sets the target Vector2.
		/// </summary>
		/// <value>The color of the target.</value>
		public Vector2 targetVector2
		{
			get { return m_TargetVector2; }
			set { m_TargetVector2 = value; }
		}
		
		/// <summary>
		/// Gets or sets the duration of the tween.
		/// </summary>
		/// <value>The duration.</value>
		public float duration
		{
			get { return m_Duration; }
			set { m_Duration = value; }
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.Tweens.ColorTween"/> should ignore time scale.
		/// </summary>
		/// <value><c>true</c> if ignore time scale; otherwise, <c>false</c>.</value>
		public bool ignoreTimeScale
		{
			get { return m_IgnoreTimeScale; }
			set { m_IgnoreTimeScale = value; }
		}
		
		/// <summary>
		/// Gets or sets the tween easing.
		/// </summary>
		/// <value>The easing.</value>
		public TweenEasing easing
		{
			get { return m_Easing; }
			set { m_Easing = value; }
		}
		
		/// <summary>
		/// Tweens the color based on percentage.
		/// </summary>
		/// <param name="floatPercentage">Float percentage.</param>
		public void TweenValue(float floatPercentage)
		{
			if (!this.ValidTarget())
				return;
			
			this.m_Target.Invoke( Vector2.Lerp (this.m_StartVector2, this.m_TargetVector2, floatPercentage) );
		}
		
		/// <summary>
		/// Adds a on changed callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddOnChangedCallback(UnityAction<Vector2> callback)
		{
			if (m_Target == null)
				m_Target = new Vector2TweenCallback();
			
			m_Target.AddListener(callback);
		}
		
		/// <summary>
		/// Adds a on finish callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddOnFinishCallback(UnityAction callback)
		{
			if (m_Finish == null)
				m_Finish = new Vector2TweenFinishCallback();
			
			m_Finish.AddListener(callback);
		}
		
		public bool GetIgnoreTimescale()
		{
			return m_IgnoreTimeScale;
		}
		
		public float GetDuration()
		{
			return m_Duration;
		}
		
		public bool ValidTarget()
		{
			return m_Target != null;
		}
		
		/// <summary>
		/// Invokes the on finish callback.
		/// </summary>
		public void Finished()
		{
			if (m_Finish != null)
				m_Finish.Invoke();
		}
	}
}
