using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.MUIP
{
    [RequireComponent(typeof(Animator))]
    public class WindowManagerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool enableMobileMode = false;
        [HideInInspector] public Animator buttonAnimator;

        void Awake()
        {
            if (buttonAnimator == null) { buttonAnimator = gameObject.GetComponent<Animator>(); }
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) { enableMobileMode = true; }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableMobileMode == true)
                return;

            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed")
                && !buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Normal to Hover");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (enableMobileMode == true)
                return;

            if (!buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hover to Pressed")
                && !buttonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                buttonAnimator.Play("Hover to Normal");
        }
    }
}
