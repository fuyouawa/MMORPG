using MMORPG.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class Health : AbstractHealth
    {
        [Title("Binding")]
        public EntityView Entity;
        [Title("Health")]
        public float Invincibility = 0.5f;
        [Title("Damage Number")]
        public Transform DamageNumberPoint;
        [Title("Feedbacks")]
        public FeedbacksManager HurtFeedbacks;

        public override float GetInvincibilityDuration(float extraInvincibilityDuration)
        {
            return Invincibility + extraInvincibilityDuration;
        }

        protected override void Awake()
        {
            base.Awake();
            if (HurtFeedbacks != null)
                HurtFeedbacks.Initialize();
        }

        protected override void OnHurt(float damage, GameObject instigator, Vector3 damageDirection)
        {
            if (HurtFeedbacks != null)
                HurtFeedbacks.Play();
            LevelManager.Instance.TakeDamage(LevelDamageNumberType.Monster, DamageNumberPoint.position, damage);
        }
    }
}
