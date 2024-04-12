using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BlackFieldManager : MonoBehaviour
{
    public bool FadeOutOnStart = true;

    private Image _image;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (FadeOutOnStart)
        {
            FadeOut();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void FadeIn(float time = 0.5f, Action onComplete = null)
    {
        gameObject.SetActive(true);
        var twe = _canvasGroup.DOFade(1, time);
        twe.onComplete += () =>
        {
            onComplete?.Invoke();
        };
    }

    public void FadeOut(float time = 0.5f, Action onComplete = null)
    {
        var twe = _canvasGroup.DOFade(0, time);
        twe.onComplete += () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        };
    }
}
