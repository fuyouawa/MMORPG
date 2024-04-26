using System.Collections.Generic;
using UnityEngine;

namespace DuloGames.UI
{
    public class Demo_QualitySlider : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private UISliderExtended m_Slider;
        #pragma warning restore 0649

        private void Start()
        {
            List<string> graphicsQualityList = new List<string>(QualitySettings.names.Length);

            foreach (string name in QualitySettings.names)
            {
                graphicsQualityList.Add(name);
            }

            this.m_Slider.options = graphicsQualityList;
            this.m_Slider.value = QualitySettings.GetQualityLevel();
        }
    }
}
