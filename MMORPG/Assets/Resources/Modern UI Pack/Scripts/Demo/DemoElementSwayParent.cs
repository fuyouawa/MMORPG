using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Michsky.MUIP
{
    public class DemoElementSwayParent : MonoBehaviour
    {
        [SerializeField] private Animator titleAnimator;
        [SerializeField] private TextMeshProUGUI elementTitle;
        [SerializeField] private TextMeshProUGUI elementTitleHelper;

        private List<DemoElementSway> elements = new List<DemoElementSway>();
        private int prevIndex;

        void Awake()
        {
            foreach (Transform child in transform)
            {
                elements.Add(child.GetComponent<DemoElementSway>());
            }
        }

        public void DissolveAll(DemoElementSway currentSway)
        {
            for (int i = 0; i < elements.Count; ++i)
            {
                if (elements[i] == currentSway)
                {
                    elements[i].Active();
                    continue;
                }

                elements[i].Dissolve();
            }
        }

        public void HighlightAll()
        {
            for (int i = 0; i < elements.Count; ++i)
            {
                elements[i].Highlight();
            }
        }

        public void SetWindowManagerButton(int index)
        {
            if (elements.Count == 0)
            {
                StartCoroutine("SWMHelper", index);
                return;
            }

            for (int i = 0; i < elements.Count; ++i)
            {
                if (i == index) { elements[i].WindowManagerSelect(); }
                else
                {
                    if (elements[i].wmSelected == false) { continue; }
                    elements[i].WindowManagerDeselect(); 
                }
            }

            if (titleAnimator == null)
                return;

            elementTitleHelper.text = elements[prevIndex].gameObject.name;
            elementTitle.text = elements[index].gameObject.name;

            titleAnimator.Play("Idle");
            titleAnimator.Play("Transition");

            prevIndex = index;
        }

        IEnumerator SWMHelper(int index)
        {
            yield return new WaitForSeconds(0.1f);
            SetWindowManagerButton(index);
        }
    }
}