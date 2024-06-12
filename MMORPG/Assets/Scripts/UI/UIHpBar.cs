using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIHpPanel : MonoBehaviour
    {
        public Image HpBar;
        public float Hp, MaxHp = 100;

        private float _lerpSpeed = 3;

        private void Update()
        {
            BarFiller();
        }

        private void BarFiller()
        {
            HpBar.fillAmount = Mathf.Lerp(HpBar.fillAmount, Hp / MaxHp, _lerpSpeed * Time.deltaTime);
        }
    }

}
