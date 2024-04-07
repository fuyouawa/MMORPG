using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PanelSwitcher
{
    private static CanvasGroup GetOrCreateCanvasGroup(GameObject obj, float initAlpha = 1)
    {
        if (!obj.TryGetComponent<CanvasGroup>(out var cg))
        {
            cg = obj.AddComponent<CanvasGroup>();
            cg.alpha = initAlpha;
        }
        return cg;
    }

    public static void FadeIn(
        GameObject target,
        float duration = 0.3f,
        bool autoActive = true,
        Action onComplete = null)
    {
        if (autoActive) target.SetActive(true);
        var cg = GetOrCreateCanvasGroup(target, 0);
        var twe = cg.DOFade(1, duration);
        twe.onComplete += () => onComplete?.Invoke();
    }

    public static void FadeOut(
        GameObject target,
        float duration = 0.3f,
        bool autoActive = true,
        Action onComplete = null)
    {
        var cg = GetOrCreateCanvasGroup(target);
        var twe = cg.DOFade(0, duration);
        twe.onComplete += () =>
        {
            if (autoActive) target.SetActive(false);
            onComplete?.Invoke();
        };
    }
}
