using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
	public class UIHpBar : MonoBehaviour
    {
        public Image HpBar;
        public float Hp, MaxHp = 100;

        private float _lerpSpeed = 3;
        private RectTransform _hpBarRectTransform;
        private float _initialHpBarWidth;

        private void Start()
        {
            // 获取 HpBar 的 RectTransform 组件
            _hpBarRectTransform = HpBar.GetComponent<RectTransform>();
            // 记录初始宽度
            _initialHpBarWidth = _hpBarRectTransform.sizeDelta.x;
        }

        private void Update()
        {
            BarFiller();
        }

        private void BarFiller()
        {
            // 计算目标宽度
            float targetWidth = _initialHpBarWidth * (Hp / MaxHp);
            // 平滑过渡
            float newWidth = Mathf.Lerp(_hpBarRectTransform.sizeDelta.x, targetWidth, _lerpSpeed * Time.deltaTime);
            // 设置新的宽度
            _hpBarRectTransform.sizeDelta = new Vector2(newWidth, _hpBarRectTransform.sizeDelta.y);
        }
    }
}
