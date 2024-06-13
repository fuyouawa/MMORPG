using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIHpBar : MonoBehaviour
    {
        public Image FillImage;
        public float Hp, MaxHp = 100;

        private float _lerpSpeed = 3;

        private void Update()
        {
            BarFiller();
        }

        private void BarFiller()
        {
            FillImage.fillAmount = Mathf.Lerp(FillImage.fillAmount, Hp / MaxHp, _lerpSpeed * Time.deltaTime);
        }
    }

}
