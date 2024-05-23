using MMORPG.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public abstract class PlayerAbility : MonoBehaviour
    {
        [FoldoutGroup("Feedbacks")]
        public FeedbacksManager EnterAbilityFeedbacks;
        [FoldoutGroup("Feedbacks")]
        public FeedbacksManager ExitAbilityFeedbacks;

        public PlayerState OwnerState { get; set; }
        public PlayerBrain Brain { get; set; }
        public int OwnerStateId { get; set; }

        public virtual void OnStateInit()
        {
        }

        public virtual void OnStateEnter()
        {
        }

        public virtual void OnStateUpdate()
        {
        }

        public virtual void OnStateFixedUpdate()
        {
        }

        public virtual void OnStateNetworkFixedUpdate()
        {
        }

        public virtual void OnStateExit()
        {
        }
    }

}
