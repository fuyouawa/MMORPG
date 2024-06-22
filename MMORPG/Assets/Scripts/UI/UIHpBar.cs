using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIHpBar : MonoBehaviour
    {
        public Image LerpFillImage;
        public Image FillImage;
        public TextMeshProUGUI TextHp;
        public int Hp, MaxHp = 100;

        private float _lerpSpeed = 3;

        public void UpdateValue(int hp, int maxHp)
        {
            Hp = hp;
            MaxHp = maxHp;
        }

        private void Update()
        {
            TextHp.text = $"{MaxHp} / {Hp}";
            BarFiller();
        }

        private void BarFiller()
        {
            var per = (float)Hp / MaxHp;
            FillImage.fillAmount = per;
            LerpFillImage.fillAmount = Mathf.Lerp(LerpFillImage.fillAmount, per, _lerpSpeed * Time.deltaTime);
        }
    }

}
