using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ToolTipAlignment
{
    BottomLeft
}

public class ToolTip : MonoBehaviour
{

    public static ToolTip Instance {  get; private set; }

    public TextMeshProUGUI ToolTipText;
    public ToolTipAlignment Alignment = ToolTipAlignment.BottomLeft;

    public Vector2 Offset;

    private CanvasGroup canvasGroup;
    private RectTransform _rectTransform;

    private float targetAlpha = 0;
    private float smoothing = 6f;

    private void Start()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = transform as RectTransform;
    }
    private void Update()
    {
        if (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
        }
    }
    public void Show(Transform follow, string text)//显示提示信息
    {
        targetAlpha = 1;
        ToolTipText.text = text;

        var alignmentOffset = Alignment switch
        {
            ToolTipAlignment.BottomLeft => new Vector3(
                _rectTransform.sizeDelta.x / 2,
                -_rectTransform.sizeDelta.y / 2, 0),
            _ => throw new ArgumentOutOfRangeException()
        };

        transform.position = follow.position + alignmentOffset + new Vector3(Offset.x, Offset.y, 0);
        transform.SetAsLastSibling();
    }
    public void Hide()//隐藏提示信息
    {
        targetAlpha = 0;
    }
    public void SetClocalPosition(Vector3 position)//设置信息面板位置
    {
        transform.localPosition = position;
    }
}

