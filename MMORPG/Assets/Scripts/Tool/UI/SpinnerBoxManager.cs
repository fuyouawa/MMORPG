using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 旋转加载框的配置
/// </summary>
public record SpinnerBoxConfig
{
    public string Description = "我是一个旋转加载面板";
    public float DescriptionFontSize = 16;
}

public class SpinnerBoxManager : MonoBehaviour
{
    public GameObject SpinnerBox;
    public TextMeshProUGUI DescriptionText;

    public SpinnerBoxConfig Config { get; set; }

    public bool IsShowing { get; private set; }

    private void Start()
    {
        SpinnerBox.SetActive(false);
    }

    public void Show()
    {
        if (IsShowing)
        {
            Debug.LogWarning("当前SpinnerBox正在显示!");
            return;
        }
        DescriptionText.text = Config.Description;
        DescriptionText.fontSize = Config.DescriptionFontSize;
        IsShowing = true;
        PanelHelper.FadeIn(SpinnerBox);
    }

    public void Close()
    {
        Debug.Assert(IsShowing);
        PanelHelper.FadeOut(SpinnerBox);
        IsShowing = false;
    }
}
