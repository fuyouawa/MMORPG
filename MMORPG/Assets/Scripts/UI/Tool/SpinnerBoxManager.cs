using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpinnerBoxManager : MonoSingleton<SpinnerBoxManager>
{
    public GameObject SpinnerBox;
    public TextMeshProUGUI DescriptionText;

    public bool IsShowing { get; private set; }

    private void Start()
    {
        SpinnerBox.SetActive(false);
    }

    public void Show()
    {
        if (IsShowing)
        {
            Debug.LogWarning("当前已有SpinnerBox正在显示!");
            return;
        }
        IsShowing = true;
        PanelSwitcher.FadeIn(SpinnerBox);
    }

    public void Close()
    {
        Debug.Assert(IsShowing);
        PanelSwitcher.FadeOut(SpinnerBox);
        IsShowing = false;
    }
}

public static class SpinnerBox
{
    public static bool IsShowing => SpinnerBoxManager.Instance.IsShowing;

    public static void Show(string description, float fontSize=16)
    {
        SceneManager.Instance.Invoke(() =>
        {
            SpinnerBoxManager.Instance.DescriptionText.text = description;
            SpinnerBoxManager.Instance.DescriptionText.fontSize = fontSize;
            SpinnerBoxManager.Instance.Show();
        });
    }

    public static void Close()
    {
        SceneManager.Instance.Invoke(SpinnerBoxManager.Instance.Close);
    }
}
