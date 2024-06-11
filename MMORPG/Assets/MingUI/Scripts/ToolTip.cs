using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{

    public static ToolTip Instance {  get; private set; }


    public int offsetX = 0;
    public int offsetY = 0;

    private Text toolTipText;
    private Text contentText;
    private CanvasGroup canvasGroup;

    private float targetAlpha = 0;
    private float smoothing = 6f;
    private void Start()
    {
        Instance = this;
        toolTipText = GetComponent<Text>();
        contentText = transform.Find("Content").GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
        transform.position = Input.mousePosition + new Vector3(offsetX,offsetY,0);
        transform.SetAsLastSibling();
    }
    public void Show(string text)//显示提示信息
    {
        targetAlpha = 1;
        toolTipText.text = text;
        contentText.text = text;
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

