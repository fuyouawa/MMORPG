using UnityEngine;
using DuloGames.UI.Tweens;

namespace DuloGames.UI
{
    public class Test_CompassTester : MonoBehaviour
    {

        [SerializeField]
        private float m_Duration = 4f;
        [SerializeField]
        private TweenEasing m_Easing = TweenEasing.Linear;

        // Tween controls
        [System.NonSerialized]
        private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        protected Test_CompassTester()
        {
            if (this.m_FloatTweenRunner == null)
                this.m_FloatTweenRunner = new TweenRunner<FloatTween>();

            this.m_FloatTweenRunner.Init(this);
        }

        protected void OnEnable()
        {
            this.StartTween(360f, this.m_Duration, true);
        }

        /// <summary>
        /// Tweens the transform rotation.
        /// </summary>
        /// <param name="targetRotation">Target rotation.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
        private void StartTween(float targetRotation, float duration, bool ignoreTimeScale)
        {
            float currentRotation = this.transform.eulerAngles.y;

            if (currentRotation.Equals(targetRotation))
                return;

            var floatTween = new FloatTween { duration = duration, startFloat = currentRotation, targetFloat = targetRotation };
            floatTween.AddOnChangedCallback(SetRotation);
            floatTween.AddOnFinishCallback(OnTweenFinished);
            floatTween.ignoreTimeScale = ignoreTimeScale;
            floatTween.easing = this.m_Easing;
            this.m_FloatTweenRunner.StartTween(floatTween);
        }

        /// <summary>
        /// Sets the transform rotation.
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        private void SetRotation(float rotation)
        {
            this.transform.eulerAngles = new Vector3(this.transform.rotation.x, rotation, this.transform.rotation.z);
        }

        /// <summary>
        /// Raises the list tween finished event.
        /// </summary>
        protected virtual void OnTweenFinished()
        {
            this.StartTween(360f, this.m_Duration, true);
        }
    }
}
